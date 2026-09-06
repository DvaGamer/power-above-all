using System;
using System.Collections.Generic;

namespace PowerAboveAll
{
    // Unity Update yalnız gerçek delta verir. Dünya verisi ve bütün zaman ilerlemesi buradadır.
    public sealed class WorldSimulation
    {
        public CampaignState Campaign { get; }
        public WorldState State => Campaign.World;
        public int LastStepCount { get; private set; }
        public const int FrameWorkBudget=512;
        public WorldSimulation(CampaignState campaign)
        { Campaign=campaign??throw new ArgumentNullException(nameof(campaign));if(campaign.World==null)throw new ArgumentException("Continuous world is missing."); }

        public static WorldSimulation Create(CampaignState campaign,IEnumerable<WorldSite> sites,IEnumerable<WorldRoad> roads)
        {
            var world=new WorldState();world.Sites.AddRange(sites);world.Roads.AddRange(roads);
            var home=world.Sites.Find(s=>s.RegionId==campaign.ArmyRegionId);
            var hostile=world.Sites.Find(s=>s.RegionId=="champagne");
            if(home==null||hostile==null)throw new ArgumentException("Campaign seats are missing.");
            // Eski kayıt yeni dünya için otomatik olarak tahmin edilmez; bu yalnız yeni kampanya kurar.
            if(campaign.Week!=0)throw new ArgumentException("Create a new campaign for continuous time.");
            AddArmy(world,"royal","world.army.royal","crown",home,campaign.Troops,campaign.Morale,"dumas",78);
            var resistance=CampaignCore.GetRegionalResistance(campaign,"champagne");
            AddArmy(world,"resistance","world.army.resistance","insurgent",hostile,Math.Max(600,resistance?.EnemyTroops??900),77,"local-command",55);
            campaign.World=world;campaign.Moves=0;
            WorldSupply.Initialize(campaign);
            var simulation=new WorldSimulation(campaign);simulation.ExportPlayerArmy();return simulation;
        }
        private static void AddArmy(WorldState world,string id,string name,string faction,WorldSite seat,int strength,float morale,string person,float skill)
        {
            var army=new WorldArmy{Id=id,NameKey=name,FactionId=faction,Position=seat.Position,RegionId=seat.RegionId,CommanderId=id+"-command",HeadquartersId=id+"-hq"};
            world.Commanders.Add(new WorldCommander{Id=army.CommanderId,CharacterId=person,Competence=skill});
            world.Headquarters.Add(new WorldHeadquarters{Id=army.HeadquartersId,CommanderId=army.CommanderId,Position=army.Position,Destination=army.Position});
            army.Posture=id=="royal"?WorldPosture.Advance:WorldPosture.Defend;
            army.WagonPosition=army.Position;
            int assigned=0;double[] shares={.24,.20,.16,.16,.12,.12};
            WorldUnitKind[] kinds={WorldUnitKind.Infantry,WorldUnitKind.Infantry,WorldUnitKind.Infantry,WorldUnitKind.Militia,WorldUnitKind.Cavalry,WorldUnitKind.Artillery};
            for(int i=0;i<6;i++)
            {
                int men=i==5?strength-assigned:(int)(strength*shares[i]);assigned+=men;
                var position=seat.Position+new WorldPoint((i-1.5)*90,id=="royal"?-90:90);
                army.Units.Add(new WorldUnit{Id=id+"-"+i,Kind=kinds[i],Role=i==4?WorldRole.Screen:i==5?WorldRole.Battery:(WorldRole)i,Men=men,Original=men,Morale=morale,Position=position,Destination=position,Experience=i==3?15:45,Formation=WorldFormation.Column,Facing=id=="royal"?90:270});
            }
            world.Armies.Add(army);
        }
        public void SetSpeed(WorldSpeed speed)
        {
            WorldClock.Rate(speed);
            State.Clock.Speed=speed;
            // Oyuncu duraklatınca eski yüksek hız kuyruğu bir sonraki oynatmada zıplamaz.
            if(speed==WorldSpeed.Pause){State.Clock.PendingMilliseconds=0;State.Clock.FractionalMilliseconds=0;}
            if(State.HasCombat&&speed>WorldSpeed.Normal)State.Clock.Speed=WorldSpeed.Normal;
        }
        public ActionResult March(string armyId,string siteId)
        {
            var army=State.Army(armyId);
            if(army==null||army.Id!=State.PlayerArmyId||army.Men==0||army.Activity==ArmyActivity.Fighting||army.Activity==ArmyActivity.Retreating)
                return Result(false,"world.order_unavailable");
            var route=WorldRouting.Find(State,army,siteId);
            if(route==null||route.Points.Count<2)return Result(false,"world.route_unavailable");
            army.Route=route;army.Activity=ArmyActivity.Marching;army.OrderIssuedAt=State.Clock.Milliseconds;
            army.OrderReceivedAt=State.Clock.Milliseconds+3000;
            State.LastNoticeKey="world.march_sent";State.LastNoticeRegion=route.DestinationRegionId;
            return Result(true,"world.march_sent");
        }
        public ActionResult OrderUnit(string unitId,WorldPoint destination,WorldIntent intent,WorldFormation formation)
        {
            var army=State.Army(State.PlayerArmyId);var unit=army.Units.Find(u=>u.Id==unitId);
            if(unit==null||!WorldCombat.Fighting(unit)||army.Activity!=ArmyActivity.Fighting||!Enum.IsDefined(typeof(WorldIntent),intent)||!Enum.IsDefined(typeof(WorldFormation),formation)||!Finite(destination))return Result(false,"world.order_unavailable");
            if(WorldPoint.Distance(army.Position,destination)>8000)return Result(false,"world.order_too_far");
            if(formation==WorldFormation.Square&&(unit.Kind==WorldUnitKind.Cavalry||unit.Kind==WorldUnitKind.Artillery))return Result(false,"battle.square_infantry");
            if(unit.Orders.Count>0)
            {
                var last=unit.Orders[unit.Orders.Count-1];
                if(last.Intent==intent&&last.Formation==formation&&WorldPoint.Distance(last.Destination,destination)<1)return Result(true,"battle.command.sent");
            }
            if(unit.Orders.Count>=2)return Result(false,"battle.command.queue_full");
            WorldCommand.Queue(State,army,unit,destination,intent,formation);
            return Result(true,"battle.command.sent");
        }
        public ActionResult MoveHeadquarters(WorldPoint destination)
        {
            var army=State.Army(State.PlayerArmyId);var hq=State.Headquarters.Find(h=>h.Id==army.HeadquartersId);
            if(army.Activity!=ArmyActivity.Fighting||hq.Integrity<=0||!Finite(destination)||WorldPoint.Distance(army.Position,destination)>8000)return Result(false,"world.order_unavailable");
            hq.Destination=destination;hq.OrderReceivedAt=State.Clock.Milliseconds+1500;hq.Moving=true;
            return Result(true,"battle.command.sent");
        }
        public void Retreat(string armyId)
        {
            var battle=State.Battles.Find(b=>!b.Ended&&(b.FirstArmyId==armyId||b.SecondArmyId==armyId));
            if(battle==null||armyId!=State.PlayerArmyId)return;
            WorldCombat.End(State,battle,battle.FirstArmyId==armyId?battle.SecondArmyId:battle.FirstArmyId);
            ResolveConsequences(battle);ExportPlayerArmy();
        }
        public void Advance(double realSeconds)
        {
            State.Clock.Accumulate(realSeconds);Drain();
        }
        public void Drain()
        {
            var clock=State.Clock;LastStepCount=0;
            if(clock.Speed==WorldSpeed.Pause)return;
            while(clock.PendingMilliseconds>=WorldCombat.TickMilliseconds&&LastStepCount<FrameWorkBudget)
            {
                bool fighting=State.HasCombat;
                long step=fighting?WorldCombat.TickMilliseconds:Math.Min(clock.PendingMilliseconds,900000);
                if(fighting)foreach(var battle in State.Battles)if(!battle.Ended)step=Math.Min(step,battle.NextTickAt-clock.Milliseconds);
                step=Math.Min(step,Math.Min(State.NextConditionAt,Math.Min(State.NextDayAt,State.NextEconomyAt))-clock.Milliseconds);
                if(!fighting)
                {
                    foreach(var army in State.Armies)
                    {
                        if(army.OrderReceivedAt>clock.Milliseconds)step=Math.Min(step,army.OrderReceivedAt-clock.Milliseconds);
                        if(Mobile(army)&&army.OrderReceivedAt<=clock.Milliseconds&&army.Route.Segment+1<army.Route.Points.Count)
                        {
                            double remaining=WorldPoint.Distance(army.Position,army.Route.Points[army.Route.Segment+1]);
                            if(Speed(army)>0)step=Math.Min(step,Math.Max(1,(long)Math.Ceiling(remaining/Speed(army)*1000)));
                        }
                    }
                    step=Math.Min(step,ContactBoundary(step));
                }
                if(step<=0)throw new InvalidOperationException("World scheduler did not advance.");
                clock.Milliseconds+=step;clock.PendingMilliseconds-=step;LastStepCount++;
                var armyStart=State.Convoys.Exists(WorldSupply.Active)?State.Armies.ConvertAll(a=>a.Position):null;
                MoveArmies(step/1000d);
                WorldSupply.Step(State,step/1000d,armyStart);
                if(fighting)
                {
                    foreach(var battle in State.Battles)
                        if(!battle.Ended&&clock.Milliseconds>=battle.NextTickAt){WorldCombat.Step(State,battle);battle.NextTickAt+=WorldCombat.TickMilliseconds;if(battle.Ended)ResolveConsequences(battle);}
                }
                ExportPlayerArmy();
                if(clock.Milliseconds>=State.NextEconomyAt)
                {
                    CampaignCore.SettleCalendarPeriod(Campaign);ImportPlayerArmy();State.NextEconomyAt+=WorldClock.Week;
                }
                if(clock.Milliseconds>=State.NextDayAt)
                {CampaignCore.ProcessCorrespondenceDay(Campaign,(int)(clock.Milliseconds/WorldClock.Day));State.NextDayAt+=WorldClock.Day;}
                if(clock.Milliseconds>=State.NextConditionAt)
                {AdvanceCondition();WorldSupply.QuarterHour(State);State.NextConditionAt+=900000;}
                if(FindContacts())
                {
                    clock.Speed=State.BattlePolicy==BattleTimePolicy.Pause?WorldSpeed.Pause:WorldSpeed.Normal;
                    clock.PendingMilliseconds=0;clock.FractionalMilliseconds=0;break;
                }
            }
            WorldCommand.Observe(State);
            ExportPlayerArmy();
        }
        private bool Mobile(WorldArmy army) => army.Men>0&&(army.Activity==ArmyActivity.Marching||army.Activity==ArmyActivity.Retreating);
        private double Speed(WorldArmy army) => army.MovementSpeed*(1-army.Fatigue*.0045)*(army.Supply<20?.65:1)*WorldRouting.RoadSpeed(State,army.Route);
        private WorldPoint Velocity(WorldArmy army)
        {
            if(!Mobile(army)||army.OrderReceivedAt>State.Clock.Milliseconds||army.Route.Segment+1>=army.Route.Points.Count)return new WorldPoint();
            var delta=army.Route.Points[army.Route.Segment+1]-army.Position;double distance=WorldPoint.Distance(army.Position,army.Route.Points[army.Route.Segment+1]);
            return distance<.001?new WorldPoint():delta*(Speed(army)/distance);
        }
        private bool CanContact(WorldArmy a,WorldArmy b) => a.FactionId!=b.FactionId&&a.Men>0&&b.Men>0&&a.Activity!=ArmyActivity.Fighting&&b.Activity!=ArmyActivity.Fighting&&a.RecoverUntil<=State.Clock.Milliseconds&&b.RecoverUntil<=State.Clock.Milliseconds;
        private long ContactBoundary(long maximum)
        {
            long result=maximum;
            for(int i=0;i<State.Armies.Count;i++)for(int j=i+1;j<State.Armies.Count;j++)
            {
                var a=State.Armies[i];var b=State.Armies[j];if(!CanContact(a,b))continue;
                var p=a.Position-b.Position;var v=Velocity(a)-Velocity(b);
                double c=p.X*p.X+p.Z*p.Z-WorldCombat.ContactMetres*WorldCombat.ContactMetres;
                if(c<=0){result=1;continue;}
                double aa=v.X*v.X+v.Z*v.Z,bb=2*(p.X*v.X+p.Z*v.Z),discriminant=bb*bb-4*aa*c;
                if(aa<1e-12||bb>=0||discriminant<0)continue;
                double seconds=(-bb-Math.Sqrt(discriminant))/(2*aa);
                if(seconds>=0)result=Math.Min(result,Math.Max(1,(long)Math.Ceiling(seconds*1000)));
            }
            return result;
        }
        private void MoveArmies(double seconds)
        {
            foreach(var army in State.Armies)
            {
                if(!Mobile(army)||army.OrderReceivedAt>=State.Clock.Milliseconds)continue;
                bool blocked=WorldRouting.RoadSpeed(State,army.Route)<=0;
                if(blocked){army.Activity=ArmyActivity.Holding;State.LastNoticeKey="world.road_blocked";continue;}
                var before=army.Position;double remaining=Speed(army)*seconds;
                while(remaining>0&&army.Route.Segment+1<army.Route.Points.Count)
                {
                    var target=army.Route.Points[army.Route.Segment+1];double distance=WorldPoint.Distance(army.Position,target);
                    if(distance>remaining){army.Position=WorldPoint.Lerp(army.Position,target,remaining/distance);army.Route.TravelledMetres+=remaining;remaining=0;}
                    else{army.Position=target;remaining-=distance;army.Route.TravelledMetres+=distance;army.Route.Segment++;}
                }
                WorldPoint moved=army.Position-before;
                foreach(var unit in army.Units){unit.Position+=moved;unit.Destination=unit.Position;unit.Moving=false;if(WorldPoint.Distance(before,army.Position)>.001)unit.Facing=WorldCombat.Heading(moved);}
                var hq=State.Headquarters.Find(h=>h.Id==army.HeadquartersId);hq.Position+=moved;hq.Destination=hq.Position;
                army.WagonPosition+=moved;
                if(army.Route.Segment>=army.Route.Points.Count-1)
                {
                    if(army.Activity==ArmyActivity.Marching){army.RegionId=army.Route.DestinationRegionId;State.LastNoticeRegion=army.RegionId;State.LastNoticeKey="world.arrived";}
                    army.Activity=army.Activity==ArmyActivity.Retreating?ArmyActivity.Recovering:ArmyActivity.Holding;
                }
            }
        }
        private bool FindContacts()
        {
            bool started=false;
            for(int i=0;i<State.Armies.Count;i++)for(int j=i+1;j<State.Armies.Count;j++)
            {
                var a=State.Armies[i];var b=State.Armies[j];
                if(!CanContact(a,b)||WorldPoint.Distance(a.Position,b.Position)>WorldCombat.ContactMetres+.01)continue;
                var battle=new WorldBattle{Id="world-battle-"+State.NextBattleId++,FirstArmyId=a.Id,SecondArmyId=b.Id,StartedAt=State.Clock.Milliseconds,NextTickAt=State.Clock.Milliseconds+WorldCombat.TickMilliseconds,Contact=WorldPoint.Lerp(a.Position,b.Position,.5),RegionId=b.RegionId,FirstOriginal=a.Men,SecondOriginal=b.Men};
                State.Battles.Add(battle);a.Activity=b.Activity=ArmyActivity.Fighting;
                // Birimler oldukları yerde kalır; görev yerlerine yürüyerek açılır.
                WorldCommand.Deploy(State,a,b);WorldCommand.Deploy(State,b,a);
                State.LastNoticeKey="world.contact";State.LastNoticeRegion=battle.RegionId;started=true;
            }
            return started;
        }
        private void AdvanceCondition()
        {
            foreach(var army in State.Armies)
            {
                if(army.Activity==ArmyActivity.Fighting)continue;
                army.Fatigue=Math.Max(0,Math.Min(100,army.Fatigue+(Mobile(army)?.18f:army.Rations<=0?0:-.5f)));
                if(army.Activity==ArmyActivity.Recovering&&army.RecoverUntil<=State.Clock.Milliseconds)
                {
                    army.Activity=ArmyActivity.Holding;
                    foreach(var unit in army.Units){unit.Routed=false;unit.Withdrawal=WorldWithdrawal.None;unit.Morale=Math.Max(30,unit.Morale);unit.Intent=WorldIntent.Hold;}
                }
            }
        }
        private void ResolveConsequences(WorldBattle battle)
        {
            bool won=battle.WinnerId==State.PlayerArmyId;
            var region=CampaignCore.Region(Campaign,battle.RegionId);
            if(region!=null){region.Unrest=Math.Max(0,Math.Min(100,region.Unrest+(won?-22:5)));region.Control=Math.Min(100,region.Control+(won?12:0));}
            Campaign.Power=Math.Max(0,Math.Min(100,Campaign.Power+(won?4:-6)));
            var general=Campaign.Characters.Find(c=>c.Id=="dumas");general.Relationship=Math.Max(0,Math.Min(100,general.Relationship+(won?2:-4)));if(won)general.Ambition=Math.Min(100,general.Ambition+3);
        }
        public void ExportPlayerArmy()
        {
            var army=State.Army(State.PlayerArmyId);Campaign.Troops=army.Men;Campaign.Morale=army.Morale;Campaign.Supply=army.Supply;Campaign.Fatigue=army.Fatigue;Campaign.ArmyRegionId=army.RegionId;Campaign.Moves=0;
        }
        // Geçişte eski ülke işlemlerinin gerçek alay listesine uygulandığı tek köprü.
        public void ImportPlayerArmy()
        {
            var army=State.Army(State.PlayerArmyId);int difference=Campaign.Troops-army.Men;
            if(difference>0){army.Units[0].Men+=difference;army.Units[0].Original+=difference;}
            else for(int i=army.Units.Count-1;i>=0&&difference<0;i--){int loss=Math.Min(army.Units[i].Men,-difference);army.Units[i].Men-=loss;difference+=loss;}
            float moraleChange=Campaign.Morale-army.Morale;
            float fatigueChange=Campaign.Fatigue-army.Fatigue;
            foreach(var unit in army.Units){unit.Morale=Math.Max(0,Math.Min(100,unit.Morale+moraleChange));unit.Fatigue=Math.Max(0,Math.Min(100,unit.Fatigue+fatigueChange));}
            army.Supply=Campaign.Supply;army.Fatigue=Campaign.Fatigue;
        }
        private static ActionResult Result(bool ok,string key)=>new ActionResult{Ok=ok,Key=key};
        internal static bool Finite(WorldPoint p)=>!double.IsNaN(p.X)&&!double.IsInfinity(p.X)&&!double.IsNaN(p.Z)&&!double.IsInfinity(p.Z)&&Math.Abs(p.X)<21000000&&Math.Abs(p.Z)<11000000;
    }
}
