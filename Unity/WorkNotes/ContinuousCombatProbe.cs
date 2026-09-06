using System;
using System.Collections.Generic;
using PowerAboveAll;

public static class ContinuousCombatProbe
{
    public static void Main()
    {
        var a=new WorldSite{Id="paris",RegionId="ile",Position=new WorldPoint(0,0)};
        var b=new WorldSite{Id="reims",RegionId="champagne",Position=new WorldPoint(10000,0)};
        var s=WorldSimulation.Create(CampaignCore.Create(),new[]{a,b},new[]{new WorldRoad{Id="road",From=a.Id,To=b.Id,Points=new List<WorldPoint>{a.Position,b.Position}}});
        s.March("royal","reims");s.SetSpeed(WorldSpeed.Day);s.Advance(1);
        for(int second=0;second<=1800&&s.State.HasCombat;second+=10)
        {
            s.Advance(10);while(s.State.Clock.PendingMilliseconds>=100)s.Drain();
            if(second%300==0)
            {
                Console.WriteLine("AFTER "+second+" seconds");
                foreach(var army in s.State.Armies)foreach(var u in army.Units)
                    Console.WriteLine("{0}/{1} men={2} morale={3:0} cohesion={4:0} fatigue={5:0} ammo={6} withdrawal={7} moving={8} pos={9:0},{10:0} goal={11:0},{12:0} reason={13}",army.Id,u.Role,u.Men,u.Morale,u.Cohesion,u.Fatigue,u.Ammo,u.Withdrawal,u.Moving,u.Position.X,u.Position.Z,u.Destination.X,u.Destination.Z,u.Pressure);
            }
        }
        Console.WriteLine("END active="+s.State.HasCombat+" winner="+s.State.Battles[0].WinnerId);
    }
}
