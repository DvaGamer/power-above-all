using System;
using System.IO;
using PowerAboveAll;

public static class SupplyTimingProbe
{
    public static void Main(string[] args)
    {
        if(args.Length!=1)throw new ArgumentException("Pass a real AutoShots state with the army waiting away from Paris.");
        string json=File.ReadAllText(args[0]);
        foreach(string plan in new[]{"none","early","late"})
        {
            var s=new WorldSimulation(CampaignArchive.Deserialize("{\"Version\":"+CampaignArchive.CurrentVersion+",\"State\":"+json+"}"));
            var a=s.State.Army(s.State.PlayerArmyId);int initial=a.Men;s.SetSpeed(WorldSpeed.Day);
            for(int day=0;day<14;day++)
            {
                if((plan=="early"&&(day==0||day==7))||(plan=="late"&&(day==6||day==12)))
                {
                    var result=WorldSupply.Dispatch(s.Campaign,"royal-depot",a.Id);
                    if(!result.Ok)throw new InvalidOperationException(plan+" dispatch failed: "+result.Key);
                }
                s.Advance(1);while(s.State.Clock.PendingMilliseconds>=100)s.Drain();
                WorldValidation.Validate(s.Campaign);
            }
            Console.WriteLine("{0}: men={1}/{2}; morale={3:0.0}; fatigue={4:0.0}; carriedFood={5:0.00}; centralFood={6}; depotFood={7}; hungryHours={8:0.0}; delivered={9}",
                plan,a.Men,initial,a.Morale,a.Fatigue,a.Rations,s.Campaign.Food,s.State.Depots.Find(d=>d.Id=="royal-depot").Food,a.HungrySeconds/3600,s.State.Convoys.FindAll(c=>c.ArmyId==a.Id&&c.Status==ConvoyStatus.Delivered).Count);
        }
    }
}
