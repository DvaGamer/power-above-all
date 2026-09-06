using System;
using PowerAboveAll;

public static class RegionalAccordArchiveProbe
{
    static void Check(bool value, string label) { if (!value) throw new Exception(label); }
    static void Ok(ActionResult result) { Check(result.Ok, result.Key); }
    static void Reject(string json, string label)
    {
        try { CampaignArchive.Deserialize(json); }
        catch (ArgumentException) { return; }
        throw new Exception("Accepted " + label);
    }
    static string Encode(CampaignState state) { return CampaignArchive.Serialize(state, false); }
    static CampaignState Reload(CampaignState state)
    {
        string before = Encode(state); var loaded = CampaignArchive.Deserialize(before);
        Check(before == Encode(loaded), "round trip changed state"); return loaded;
    }
    public static void Main()
    {
        foreach (string role in new[] { "legacy", "crown", "assembly", "army" })
        {
            var state = Reload(CampaignCore.Create(role));
            var terms = CampaignCore.GetRegionalAccordTerms(state, "champagne");
            Ok(CampaignCore.GrantRegionalAccord(state, "champagne"));
            Check(CampaignCore.Region(state, "champagne").Unrest == 59, "profile");
            Check(CampaignCore.Forecast(state).TaxIncome == terms.ProjectedTaxIncome, "preview");
            state = Reload(state);
            for (int i = 0; i < 4; i++)
            {
                int gold = state.Gold; var forecast = CampaignCore.Forecast(state);
                Ok(CampaignCore.NextWeek(state));
                Check(state.Gold == gold + forecast.NetGold, "changed weekly tax or arrears");
                if (state.PendingPetition) Ok(CampaignCore.ChoosePetition(state, "relief"));
                state = Reload(state);
            }
            Check(!CampaignCore.HasRegionalAccord(state), "did not finish");
            Check(state.Characters.Find(c => c.Id == "morel").Relationship == 54, "completion relationship");
            Console.WriteLine(role + " fresh/active/four actual taxes/complete round trips PASS");
        }
        string fresh = Encode(CampaignCore.Create());
        Reject(fresh.Replace("\"AccordRegionId\":", "\"IgnoredRegion\":"), "missing region");
        Reject(fresh.Replace("\"AccordRegionId\":\"\"", "\"AccordRegionId\":null"), "null region");
        Reject(fresh.Replace("\"AccordUntilWeek\":", "\"IgnoredUntil\":"), "missing until");
        Reject(fresh.Replace("\"AccordUntilWeek\":0", "\"AccordUntilWeek\":null"), "null until");
        Reject(fresh.Replace("\"AccordUntilWeek\":0", "\"AccordUntilWeek\":\"not-a-week\""), "text until");
        string old = fresh.Replace("\"AccordRegionId\":", "\"IgnoredRegion\":").Replace("\"AccordUntilWeek\":", "\"IgnoredUntil\":");
        var v2 = CampaignArchive.Deserialize(old.Replace("\"Version\":3", "\"Version\":2"));
        Check(v2.AccordRegionId == "" && v2.AccordUntilWeek == 0, "v2 migration");
        var v1 = CampaignArchive.Deserialize(old.Replace("\"Version\":3", "\"Version\":1")
            .Replace("\"RoleId\":", "\"IgnoredRole\":").Replace("\"NextMandateWeek\":", "\"IgnoredNext\":").Replace("\"Mandates\":", "\"IgnoredMandates\":"));
        Check(v1.RoleId == "legacy" && v1.AccordRegionId == "", "v1 migration");
        var broken = CampaignCore.Create(); Ok(CampaignCore.GrantRegionalAccord(broken, "champagne"));
        Ok(CampaignCore.Act(broken, "tax", "champagne")); broken = Reload(broken);
        Check(broken.AccordRegionId == "" && broken.AccordUntilWeek == 4, "lost cooldown");
        Check(!CampaignCore.GrantRegionalAccord(broken, "normandy").Ok, "repeated grant");
        Reject(Encode(broken).Replace("\"Version\":3", "\"Version\":2"), "downgraded cooldown");
        Console.WriteLine("Required v3 fields, explicit old-version migrations, early tax/cooldown PASS");
    }
}
