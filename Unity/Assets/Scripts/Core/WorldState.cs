using System;
using System.Collections.Generic;

namespace PowerAboveAll
{
    public enum WorldSpeed { Pause, Normal, Hour, Day }
    public enum BattleTimePolicy { SlowToNormal, Pause }
    public enum ArmyActivity { Holding, Marching, Fighting, Retreating, Recovering }
    public enum WorldUnitKind { Infantry, Militia, Cavalry, Artillery }
    public enum WorldFormation { Line, Column, Square }
    public enum WorldIntent { Advance, Hold, Reserve, Withdraw }
    public enum WorldRole { Centre, Left, Right, Reserve, Battery, Screen }
    public enum WorldPosture { Advance, Defend }
    public enum WorldWithdrawal { None, Ordered, Disordered, Rout, Surrendered }
    [Flags] public enum WorldPressure { None=0, Flanked=1, Isolated=2, RearBlocked=4, HeadquartersLost=8, Ammunition=16, Exhausted=32, Disordered=64, Obstructed=128 }
    public enum WorldTerrainKind { Woodland, Town, River, Hill }
    [Serializable] public sealed class WorldTerrainFeature
    {
        public string Id, Source, Confidence;
        public WorldTerrainKind Kind;
        public WorldPoint Centre;
        public double Radius, Height;
        public List<WorldPoint> Points=new List<WorldPoint>();
    }
    [Serializable] public sealed class WorldSighting
    {
        public string ArmyId;
        public WorldPoint Position;
        public long ObservedAt;
        public int Minimum, Maximum;
        public bool Visible;
    }

