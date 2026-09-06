using System;
using System.Collections.Generic;

namespace PowerAboveAll
{
    public static class WorldValidation
    {
        private static void Check(bool condition,[System.Runtime.CompilerServices.CallerLineNumber] int line=0){if(!condition)throw new ArgumentException("Invalid continuous world archive (rule "+line+").");}
        private static bool Percent(float value)=>!float.IsNaN(value)&&!float.IsInfinity(value)&&value>=0&&value<=100;
        public static void Validate(CampaignState campaign)
        {
            Check(campaign.Worlds!=null&&campaign.Worlds.Count<=1&&(campaign.Worlds.Count==0||campaign.Worlds[0]!=null));
            var w=campaign.World;if(w==null)return;
            Check(w.Schema==3&&w.Clock!=null&&w.NextBattleId>0&&w.NextConvoyId>0);
            var clock=w.Clock;
            Check(clock.Milliseconds>=0&&clock.Milliseconds<WorldClock.Week*400000L&&clock.PendingMilliseconds>=0&&clock.PendingMilliseconds<=WorldClock.Day*400000L);
            Check(!double.IsNaN(clock.FractionalMilliseconds)&&clock.FractionalMilliseconds>=0&&clock.FractionalMilliseconds<1);
            Check(Enum.IsDefined(typeof(WorldSpeed),clock.Speed)&&Enum.IsDefined(typeof(BattleTimePolicy),w.BattlePolicy));
            Check(campaign.Week==clock.Milliseconds/WorldClock.Week);
            Check(w.NextDayAt==(clock.Milliseconds/WorldClock.Day+1)*WorldClock.Day);
            Check(w.NextEconomyAt==(clock.Milliseconds/WorldClock.Week+1)*WorldClock.Week);
            Check(w.NextConditionAt==(clock.Milliseconds/900000+1)*900000);
            Check(w.Sites!=null&&w.Sites.Count>0&&w.Sites.Count<=100000&&w.Roads!=null&&w.Roads.Count<=200000);
            var sites=new HashSet<string>();
            foreach(var s in w.Sites)Check(s!=null&&!string.IsNullOrEmpty(s.Id)&&sites.Add(s.Id)&&WorldSimulation.Finite(s.Position));
            var roads=new HashSet<string>();
            foreach(var r in w.Roads)
            {
                Check(r!=null&&!string.IsNullOrEmpty(r.Id)&&roads.Add(r.Id)&&sites.Contains(r.From)&&sites.Contains(r.To)&&r.From!=r.To&&r.Points!=null&&r.Points.Count>=2&&r.Points.Count<100000&&r.SpeedFactor>0&&r.SpeedFactor<=5);
                foreach(var p in r.Points)Check(WorldSimulation.Finite(p));
                Check(WorldPoint.Distance(r.Points[0],w.Sites.Find(s=>s.Id==r.From).Position)<10&&WorldPoint.Distance(r.Points[r.Points.Count-1],w.Sites.Find(s=>s.Id==r.To).Position)<10);
            }
            Check(w.Commanders!=null&&w.Headquarters!=null&&w.Armies!=null&&w.Armies.Count>0&&w.Armies.Count<=1000&&w.Battles!=null&&w.Battles.Count<100000);
            var command=new HashSet<string>();var hqs=new HashSet<string>();var armies=new HashSet<string>();var units=new HashSet<string>();
            foreach(var c in w.Commanders)Check(c!=null&&!string.IsNullOrEmpty(c.Id)&&command.Add(c.Id)&&Percent(c.Competence)&&Percent(c.Loyalty)&&Percent(c.Ambition));
            foreach(var h in w.Headquarters)Check(h!=null&&!string.IsNullOrEmpty(h.Id)&&hqs.Add(h.Id)&&command.Contains(h.CommanderId)&&WorldSimulation.Finite(h.Position)&&WorldSimulation.Finite(h.Destination)&&Percent(h.Integrity)&&h.OrderReceivedAt>=0&&h.OrderReceivedAt<=clock.Milliseconds+6000);
            foreach(var a in w.Armies)
            {
                Check(a!=null&&!string.IsNullOrEmpty(a.Id)&&armies.Add(a.Id)&&!string.IsNullOrEmpty(a.FactionId)&&command.Contains(a.CommanderId)&&hqs.Contains(a.HeadquartersId)&&Array.Exists(CampaignCore.Regions,r=>r.Id==a.RegionId));
                Check(WorldSimulation.Finite(a.Position)&&Percent(a.Supply)&&Percent(a.Fatigue)&&a.MovementSpeed>0&&a.MovementSpeed<=10&&Enum.IsDefined(typeof(ArmyActivity),a.Activity));
                Check(Enum.IsDefined(typeof(WorldPosture),a.Posture)&&WorldSimulation.Finite(a.FrontAnchor)&&WorldSimulation.Finite(a.Forward)&&WorldSimulation.Finite(a.RetreatPoint)&&WorldSimulation.Finite(a.WagonPosition)&&Percent(a.WagonIntegrity)&&a.AmmunitionWagon>=0&&a.AmmunitionWagon<=1000000);
                Check(FiniteTime(a.ReserveDecisionAt)&&FiniteTime(a.DisruptionSeconds));
                Check(FiniteTime(a.Rations)&&a.Rations<=100000000&&FiniteTime(a.HungrySeconds));
                Check(a.OrderIssuedAt>=0&&a.OrderIssuedAt<=clock.Milliseconds&&a.OrderReceivedAt>=a.OrderIssuedAt&&a.OrderReceivedAt<=a.OrderIssuedAt+WorldClock.Day);
                Check(a.Route!=null&&a.Route.Points!=null&&a.Route.RoadIds!=null&&a.Route.Segment>=0&&(a.Route.Points.Count==0?a.Route.Segment==0:a.Route.Segment<a.Route.Points.Count));
                Check(!double.IsNaN(a.Route.TravelledMetres)&&!double.IsInfinity(a.Route.TravelledMetres)&&a.Route.TravelledMetres>=0);
                foreach(var point in a.Route.Points)Check(WorldSimulation.Finite(point));foreach(var road in a.Route.RoadIds)Check(roads.Contains(road));
                Check(a.Route.SegmentRoadIds!=null&&(a.Route.SegmentRoadIds.Count==0||a.Route.SegmentRoadIds.Count==a.Route.Points.Count-1));
                foreach(var road in a.Route.SegmentRoadIds)Check(road==""||roads.Contains(road));
                Check(a.Units!=null&&a.Units.Count>0&&a.Units.Count<=1000);
                foreach(var u in a.Units)
                {
                    Check(u!=null&&!string.IsNullOrEmpty(u.Id)&&units.Add(u.Id)&&u.Original>=0&&u.Men>=0&&u.Men<=u.Original&&u.Original<=100000000&&u.Ammo>=0&&u.Ammo<=1000&&u.Captured>=0&&u.Captured<=u.Original-u.Men);
                    Check(Percent(u.Morale)&&Percent(u.Cohesion)&&Percent(u.Fatigue)&&Percent(u.Experience)&&WorldSimulation.Finite(u.Position)&&WorldSimulation.Finite(u.Destination));
                    Check(Enum.IsDefined(typeof(WorldUnitKind),u.Kind)&&Enum.IsDefined(typeof(WorldFormation),u.Formation)&&Enum.IsDefined(typeof(WorldIntent),u.Intent));
                    Check(WorldSimulation.Finite(u.ResumeDestination));
                    Check(Enum.IsDefined(typeof(WorldRole),u.Role)&&Enum.IsDefined(typeof(WorldWithdrawal),u.Withdrawal)&&((int)u.Pressure&~255)==0&&WorldSimulation.Finite(u.AssignedPosition)&&FiniteTime(u.ReorganizeUntil)&&FiniteTime(u.WithdrawalSeconds)&&FiniteTime(u.Quiet)&&!double.IsNaN(u.LastFiredAt)&&!double.IsInfinity(u.LastFiredAt));
                    Check(!double.IsNaN(u.Facing)&&!double.IsInfinity(u.Facing)&&!double.IsNaN(u.Reload)&&u.Reload>=0&&u.Reload<1000&&u.Orders!=null&&u.Orders.Count<=2);
                    long last=0;
                    foreach(var o in u.Orders){Check(o!=null&&WorldSimulation.Finite(o.Destination)&&o.IssuedAt>=0&&o.IssuedAt<=clock.Milliseconds&&o.ReceivedAt>=o.IssuedAt&&o.ReceivedAt>last&&o.ReceivedAt-o.IssuedAt<=6000&&Enum.IsDefined(typeof(WorldFormation),o.Formation)&&Enum.IsDefined(typeof(WorldIntent),o.Intent));last=o.ReceivedAt;}
                }
            }
            Check(armies.Contains(w.PlayerArmyId)&&armies.Contains(w.SelectedArmyId)&&w.SelectedUnitId!=null&&(w.SelectedUnitId==""||units.Contains(w.SelectedUnitId)));
            var battles=new HashSet<string>();var active=new HashSet<string>();
            foreach(var b in w.Battles)
            {
                Check(b!=null&&!string.IsNullOrEmpty(b.Id)&&battles.Add(b.Id)&&armies.Contains(b.FirstArmyId)&&armies.Contains(b.SecondArmyId)&&b.FirstArmyId!=b.SecondArmyId&&WorldSimulation.Finite(b.Contact)&&b.StartedAt>=0&&b.StartedAt<=clock.Milliseconds&&b.RandomState!=0&&b.FirstOriginal>0&&b.SecondOriginal>0);
                if(b.Ended)Check(b.EndedAt>=b.StartedAt&&b.EndedAt<=clock.Milliseconds&&(b.WinnerId==""||b.WinnerId==b.FirstArmyId||b.WinnerId==b.SecondArmyId));
                else Check(active.Add(b.FirstArmyId)&&active.Add(b.SecondArmyId)&&b.NextTickAt>clock.Milliseconds&&b.NextTickAt<=clock.Milliseconds+WorldCombat.TickMilliseconds);
            }
            foreach(var a in w.Armies)Check((a.Activity==ArmyActivity.Fighting)==active.Contains(a.Id));
            Check(w.Terrain!=null&&w.Terrain.Count<=20000&&w.Sightings!=null&&w.Sightings.Count<=w.Armies.Count);
            var terrain=new HashSet<string>();
            foreach(var f in w.Terrain)
            {
                Check(f!=null&&!string.IsNullOrEmpty(f.Id)&&terrain.Add(f.Id)&&!string.IsNullOrEmpty(f.Source)&&!string.IsNullOrEmpty(f.Confidence)&&Enum.IsDefined(typeof(WorldTerrainKind),f.Kind)&&WorldSimulation.Finite(f.Centre)&&f.Radius>0&&f.Radius<100000&&!double.IsNaN(f.Height)&&f.Height>=0&&f.Height<9000&&f.Points!=null&&f.Points.Count<100000);
                foreach(var p in f.Points)Check(WorldSimulation.Finite(p));if(f.Kind==WorldTerrainKind.River)Check(f.Points.Count>=2);
            }
            var seen=new HashSet<string>();
            foreach(var s in w.Sightings)Check(s!=null&&armies.Contains(s.ArmyId)&&seen.Add(s.ArmyId)&&WorldSimulation.Finite(s.Position)&&s.ObservedAt>=0&&s.ObservedAt<=clock.Milliseconds&&s.Minimum>=0&&s.Maximum>=s.Minimum&&s.Maximum<=2000000000);
            Check(w.Depots!=null&&w.Depots.Count<=100000&&w.Convoys!=null&&w.Convoys.Count<=100000);
            var depots=new HashSet<string>();var convoys=new HashSet<string>();
            foreach(var d in w.Depots)Check(d!=null&&!string.IsNullOrEmpty(d.Id)&&depots.Add(d.Id)&&sites.Contains(d.SiteId)&&!string.IsNullOrEmpty(d.FactionId)&&d.Food>=0&&d.Food<=1000000&&d.Ammunition>=0&&d.Ammunition<=1000000);
            foreach(var c in w.Convoys)
            {
                Check(c!=null&&!string.IsNullOrEmpty(c.Id)&&convoys.Add(c.Id)&&depots.Contains(c.DepotId)&&armies.Contains(c.ArmyId)&&!string.IsNullOrEmpty(c.FactionId)&&WorldSimulation.Finite(c.Position)&&WorldSimulation.Finite(c.Rendezvous));
                Check(Enum.IsDefined(typeof(ConvoyStatus),c.Status)&&c.Food>=0&&c.Food<=WorldSupply.FoodLoad&&c.Ammunition>=0&&c.Ammunition<=WorldSupply.AmmunitionLoad&&c.DispatchedAt>=0&&c.DispatchedAt<=clock.Milliseconds&&c.CompletedAt>=0&&c.CompletedAt<=clock.Milliseconds);
                Check(WorldSupply.Active(c)||c.Food+c.Ammunition==0);
                Check(c.Route!=null&&c.Route.Points!=null&&c.Route.Points.Count>0&&c.Route.Segment>=0&&c.Route.Segment<c.Route.Points.Count&&c.Route.SegmentRoadIds!=null&&c.Route.SegmentRoadIds.Count==c.Route.Points.Count-1&&c.Route.RoadIds!=null&&FiniteTime(c.Route.TravelledMetres));
                foreach(var p in c.Route.Points)Check(WorldSimulation.Finite(p));foreach(var road in c.Route.SegmentRoadIds)Check(road==""||roads.Contains(road));foreach(var road in c.Route.RoadIds)Check(roads.Contains(road));
            }
        }
        private static bool FiniteTime(double value)=>!double.IsNaN(value)&&!double.IsInfinity(value)&&value>=0&&value<1e15;
    }
}
