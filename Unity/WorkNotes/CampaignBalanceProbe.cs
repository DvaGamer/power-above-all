using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PowerAboveAll;

public static class CampaignBalanceProbe
{
    sealed class Policy
    {
        public string Name, Role, Petition, Mandate;
        public bool Subsidy, Recruit;
    }
    sealed class RunResult
    {
        public readonly List<string> Checkpoints = new List<string>();
        public string EquivalenceKey;
    }
    static string F(float value) { return value.ToString("0.0", CultureInfo.InvariantCulture); }
    static string Measures(CampaignState state)
    {
        var paris = CampaignCore.Region(state,"ile");
        var line = new StringBuilder();
        line.Append(state.Week).Append('|').Append(state.Gold).Append('|').Append(state.Food).Append('|')
            .Append(state.MilitarySupplies).Append('|').Append(state.Troops).Append('|').Append(state.Manpower)
            .Append('|').Append(F(CampaignCore.AverageUnrest(state))).Append('|').Append(F(state.Power));
        foreach(var faction in state.Factions) line.Append('|').Append(F(faction.Approval));
        line.Append('|').Append(F(paris.Unrest)).Append('|').Append(F(paris.Control)).Append('|').Append(F(paris.EliteLoyalty))
            .Append('|').Append(F(state.Morale)).Append('|').Append(F(state.Supply));
        foreach(var person in state.Characters) line.Append('|').Append(F(person.Relationship));
        return line.ToString();
    }
    static string FullMetrics(CampaignState state)
    {
        var value = new StringBuilder(Measures(state));
        foreach(var region in state.Regions)
            value.Append('|').Append(region.Id).Append(':').Append(region.Unrest.ToString("R",CultureInfo.InvariantCulture))
                .Append(':').Append(region.Control.ToString("R",CultureInfo.InvariantCulture)).Append(':').Append(region.EliteLoyalty.ToString("R",CultureInfo.InvariantCulture));
        foreach(var faction in state.Factions) value.Append('|').Append(faction.Influence).Append(':').Append(faction.Radicalism);
        return value.ToString();
    }
    static string Target(CampaignState state)
    {
        if(state.RoleId=="crown")return "ile";
        if(state.RoleId=="army")return state.ArmyRegionId;
        RegionState highest=state.Regions[0];
        foreach(var region in state.Regions)if(region.Unrest>highest.Unrest)highest=region;
        return highest.Id;
    }
    static void Success(ActionResult action)
    { if(!action.Ok)throw new Exception("Unexpected refusal: "+action.Key); }
    static RunResult Run(Policy policy,bool print)
    {
        var state=CampaignCore.Create(policy.Role);
        var output=new RunResult();var equivalence=new StringBuilder();
        var notes=new List<string>();
        int issued=0,honoured=0,broken=0,fallback=0,recruits=0,recruitRefused=0;
        int foodStress=0,payStress=0,materialStress=0,totalLost=0;
        int firstFood=0,firstPay=0,firstMaterial=0;
        for(;;)
        {
            if(state.PendingPetition)
            {
                string choice=policy.Petition;
                if(choice=="relief"&&state.Food<60){choice="negotiate";notes.Add("week"+state.Week+": relief unaffordable -> negotiate");}
                Success(CampaignCore.ChoosePetition(state,choice));
            }
            if(CampaignCore.MandateDue(state))
            {
                string id=CampaignCore.MandateId(state.Obligation),choice=policy.Mandate=="honour"?"fulfil":"break";
                var check=CampaignCore.CanResolveMandate(state,id,choice);
                if(!check.Ok&&choice=="fulfil")
                {notes.Add("week"+state.Week+": cannot honour "+check.Key+" -> break");choice="break";fallback++;}
                Success(CampaignCore.ResolveMandate(state,id,choice));
                if(choice=="fulfil")honoured++;else broken++;
            }
            CampaignCore.Validate(state);
            equivalence.AppendLine(FullMetrics(state));
            if(state.Week==0||state.Week==4||state.Week==8||state.Week==12||state.Week==16||state.Week==20||state.Week==24)
                output.Checkpoints.Add(Measures(state));
            if(state.Week==24)break;
            if(state.Week==0&&policy.Subsidy)Success(CampaignCore.Act(state,"subsidy","ile"));
            if(policy.Recruit&&state.Week%2==0)
            {
                var recruitment=CampaignCore.Act(state,"recruit",state.ArmyRegionId);
                if(recruitment.Ok)recruits++;
                else {recruitRefused++;notes.Add("week"+state.Week+": recruit refused "+recruitment.Key);}
            }
            if(policy.Mandate!="none"&&state.Obligation==null&&state.Week>=state.NextMandateWeek)
            {
                string target=Target(state);var check=CampaignCore.CanIssueMandate(state,target);
                if(check.Ok){Success(CampaignCore.IssueMandate(state,target));issued++;}
                else notes.Add("week"+state.Week+": mandate refused "+check.Key);
            }
            EconomyForecast forecast=CampaignCore.Forecast(state);
            bool hunger=(long)state.Food+forecast.NetFood<0,unpaid=(long)state.Gold+forecast.NetGold<0;
            int replenishment=(state.Troops>0||state.MilitarySupplies<120)&&!unpaid?18:0;
            int use=(int)Math.Ceiling(state.Troops/120d);
            bool unequipped=(long)state.MilitarySupplies+replenishment<use;
            if(hunger){foodStress++;if(firstFood==0)firstFood=state.Week+1;}
            if(unpaid){payStress++;if(firstPay==0)firstPay=state.Week+1;}
            if(unequipped){materialStress++;if(firstMaterial==0)firstMaterial=state.Week+1;}
            int previousTroops=state.Troops;
            Success(CampaignCore.NextWeek(state));
            if(state.Troops<previousTroops)totalLost+=previousTroops-state.Troops;
        }
        output.EquivalenceKey=equivalence.ToString();
        if(print)
        {
            Console.WriteLine("POLICY "+policy.Name+" role="+policy.Role+" petition="+policy.Petition);
            foreach(string checkpoint in output.Checkpoints)Console.WriteLine(checkpoint);
            Console.WriteLine("actions issued="+issued+" honoured="+honoured+" broken="+broken+" honourFallback="+fallback+" recruits="+recruits+" recruitRefused="+recruitRefused);
            Console.WriteLine("stress food="+foodStress+" first="+firstFood+" pay="+payStress+" first="+firstPay+" materials="+materialStress+" first="+firstMaterial+" troopsLost="+totalLost);
            foreach(string note in notes)Console.WriteLine(note);
        }
        return output;
    }
    public static void Main()
    {
        Console.WriteLine("week|gold|food|supplies|troops|manpower|avgUnrest|power|crown|assembly|urban|army|parisUnrest|parisControl|parisElite|morale|supply|valcourt|morel|lefevre|dumas");
        var baseline=Run(new Policy{Name="no_privilege_relief",Role="legacy",Petition="relief",Mandate="none"},true);
        foreach(string role in new[]{"crown","assembly","army"})
        {
            var parity=Run(new Policy{Name="role_parity",Role=role,Petition="relief",Mandate="none"},false);
            if(parity.EquivalenceKey!=baseline.EquivalenceKey)throw new Exception("No-privilege role changed campaign metrics: "+role);
        }
        Console.WriteLine("NO-PRIVILEGE FOUR-ROLE PARITY: all 25 weekly measured states match");
        Run(new Policy{Name="no_privilege_negotiate",Role="legacy",Petition="negotiate",Mandate="none"},true);
        foreach(string role in new[]{"crown","assembly","army"})
        foreach(string mandate in new[]{"honour","break"})
            Run(new Policy{Name=role+"_"+mandate,Role=role,Petition="relief",Mandate=mandate},true);
        Run(new Policy{Name="paris_subsidy",Role="legacy",Petition="relief",Mandate="none",Subsidy=true},true);
        Run(new Policy{Name="paris_subsidy_recruit",Role="legacy",Petition="relief",Mandate="none",Subsidy=true,Recruit=true},true);
    }
}
