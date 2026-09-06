using System;
using System.IO;
using PowerAboveAll;
public static class GeographicCombatProbe
{
    public static void Main(string[] args)
    {
        if(args.Length!=1)throw new ArgumentException("Pass one current-schema AutoShots state JSON path.");
        string path=args[0];
        var s=new WorldSimulation(CampaignArchive.Deserialize("{\"Version\":"+CampaignArchive.CurrentVersion+",\"State\":"+File.ReadAllText(path)+"}"));
        s.SetSpeed(WorldSpeed.Normal);
        for(int sec=0;sec<=2400&&s.State.HasCombat;sec+=10)
        {
            s.Advance(10);while(s.State.Clock.PendingMilliseconds>=100)s.Drain();
            if(sec%300==0)foreach(var a in s.State.Armies)foreach(var u in a.Units)
                Console.WriteLine("t={0} {1}/{2} men={3} morale={4:0} cohesion={5:0} fatigue={6:0} ammo={7} withdrawal={8} moving={9} gap={10:0} reason={11}",sec,a.Id,u.Role,u.Men,u.Morale,u.Cohesion,u.Fatigue,u.Ammo,u.Withdrawal,u.Moving,WorldPoint.Distance(u.Position,u.Destination),u.Pressure);
        }
        Console.WriteLine("END active="+s.State.HasCombat+" winner="+s.State.Battles[0].WinnerId+" gameSeconds="+s.State.Clock.Seconds);
    }
}
