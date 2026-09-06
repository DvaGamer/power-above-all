using System;
using System.Globalization;
using PowerAboveAll;

// Saf kabul/denge gozlemi. Dosya, Unity, saat, savas sonucu veya insan kaydi kullanmaz.
public static class RegionalAccordBalanceProbe
{
    sealed class Policy
    {
        public string Name, Region = "champagne";
        public bool Accord, BreakAfterTwo, Recruit, March;
    }
    static int checks;
    static string F(float value) { return value.ToString("0.###", CultureInfo.InvariantCulture); }
    static string Snapshot(CampaignState s) { return CampaignArchive.Serialize(s, false); }
    static CampaignState Clone(CampaignState s) { return CampaignArchive.Deserialize(Snapshot(s)); }
    static float Morel(CampaignState s) { return s.Characters.Find(c => c.Id == "morel").Relationship; }
    static float Assembly(CampaignState s) { return s.Factions.Find(f => f.Id == "assembly").Approval; }
    static void Check(bool condition, string name)
    { if (!condition) throw new Exception("FAIL: " + name); checks++; }
    static void Success(ActionResult action, string name)
    { Check(action != null && action.Ok, name + ": " + (action == null ? "null" : action.Key)); }
    static void RefusedUnchanged(CampaignState s, Func<ActionResult> action, string name)
    {
        string before = Snapshot(s);
        var result = action();
        Check(result != null && !result.Ok && Snapshot(s) == before, name);
    }

    // Ayni yerel/kurumsal durumun marjinal vergi bedeli; baska politikanin sonucu degildir.
    static int TaxWithoutHolidayAtSameState(CampaignState s)
    {
        var comparison = Clone(s);
        comparison.AccordRegionId = "";
        CampaignCore.Validate(comparison);
        return CampaignCore.Forecast(comparison).TaxIncome;
    }
    static void GrantAndCheck(CampaignState s, string region)
    {
        string before = Snapshot(s);
        var terms = CampaignCore.GetRegionalAccordTerms(s, region);
        Check(terms != null && terms.RemainingWeeks == 4 && terms.UntilWeek == s.Week + 4, "new offer keeps four original calculations");
        Check(terms.CurrentTaxIncome == CampaignCore.Forecast(s).TaxIncome && Snapshot(s) == before, "new offer is read-only at current total tax");
        Success(CampaignCore.GrantRegionalAccord(s, region), "grant");
        Check(CampaignCore.Forecast(s).TaxIncome == terms.ProjectedTaxIncome, "grant total tax equals preview");
        Check(TaxWithoutHolidayAtSameState(s) - CampaignCore.Forecast(s).TaxIncome == terms.TaxForgone, "grant foregone tax uses identical post-grant state");
        RefusedUnchanged(s, () => CampaignCore.GrantRegionalAccord(s, "normandy"), "second same-week grant cannot farm another region");
    }