    [Serializable] public struct WorldPoint
    {
        public double X, Z;
        public WorldPoint(double x, double z) { X=x; Z=z; }
        public const double MetresPerAtlasUnit = 11119.50802335329;
        public static WorldPoint FromGeographic(double longitude,double latitude) =>
            new WorldPoint(longitude*6.9465837*MetresPerAtlasUnit,latitude*10*MetresPerAtlasUnit);
        public static double Distance(WorldPoint a,WorldPoint b) => Math.Sqrt((a.X-b.X)*(a.X-b.X)+(a.Z-b.Z)*(a.Z-b.Z));
        public static WorldPoint Lerp(WorldPoint a,WorldPoint b,double t) => new WorldPoint(a.X+(b.X-a.X)*t,a.Z+(b.Z-a.Z)*t);
        public static WorldPoint operator +(WorldPoint a,WorldPoint b) => new WorldPoint(a.X+b.X,a.Z+b.Z);
        public static WorldPoint operator -(WorldPoint a,WorldPoint b) => new WorldPoint(a.X-b.X,a.Z-b.Z);
        public static WorldPoint operator *(WorldPoint a,double n) => new WorldPoint(a.X*n,a.Z*n);
    }
    [Serializable] public sealed class WorldClock
    {
        public const long Second=1000, Hour=3600000, Day=86400000, Week=604800000;
        public long Milliseconds, PendingMilliseconds;
        public double FractionalMilliseconds;
        public WorldSpeed Speed=WorldSpeed.Pause;
        public double Seconds => Milliseconds/1000d;
        public DateTime Date => new DateTime(1789,5,5).AddMilliseconds(Milliseconds);
        public static int Rate(WorldSpeed speed)
        {
            switch(speed){case WorldSpeed.Pause:return 0;case WorldSpeed.Normal:return 1;case WorldSpeed.Hour:return 3600;case WorldSpeed.Day:return 86400;default:throw new ArgumentOutOfRangeException(nameof(speed));}
        }
        public void Accumulate(double realSeconds)
        {
            if(double.IsNaN(realSeconds)||double.IsInfinity(realSeconds)||realSeconds<0||realSeconds>3600)throw new ArgumentOutOfRangeException(nameof(realSeconds));
            if(Speed==WorldSpeed.Pause)return;
            double amount=realSeconds*Rate(Speed)*Second+FractionalMilliseconds;
            long whole=(long)Math.Floor(amount+1e-8);
            FractionalMilliseconds=Math.Max(0,amount-whole);
            PendingMilliseconds=checked(PendingMilliseconds+whole);
        }
    }
    [Serializable] public sealed class WorldSite
    {
        public string Id, RegionId;
        public WorldPoint Position;
    }
    [Serializable] public sealed class WorldRoad
    {
        public string Id, From, To;
        public bool Blocked;
        public double SpeedFactor=1;
        public List<WorldPoint> Points=new List<WorldPoint>();
    }
    [Serializable] public sealed class WorldRoute
    {
        public string DestinationSiteId="", DestinationRegionId="";
        public List<WorldPoint> Points=new List<WorldPoint>();
        public List<string> RoadIds=new List<string>();
        public List<string> SegmentRoadIds=new List<string>();
        public int Segment;
        public double TravelledMetres;
    }
    [Serializable] public sealed class WorldCommander
    {
        public string Id, CharacterId;
        public float Competence=60, Loyalty=50, Ambition=50;
    }
    [Serializable] public sealed class WorldHeadquarters
    {
        public string Id, CommanderId;
        public WorldPoint Position, Destination;
        public bool Moving;
        public float Integrity=100;
        public long OrderReceivedAt;
    }
    [Serializable] public sealed class WorldUnitOrder
    {
        public WorldIntent Intent;
        public WorldFormation Formation;
        public WorldPoint Destination;
        public long IssuedAt, ReceivedAt;
    }
    [Serializable] public sealed class WorldUnit
    {
        public string Id;
        public WorldUnitKind Kind;
        public WorldFormation Formation;
        public WorldIntent Intent=WorldIntent.Advance;
        public int Men, Original, Ammo=16, Captured;
        public float Morale=78, Cohesion=90, Fatigue, Experience=45;
        public double Facing, Reload, LastFiredAt=-100, Quiet;
        public WorldPoint Position, Destination;
        public bool Moving, Routed;
        public bool ManualOrder;
        public bool Replenishing;
        public WorldPoint ResumeDestination;
        public WorldRole Role;
        public WorldWithdrawal Withdrawal;
        public WorldPressure Pressure;
        public double ReorganizeUntil, WithdrawalSeconds;
        public WorldPoint AssignedPosition;
        public List<WorldUnitOrder> Orders=new List<WorldUnitOrder>();
    }
    [Serializable] public sealed class WorldArmy
    {
        public string Id, NameKey, FactionId, RegionId, CommanderId, HeadquartersId;
        public ArmyActivity Activity;
        public WorldPoint Position;
        public WorldRoute Route=new WorldRoute();
        public List<WorldUnit> Units=new List<WorldUnit>();
        public double MovementSpeed=1.1;
        public float Supply=100, Fatigue;
        public double Rations, HungrySeconds;
        public long OrderIssuedAt, OrderReceivedAt, RecoverUntil;
        public WorldPosture Posture;
        public WorldPoint FrontAnchor, Forward, RetreatPoint;
        public bool Deployed, RearBlocked;
        public double ReserveDecisionAt, DisruptionSeconds;
        public int AmmunitionWagon=160;
        public WorldPoint WagonPosition;
        public float WagonIntegrity=100;
        public int Men { get {int result=0;foreach(var unit in Units)result+=unit.Men;return result;} }
        public float Morale { get {double total=0;foreach(var unit in Units)total+=unit.Morale*unit.Men;return Men==0?0:(float)(total/Men);} }
    }
    [Serializable] public sealed class WorldBattle
    {
        public string Id, FirstArmyId, SecondArmyId, WinnerId="", RegionId;
        public long StartedAt, EndedAt, NextTickAt;
        public WorldPoint Contact;
        public uint RandomState=1789;
        public bool Ended;
        public int FirstOriginal, SecondOriginal;
    }
    [Serializable] public sealed class WorldState
    {
        public int Schema=3, NextBattleId=1, NextConvoyId=1;
        public WorldClock Clock=new WorldClock();
        public BattleTimePolicy BattlePolicy=BattleTimePolicy.SlowToNormal;
        public string PlayerArmyId="royal", SelectedArmyId="royal", SelectedUnitId="";
        public List<WorldSite> Sites=new List<WorldSite>();
        public List<WorldRoad> Roads=new List<WorldRoad>();
        public List<WorldArmy> Armies=new List<WorldArmy>();
        public List<WorldCommander> Commanders=new List<WorldCommander>();
        public List<WorldHeadquarters> Headquarters=new List<WorldHeadquarters>();
        public List<WorldBattle> Battles=new List<WorldBattle>();
        public List<WorldTerrainFeature> Terrain=new List<WorldTerrainFeature>();
        public List<WorldSighting> Sightings=new List<WorldSighting>();
        public List<WorldDepot> Depots=new List<WorldDepot>();
        public List<WorldConvoy> Convoys=new List<WorldConvoy>();
        public long NextDayAt=WorldClock.Day, NextEconomyAt=WorldClock.Week, NextConditionAt=900000;
        public string LastNoticeKey="world.ready", LastNoticeRegion="ile";
        public WorldArmy Army(string id) => Armies.Find(a=>a.Id==id);
        public bool HasCombat => Battles.Exists(b=>!b.Ended);
    }
}
