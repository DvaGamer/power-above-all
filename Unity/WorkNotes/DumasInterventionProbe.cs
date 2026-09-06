using System;
using System.Collections.Generic;
using System.Globalization;
using PowerAboveAll;

// Sonlu arama: uc rol, alti asker toplama plani, iki yardim politikasi, en fazla32 hafta.
// Durum alanlarina yazilmaz; dallar yalniz public Archive ile kopyalanir. Dosya yazmaz.
public static class DumasInterventionProbe
{
    static int checks, campaigns, notices, attempts, sufficient, refused, printed;
    static readonly HashSet<string> examples = new HashSet<string>();
    static readonly int[][] recruitPlans = {
        new int[0], new[] { 0 }, new[] { 0, 2 }, new[] { 0, 2, 4 },
        new[] { 0, 2, 4, 6 }, new[] { 0, 2, 4, 6, 8 }
    };
    static string Save(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static void Check(bool condition, string message)
    { checks++; if (!condition) throw new Exception(message); }
    static CampaignState Copy(CampaignState state)
    {
        string snapshot = Save(state);
        var copy = CampaignArchive.Deserialize(snapshot);
        Check(snapshot == Save(copy), "Archive branch is not identical"); return copy;
    }
    static string Number(float value) { return value.ToString("0.##", CultureInfo.InvariantCulture); }
    static bool Command(CampaignState state, List<string> commands, string command, Func<ActionResult> action)
    {
        string before = Save(state); var result = action();
        commands.Add(command);
        if (!result.Ok)
        {
            refused++; Check(before == Save(state), "Refused command mutated state: " + command);
            commands.Add("# refused: " + result.Key);
        }
        else CampaignCore.Validate(state);
        return result.Ok;
    }
    static void Advance(CampaignState state, List<string> commands)
    {
        string before = Save(state); var forecast = CampaignCore.Forecast(state);
        Check(before == Save(state), "Forecast changed campaign");
        int food = state.Food, gold = state.Gold;
        Check(Command(state, commands, "week", () => CampaignCore.NextWeek(state)), "Search path cannot advance");
        Check(state.Food == Math.Max(0, food + forecast.NetFood), "Food differs from public preview");
        Check(state.Gold == Math.Max(0, gold + forecast.NetGold), "Gold differs from public preview");
    }
    static void PrintState(string label, CampaignState state)
    {
        var f = CampaignCore.Forecast(state); var t = CampaignCore.GetDumasInitiativeTerms(state);
        var general = state.Characters.Find(item => item.Id == "dumas");
        var region = CampaignCore.Region(state, state.ArmyRegionId);
        Console.WriteLine(label + " week=" + state.Week + " role=" + state.RoleId + " gold=" + state.Gold +
            " food=" + state.Food + " troops=" + state.Troops + " supplies=" + state.MilitarySupplies +
            " supply=" + Number(state.Supply) + " morale=" + Number(state.Morale) + " power=" + Number(state.Power) +
            " unrest=" + Number(region.Unrest) + " elite=" + Number(region.EliteLoyalty) +
            " dumasAmbition=" + Number(general.Ambition) + " dumasRelationship=" + Number(general.Relationship) +
            " subsidy=" + state.SubsidyParis + " tax=" + f.TaxIncome + " production=" + f.Production +
            " netFood=" + f.NetFood + " forage=" + f.ForageFood +
            " disposition=" + (t == null ? "none" : t.Disposition) +
            " due=" + state.DumasForageDueWeek + " next=" + state.DumasNextForageWeek +
            " mandate=" + (state.Obligation == null ? "none" : CampaignCore.MandateId(state.Obligation)) +
            " mandateDue=" + (state.Obligation == null ? 0 : state.Obligation.DueWeek) +
            " mandateGoldDue=" + (state.Obligation == null ? 0 : state.Obligation.GoldDue) +
            " mandateFoodDue=" + (state.Obligation == null ? 0 : state.Obligation.FoodDue) +
            " accordUntil=" + state.AccordUntilWeek);
    }
    static void TryIntervention(CampaignState source, List<string> path, int plan, string method)
    {
        bool stopSubsidy = method.StartsWith("off", StringComparison.Ordinal);
        bool mandate = method.EndsWith("mandate", StringComparison.Ordinal);
        bool accord = method.EndsWith("accord", StringComparison.Ordinal);
        if (stopSubsidy && !source.SubsidyParis) return;
        if (mandate && source.RoleId == "legacy") return;
        attempts++;
        var state = Copy(source); var commands = new List<string>(path);
        if (stopSubsidy && !Command(state, commands, "act subsidy", () => CampaignCore.Act(state, "subsidy", "ile"))) return;
        if (mandate && !Command(state, commands, "mandate issue", () => CampaignCore.IssueMandate(state, "ile"))) return;
        if (accord && !Command(state, commands, "accord grant", () => CampaignCore.GrantRegionalAccord(state, "ile"))) return;
        var terms = CampaignCore.GetDumasInitiativeTerms(state);
        if (terms == null || terms.Disposition != "sufficient") return;
        sufficient++;
        int due = source.DumasForageDueWeek, next = source.DumasNextForageWeek;
        Check(state.DumasForageDueWeek == due && state.DumasNextForageWeek == next, "Intervention changed announcement timer");
        Check(CampaignCore.Forecast(state).ForageFood == 0, "Cancelled proposal still forecasts food transfer");
        Check(terms.FoodGathered == 0 && terms.UnrestDelta == 0 && terms.EliteLoyaltyDelta == 0 &&
            terms.AmbitionDelta == 0 && terms.PowerCost == 0, "Sufficient proposal retains gathering penalties");
        string afterIntervention = Save(state);
        float ambition = state.Characters.Find(item => item.Id == "dumas").Ambition;
        float elite = CampaignCore.Region(state, state.ArmyRegionId).EliteLoyalty;
        Advance(state, commands);
        Check(state.Week == due && state.DumasForageDueWeek == 0 && state.DumasNextForageWeek == next, "Cancellation settled on wrong date");
        Check(state.Journal.Exists(entry => entry.Week == due && entry.Key == "log.dumas.sufficient"), "Missing dated cancellation report");
        Check(!state.Journal.Exists(entry => entry.Week == due && entry.Key == "log.dumas.gathered"), "Cancelled proposal also gathered");
        Check(state.Characters.Find(item => item.Id == "dumas").Ambition == ambition, "Cancellation increased ambition");
        Check(CampaignCore.Region(state, state.ArmyRegionId).EliteLoyalty == elite, "Cancellation penalized local elite");
        Copy(state);
        // Her rol/yontem icin ilk gercek ornek yazilir; arama diger yollari da sonlu olarak denetler.
        if (!examples.Add(source.RoleId + ":" + method)) return;
        printed++;
        Console.WriteLine("EXAMPLE " + printed + " method=" + method + " recruitPlan=" + plan + " commands=" + commands.Count);
        PrintState("before-intervention", source);
        PrintState("after-intervention", CampaignArchive.Deserialize(afterIntervention));
        PrintState("after-settlement", state);
        Console.WriteLine("COMMANDS-BEGIN");
        foreach (string command in commands) Console.WriteLine(command);
        Console.WriteLine("COMMANDS-END");
        // Tam kopya root'un sayisal fixture ve ek borc takibi icin; insan kaydina yazilmaz.
        Console.WriteLine("SETTLED-ARCHIVE " + Save(state));
    }
    public static void Main()
    {
        Console.WriteLine("kind=pure-core-search; player-proof=false; state-mutation=public-API-only; maxCampaigns=36; maxWeeksEach=32");
        foreach (string role in new[] { "legacy", "assembly", "army" })
        for (int plan = 0; plan < recruitPlans.Length; plan++)
        foreach (bool subsidy in new[] { true, false })
        {
            campaigns++;
            var state = CampaignCore.Create(role); var commands = new List<string> { "new" };
            if (role != "legacy") { commands.Add("role-menu"); commands.Add("role-start " + role); }
            if (subsidy) Check(Command(state, commands, "act subsidy", () => CampaignCore.Act(state, "subsidy", "ile")), "Initial subsidy failed");
            for (int week = 0; week < 32; week++)
            {
                Check(state.Week == week, "Unexpected search week");
                if (state.PendingPetition)
                    Check(Command(state, commands, "petition relief", () => CampaignCore.ChoosePetition(state, "relief")), "Reachable petition cannot be resolved");
                if (Array.IndexOf(recruitPlans[plan], week) >= 0)
                    Command(state, commands, "act recruit", () => CampaignCore.Act(state, "recruit", "ile"));
                var terms = CampaignCore.GetDumasInitiativeTerms(state);
                if (terms != null && terms.Disposition == "gather")
                {
                    notices++;
                    foreach (string method in new[] { "off", "mandate", "accord", "off-mandate", "off-accord" })
                        TryIntervention(state, commands, plan, method);
                }
                Advance(state, commands);
            }
        }
        Console.WriteLine("SEARCH-COMPLETE campaigns=" + campaigns + " notices=" + notices + " attempts=" + attempts +
            " sufficient=" + sufficient + " examples=" + printed + " refused=" + refused + " checks=" + checks);
        Check(sufficient > 0, "No reachable sufficient cancellation found within this bounded search");
        Console.WriteLine("PASS checks=" + checks + "; cancellation-proof=pure-core-only; outstanding-mandates-disclosed=true");
    }
}