    static string Run(Policy p, bool archiveBetweenWeeks, bool print)
    {
        var s = CampaignCore.Create();
        int totalForgone = 0, totalTax = 0, breaks = 0, recruitRefusals = 0;
        for (int step = 0; step < 12; step++)
        {
            Check(s.Week == step, p.Name + " week advances exactly once");
            if (s.PendingPetition) Success(CampaignCore.ChoosePetition(s, "negotiate"), "same petition choice for every policy");
            if (p.Accord && s.Week % 4 == 0) GrantAndCheck(s, p.Region);
            if (p.Recruit && s.Week % 4 == 1)
            {
                string before = Snapshot(s);
                var recruited = CampaignCore.Act(s, "recruit", s.ArmyRegionId);
                if (!recruited.Ok)
                {
                    recruitRefusals++;
                    Check(Snapshot(s) == before, "unaffordable recruitment is atomic");
                    if (print) Console.WriteLine("NOTE\t" + p.Name + "\tweek=" + step + "\trecruit-refused=" + recruited.Key);
                }
            }
            if (p.BreakAfterTwo && s.Week % 4 == 2)
            {
                int until = s.AccordUntilWeek;
                Success(CampaignCore.Act(s, "tax", p.Region), "break at second week via actual tax");
                breaks++;
                Check(!CampaignCore.HasRegionalAccord(s) && s.AccordUntilWeek == until, "tax break preserves original renewal deadline");
                RefusedUnchanged(s, () => CampaignCore.GrantRegionalAccord(s, "normandy"), "break cannot reopen same-week grant on another region");
                RefusedUnchanged(s, () => CampaignCore.Act(s, "tax", p.Region), "second used tax cannot repeat money or break effects");
            }
            if (p.March && s.Week == 0)
            {
                var march = CampaignCore.CanMarch(s, p.Region);
                Check(march.Ok && !march.RequiresBattle, "accord creates a peaceful actual route");
                var preview = CampaignCore.PreviewMarch(s, p.Region);
                Check(preview != null && preview.Difficult && preview.FoodCost == 18, "peaceful route still pays difficult march cost");
                Success(CampaignCore.March(s, p.Region), "peaceful march");
                Check(s.ArmyRegionId == p.Region && s.Food == preview.FoodAfter && s.MilitarySupplies == preview.MilitarySuppliesAfter && s.Moves == preview.MovesAfter, "actual peaceful march equals preview");
                Check(s.ResolvedBattles.Count == 0, "peaceful access creates no fictitious battle history");
            }
            var forecast = CampaignCore.Forecast(s);
            int withoutHoliday = TaxWithoutHolidayAtSameState(s);
            int forgone = withoutHoliday - forecast.TaxIncome;
            int oldUntil = s.AccordUntilWeek;
            int remaining = CampaignCore.HasRegionalAccord(s) ? oldUntil - s.Week : 0;
            if (remaining > 0)
            {
                string before = Snapshot(s);
                var active = CampaignCore.GetActiveRegionalAccordTerms(s);
                Check(active.RegionId == p.Region && active.UntilWeek == oldUntil && active.RemainingWeeks == remaining && active.TaxForgone == forgone, "active quote follows signed region/deadline and marginal tax");
                Check(Snapshot(s) == before, "active quote is read-only");
            }
            else Check(forgone == 0, "expired or broken holiday has no remaining tax exemption");
            int oldGold = s.Gold;
            float oldAssembly = Assembly(s), oldMorel = Morel(s);
            Success(CampaignCore.NextWeek(s), "weekly economic calculation");
            Check(s.Gold == Math.Max(0, oldGold + forecast.NetGold), "booked gold reconciles with pre-week forecast");
            if (remaining == 1)
            {
                Check(!CampaignCore.HasRegionalAccord(s) && s.AccordRegionId == "" && s.AccordUntilWeek == oldUntil, "fourth calculation ends accord without changing deadline");
                Check(Assembly(s) == Math.Min(100, oldAssembly + 5) && Morel(s) == Math.Min(100, oldMorel + 4), "honour reward follows fourth calculation");
            }
            else Check(Assembly(s) == oldAssembly && Morel(s) == oldMorel, "other weeks cannot duplicate honour reward");
            totalTax += forecast.TaxIncome; totalForgone += forgone;
            if (archiveBetweenWeeks)
            {
                string before = Snapshot(s);
                s = CampaignArchive.Deserialize(before);
                Check(Snapshot(s) == before, "weekly v3 archive preserves complete campaign and deadline");
            }
            CampaignCore.Validate(s);
            if (print)
            {
                var r = CampaignCore.Region(s, p.Region);
                Console.WriteLine(string.Join("\t", new[] { "WEEK", p.Name, step.ToString(), s.Week.ToString(), forecast.TaxIncome.ToString(), withoutHoliday.ToString(), forgone.ToString(), remaining.ToString(), oldUntil.ToString(), s.AccordRegionId, s.AccordUntilWeek.ToString(), Math.Max(0, s.AccordUntilWeek - s.Week).ToString(), s.Gold.ToString(), s.Food.ToString(), s.MilitarySupplies.ToString(), s.Troops.ToString(), s.Manpower.ToString(), F(s.Power), F(s.Morale), F(s.Supply), F(r.Unrest), F(r.Control), F(Morel(s)), F(Assembly(s)), s.ArmyRegionId, (r.Unrest >= 65).ToString() }));
            }
        }
        if (print) Console.WriteLine("SUMMARY\t" + p.Name + "\tbookedTax=" + totalTax + "\tmarginalForgone=" + totalForgone + "\tgold=" + s.Gold + "\tfood=" + s.Food + "\ttroops=" + s.Troops + "\tbreaks=" + breaks + "\trecruitRefusals=" + recruitRefusals);
        return Snapshot(s);
    }

