using System;
using System.Collections.Generic;
using PowerAboveAll;

public static class WorldBalanceProbe
{
    public static void Main()
    {
        foreach(bool prepared in new[]{false,true})foreach(uint seed in new uint[]{1789,731,9821})
        {
            var home=new WorldSite{Id="paris",RegionId="ile",Position=new WorldPoint(0,0)};
            var enemy=new WorldSite{Id="reims",RegionId="champagne",Position=new WorldPoint(780,0)};
            var s=WorldSimulation.Create(CampaignCore.Create(),new[]{home,enemy},new[]{new WorldRoad{Id="road",From=home.Id,To=enemy.Id,Points=new List<WorldPoint>{home.Position,enemy.Position}}});
            var a=s.State.Army("royal");var b=s.State.Army("resistance");
            a.Posture=WorldPosture.Defend;b.Posture=WorldPosture.Advance;
            int assigned=0,original=b.Men;for(int i=0;i<b.Units.Count;i++){int men=i==5?2400-assigned:(int)(b.Units[i].Original*2400d/original);b.Units[i].Men=b.Units[i].Original=men;assigned+=men;}
            if(prepared)
            {
                b.Fatigue=65;b.AmmunitionWagon=0;b.WagonIntegrity=0;foreach(var u in b.Units)u.Ammo=4;
                s.State.Terrain.Add(new WorldTerrainFeature{Id="ridge",Kind=WorldTerrainKind.Hill,Centre=new WorldPoint(-200,0),Radius=700,Height=30,Source="synthetic balance fixture",Confidence="Not actual French terrain"});
                s.State.Commanders.Find(c=>c.Id==a.CommanderId).Competence=85;
            }
            s.SetSpeed(WorldSpeed.Normal);s.Advance(.1);s.State.Battles[0].RandomState=seed;
            for(int i=0;i<240&&s.State.HasCombat;i++)
            {
                s.Advance(10);while(s.State.Clock.PendingMilliseconds>=100)s.Drain();
                var reserve=a.Units.Find(u=>u.Role==WorldRole.Reserve);
                var gap=a.Units.Find(u=>(u.Role==WorldRole.Left||u.Role==WorldRole.Centre||u.Role==WorldRole.Right)&&u.Withdrawal!=WorldWithdrawal.None);
                if(prepared&&gap!=null&&!reserve.ManualOrder&&reserve.Orders.Count==0)s.OrderUnit(reserve.Id,WorldCommand.Slot(a,gap),WorldIntent.Hold,WorldFormation.Line);
            }
            Console.WriteLine("prepared={0} seed={1} winner={2} active={3} seconds={4:0} royal={5}/1200 enemy={6}/2400",prepared,seed,s.State.Battles[0].WinnerId,s.State.HasCombat,s.State.Clock.Seconds,a.Men,b.Men);
        }
    }
}
