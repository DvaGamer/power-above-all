using System;
using PowerAboveAll;

public static class ArchiveSerializerIntegrationProbe
{
    static void Require(bool value) { if (!value) throw new Exception("Archive integration assertion failed."); }
    static void Reject(string json)
    {
        try { CampaignArchive.Deserialize(json); }
        catch (ArgumentException) { return; }
        throw new Exception("Invalid archive accepted: " + json.Substring(0, Math.Min(80, json.Length)));
    }
    public static void Main()
    {
        foreach (string role in new[] { "legacy", "crown", "assembly", "army" })
        {
            var state = CampaignCore.Create(role);
            string encoded = CampaignArchive.Serialize(state, false);
            Require(encoded.IndexOf("\"Obligation\"", StringComparison.Ordinal) < 0);
            state = CampaignArchive.Deserialize(encoded);
            Require(encoded == CampaignArchive.Serialize(state, false) && state.Obligation == null);
            Reject(encoded.Replace("\"Mandates\":[]", "\"Mandates\":null"));
            Reject(encoded.Replace("\"Mandates\":[]", "\"Ignored\":[]"));
            if (role != "legacy")
            {
                Require(CampaignCore.IssueMandate(state, "ile").Ok);
                state = CampaignArchive.Deserialize(CampaignArchive.Serialize(state));
                Require(CampaignCore.ResolveMandate(state, CampaignCore.MandateId(state.Obligation), "fulfil").Ok);
                state = CampaignArchive.Deserialize(CampaignArchive.Serialize(state));
                Require(state.Obligation == null && state.NextMandateWeek == 4);
            }
            Console.WriteLine(role + ": fresh/open/resolved and missing/null rejection PASS");
        }
        string legacy = CampaignArchive.Serialize(CampaignCore.Create(), false)
            .Replace("\"Version\":2", "\"Version\":1")
            .Replace("\"Mandates\":[],", "")
            .Replace("\"NextMandateWeek\":0,", "")
            .Replace("\"RoleId\":\"legacy\",", "");
        var migrated = CampaignArchive.Deserialize(legacy);
        Require(migrated.RoleId == "legacy" && migrated.Mandates.Count == 0);
        Reject(legacy.Replace("\"Version\":1", "\"Version\":2"));
        Reject("{\"Version\":2,\"State\":null}");
        Reject("{\"Version\":2}");
        Reject("{[]");
        Console.WriteLine("true v1 migration; malformed/missing-state/v2-field rejection PASS");
    }
}