    static void ExistingRoleAndPetitionJourney()
    {
        var s = CampaignCore.Create("assembly");
        Success(CampaignCore.IssueMandate(s, "champagne"), "assembly opens its distinct pledge");
        string originalMandate = CampaignCore.MandateId(s.Obligation);
        GrantAndCheck(s, "champagne");
        Success(CampaignCore.NextWeek(s), "coexisting commitments week1");
        Success(CampaignCore.Act(s, "recruit", s.ArmyRegionId), "recruit before commitments complete");
        s = Clone(s);
        Success(CampaignCore.NextWeek(s), "coexisting commitments week2");
        Check(s.PendingPetition && CampaignCore.MandateDue(s), "petition and role deadline coexist at week2");
        RefusedUnchanged(s, () => CampaignCore.NextWeek(s), "petition barrier cannot age regional accord");
        RefusedUnchanged(s, () => CampaignCore.GrantRegionalAccord(s, "normandy"), "pending petition grant is atomic");
        Success(CampaignCore.ChoosePetition(s, "negotiate"), "resolve petition normally");
        RefusedUnchanged(s, () => CampaignCore.NextWeek(s), "role barrier cannot age regional accord");
        Check(CampaignCore.MandateId(s.Obligation) == originalMandate && s.AccordUntilWeek == 4, "recruit/archive/barriers preserve both deadlines");
        Success(CampaignCore.ResolveMandate(s, originalMandate, "fulfil"), "fulfil original role pledge");
        float oldMorel = Morel(s), oldAssembly = Assembly(s);
        Success(CampaignCore.NextWeek(s), "accord week3 after role fulfilment");
        Check(CampaignCore.HasRegionalAccord(s) && s.AccordUntilWeek == 4, "role fulfilment does not finish accord early");
        Success(CampaignCore.NextWeek(s), "accord week4 after role fulfilment");
        Check(!CampaignCore.HasRegionalAccord(s) && Morel(s) == Math.Min(100, oldMorel + 4) && Assembly(s) == Math.Min(100, oldAssembly + 5), "regional reward remains separate after role recruitment journey");
        Success(CampaignCore.NextWeek(s), "no new regional modal blocks week5");
        Console.WriteLine("JOURNEY\tassembly-pledge-recruit-petition-accord\tPASS\tweek=" + s.Week + "\tMorel=" + F(Morel(s)) + "\tAssembly=" + F(Assembly(s)));
    }

    static void ZeroStocksCanRecover()
    {
        var s = CampaignCore.Create();
        // Acik sinir fixture'i; oynanmis ekonomi veya savas sayilmaz.
        s.Gold = s.Food = s.MilitarySupplies = s.Troops = 0; s.Power = 0;
        GrantAndCheck(s, "champagne");
        for (int week = 0; week < 5; week++)
        {
            if (s.PendingPetition) Success(CampaignCore.ChoosePetition(s, "negotiate"), "zero-stock petition can negotiate");
            Success(CampaignCore.NextWeek(s), "zero-stock accord never adds a weekly lock");
            s = Clone(s);
        }
        Check(s.Gold > 0 && s.Food > 0 && !CampaignCore.HasRegionalAccord(s), "zero-stock state recovers through actual weekly economy");
        Console.WriteLine("BOUNDARY\tzero-stocks-power-and-army\tPASS\tgold=" + s.Gold + "\tfood=" + s.Food + "\tweek=" + s.Week);
    }

    public static int Main()
    {
        try
        {
            Console.WriteLine("PURE CORE PROBE: hypothetical policy trajectories, not new battle evidence; no files read or written.");
            Console.WriteLine("EXTERNAL REFERENCE ONLY: tactical-trust-first-20260905-233324-829-3db06d4c observed natural victory at125.803s with196casualties and24supplies. No battle outcome is injected by this program.");
            var start = CampaignCore.Create();
            Check(CampaignCore.CanMarch(start, "champagne").RequiresBattle, "baseline Champagne requires battle");
            Console.WriteLine("ROW\tpolicy\tweekFrom\tweekTo\tbookedTax\tsameStateNoHolidayTax\tmarginalForgone\tcalculationsBefore\toriginalUntilBefore\tactiveRegionAfter\tuntilAfter\trenewInAfter\tgoldAfter\tfoodAfter\tsuppliesAfter\ttroopsAfter\tmanpowerAfter\tpowerAfter\tmoraleAfter\tsupplyAfter\ttargetUnrestAfter\ttargetControlAfter\tMorelAfter\tAssemblyAfter\tarmyRegionAfter\ttargetHostileAfter");
            var policies = new[] {
                new Policy { Name = "baseline-stay" },
                new Policy { Name = "honour-every-four-stay", Accord = true },
                new Policy { Name = "break-after-two-stay", Accord = true, BreakAfterTwo = true },
                new Policy { Name = "baseline-recruit-1-5-9", Recruit = true },
                new Policy { Name = "honour-recruit-1-5-9", Accord = true, Recruit = true },
                new Policy { Name = "honour-peaceful-march", Accord = true, March = true },
                new Policy { Name = "honour-poitou-stay", Accord = true, Region = "poitou" }
            };
            foreach (var policy in policies)
            {
                string archived = Run(policy, true, true);
                string direct = Run(policy, false, false);
                Check(archived == direct, policy.Name + " archive/no-archive trajectories identical");
            }
            ExistingRoleAndPetitionJourney();
            ZeroStocksCanRecover();
            Console.WriteLine("PASS\t" + checks + " checks; observations remain fictional game balance.");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error.ToString()); return 1; }
    }
}
