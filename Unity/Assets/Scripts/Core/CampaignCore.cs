using System;
using System.Collections.Generic;
using System.Globalization;

namespace PowerAboveAll
{
    [Serializable] public sealed class RegionDefinition
    {
        public string Id;
        public float X, Y;
        public int Population, BaseTax, BaseFood;
        public string[] Neighbours;
    }
    [Serializable] public sealed class RegionState
    {
        public string Id;
        public float Unrest, Control, EliteLoyalty;
        public bool BreadUsed, TaxUsed, RecruitUsed;
    }
    [Serializable] public sealed class FactionState
    {
        public string Id, LeaderId, DemandKey;
        public float Influence, Approval, Radicalism;
    }
    [Serializable] public sealed class CharacterState
    {
        public string Id, FactionId, NameKey, PositionKey, AgendaKey;
        public float Loyalty, Ambition, Competence, Relationship;
    }
    [Serializable] public sealed class LogEntry
    {
        public string Key;
        public string[] Args;
        public int Week;
    }
    [Serializable] public sealed class CampaignState
    {
        public int Week, Gold, Food, MilitarySupplies, Manpower, Troops, Moves;
        public string ArmyRegionId, SelectedRegionId;
        public float Morale, Supply, Fatigue, Power;
        public List<RegionState> Regions = new List<RegionState>();
        public List<FactionState> Factions = new List<FactionState>();
        public List<CharacterState> Characters = new List<CharacterState>();
        public List<LogEntry> Journal = new List<LogEntry>();
        public List<string> ResolvedBattles = new List<string>();
        public bool SubsidyParis;
        public bool PendingPetition, PetitionResolved;
        [System.Runtime.Serialization.OptionalField] public string RoleId;
        [System.Runtime.Serialization.OptionalField] public int NextMandateWeek;
        // v1/v2 arşivleri bu iki alanı taşımaz; v3 wire sözleşmesi varlıklarını ayrıca doğrular.
        [System.Runtime.Serialization.OptionalField] public string AccordRegionId = "";
        [System.Runtime.Serialization.OptionalField] public int AccordUntilWeek;
        // v4 arşivinde zorunlu; eski zaferler yeni bir tercih üretmez.
        [System.Runtime.Serialization.OptionalField] public string PendingVictoryId = "";
        [System.Runtime.Serialization.OptionalField] public int DumasForageDueWeek, DumasNextForageWeek;
        [System.Runtime.Serialization.OptionalField] public string ArmyPolicyId = "campaign";
        [System.Runtime.Serialization.OptionalField] public int ArmyTargetTroops, ArmyReductionDueWeek;
        [System.Runtime.Serialization.OptionalField] public bool DumasOfficerCommission, DumasExtraRecruitUsed;
        // JsonUtility boş sınıfı örnekleyebilir; boş liste ise gerçek yokluğu korur.
        [System.Runtime.Serialization.OptionalField] public List<MandateObligation> Mandates;
        public MandateObligation Obligation
        {
            get { return Mandates != null && Mandates.Count > 0 ? Mandates[0] : null; }
            set
            {
                if (Mandates == null) Mandates = new List<MandateObligation>();
                Mandates.Clear();
                if (value != null) Mandates.Add(value);
            }
        }
    }
    public sealed class ActionResult
    {
        public bool Ok, RequiresBattle;
        public string Key;
        public string[] Args = new string[0];
    }
    [System.Serializable] public sealed class MarchPreview
    {
        public int FoodCost, FoodAfter, MilitarySuppliesAfter, MovesAfter;
        public float Supply, Fatigue, Morale;
        public bool Difficult, Hungry;
    }
    public sealed class EconomyForecast
    {
        public int TaxIncome, ArmyCost, Production, CivilianConsumption;
        public int ArmyConsumption, SubsidyConsumption, NetGold, NetFood;
        public int ForageFood;
    }

    // Fictional balance data. No engine, rendering, clock or localization dependency.
    public static partial class CampaignCore
    {
        private const int MaximumStock = 100000000;
        private const int MaximumWeek = 1000000;
        private static readonly string[] FactionIds = { "crown", "assembly", "urban", "army" };
        private static readonly string[] CharacterIds = { "valcourt", "morel", "lefevre", "dumas" };
        public static readonly RegionDefinition[] Regions = {
            Def("brittany",185,288,800000,24,18,"normandy","orleans","poitou"),
            Def("normandy",323,240,1000000,32,20,"brittany","picardy","ile","orleans"),
            Def("picardy",449,173,700000,23,17,"normandy","ile","champagne"),
            Def("ile",439,267,1300000,48,8,"normandy","picardy","champagne","burgundy","orleans"),
            Def("champagne",534,252,600000,25,15,"picardy","ile","lorraine","burgundy"),
            Def("lorraine",618,254,650000,24,12,"champagne","burgundy"),
            Def("burgundy",548,367,800000,29,16,"ile","champagne","lorraine","orleans","languedoc","provence"),
            Def("orleans",391,353,650000,23,21,"brittany","normandy","ile","burgundy","poitou","guyenne"),
            Def("poitou",304,409,600000,19,19,"brittany","orleans","guyenne"),
            Def("guyenne",349,511,950000,32,16,"poitou","orleans","languedoc"),
            Def("languedoc",474,543,1000000,28,18,"guyenne","burgundy","provence"),
            Def("provence",588,517,800000,33,11,"burgundy","languedoc")
        };
        private static RegionDefinition Def(string id,float x,float y,int pop,int tax,int food,params string[] near)
        { return new RegionDefinition { Id=id,X=x,Y=y,Population=pop,BaseTax=tax,BaseFood=food,Neighbours=near }; }
        private static float Clamp(float value) { return Math.Max(0f, Math.Min(100f,value)); }
        private static int Stock(long value) { return (int)Math.Max(0L,Math.Min(MaximumStock,value)); }
        private static int Round(double value) { return (int)Math.Round(value,MidpointRounding.AwayFromZero); }
        private static string N(int value) { return value.ToString(CultureInfo.InvariantCulture); }
        private static RegionDefinition Definition(string id) { return Array.Find(Regions,r=>r.Id==id); }
        public static RegionState Region(CampaignState s,string id) { return s.Regions.Find(r=>r.Id==id); }
        private static FactionState Faction(CampaignState s,string id) { return s.Factions.Find(f=>f.Id==id); }
        private static CharacterState Character(CampaignState s,string id) { return s.Characters.Find(c=>c.Id==id); }
        private static ActionResult Result(bool ok,string key,params string[] args)
        { return new ActionResult { Ok=ok,Key=key,Args=args }; }
        private static ActionResult Record(CampaignState s,string key,params string[] args)
        {
            s.Journal.Insert(0,new LogEntry { Week=s.Week,Key=key,Args=args });
            if(s.Journal.Count>40)s.Journal.RemoveRange(40,s.Journal.Count-40);
            return Result(true,key,args);
        }
        public static CampaignState Create()
        {
            var s=new CampaignState { Gold=840,Food=360,MilitarySupplies=120,Manpower=2400,Troops=1200,
                ArmyRegionId="ile",SelectedRegionId="ile",Moves=2,Morale=78,Supply=100,Power=55,
                RoleId="legacy",Mandates=new List<MandateObligation>() };
            float[] unrest={38,30,42,48,69,47,33,27,41,35,44,52};
            for(int i=0;i<Regions.Length;i++)s.Regions.Add(new RegionState { Id=Regions[i].Id,Unrest=unrest[i],Control=Clamp(95-unrest[i]*.5f),EliteLoyalty=60 });
            float[] influence={72,56,45,58}, approval={65,45,35,60}, radical={12,30,40,15};
            for(int i=0;i<FactionIds.Length;i++)
            {
                string id=FactionIds[i], person=CharacterIds[i];
                s.Factions.Add(new FactionState { Id=id,LeaderId=person,DemandKey="faction."+id+".demand",Influence=influence[i],Approval=approval[i],Radicalism=radical[i] });
                s.Characters.Add(new CharacterState { Id=person,FactionId=id,NameKey="character."+person+".name",PositionKey="character."+person+".position",AgendaKey="character."+person+".agenda",Loyalty=approval[i],Ambition=i==3?80:55,Competence=i==3?78:65,Relationship=50 });
            }
            Record(s,"log.begin");return s;
        }
        public static float AverageUnrest(CampaignState s)
        { float total=0;foreach(var r in s.Regions)total+=r.Unrest;return total/s.Regions.Count; }
        // Taxes depend on unrest, actual government control and Assembly approval.
        // Weekly army cost includes 36 gold to replenish 18 military supplies.
        public static EconomyForecast Forecast(CampaignState s)
        { return BuildWeekProjection(new EconomyView(s, HasRegionalAccord(s) ? s.AccordRegionId : null)).Economy; }

        // Saf yaprak: girişim/önizleme çağırmaz; bütün senaryolar aynı yuvarlamayı kullanır.
        private static EconomyForecast CalculateEconomy(EconomyView view)
        {
            var s=view.State;
            double tax=0,food=0;
            foreach(var d in Regions)
            {
                var r=Region(s,d.Id);
                float unrest=view.Unrest(r),control=view.Control(r);
                if(d.Id!=view.ExemptRegion)tax+=d.BaseTax*(1-unrest/150f)*(.5f+control/200f);
                food+=d.BaseFood*(1-unrest/200f);
            }
            var f=new EconomyForecast {
                TaxIncome=Round(tax*(.75f+Faction(s,"assembly").Approval/200f)),
                ArmyCost=ArmyCostFor(s,s.Troops),Production=Round(food),
                CivilianConsumption=110,ArmyConsumption=ArmyFoodFor(s.Troops),SubsidyConsumption=s.SubsidyParis?20:0
            };
            f.NetGold=f.TaxIncome-f.ArmyCost;
            f.NetFood=f.Production-f.CivilianConsumption-f.ArmyConsumption-f.SubsidyConsumption;return f;
        }
        public static ActionResult Act(CampaignState s,string action,string id)
        {
            var r=Region(s,id);if(r==null)return Result(false,"error.region");
            var urban=Faction(s,"urban");
            switch(action)
            {
                case "bread":
                    if(r.BreadUsed)return Result(false,"error.used");
                    if(s.Food<40)return Result(false,"error.bread.cost");
                    s.Food-=40;r.Unrest=Clamp(r.Unrest-15);r.Control=Clamp(r.Control+2);r.BreadUsed=true;
                    urban.Approval=Clamp(urban.Approval+2);Character(s,"lefevre").Relationship=Clamp(Character(s,"lefevre").Relationship+2);
                    return Record(s,"log.bread","region."+id);
                case "tax":
                    if(r.TaxUsed)return Result(false,"error.used");
                    if(s.Gold>MaximumStock-100)return Result(false,"error.capacity");
                    BreakRegionalAccordForTax(s,id);
                    s.Gold+=100;r.Unrest=Clamp(r.Unrest+12);r.EliteLoyalty=Clamp(r.EliteLoyalty-4);r.TaxUsed=true;
                    urban.Approval=Clamp(urban.Approval-3);Faction(s,"crown").Approval=Clamp(Faction(s,"crown").Approval+1);
                    return Record(s,"log.tax","region."+id);
                case "recruit":
                    var recruitCheck=CheckRecruitment(s,r,false);if(!recruitCheck.Ok)return recruitCheck;
                    ApplyRecruitment(s,r);
                    return Record(s,"log.recruit","region."+id);
                case "subsidy":
                    if(id!="ile")return Result(false,"error.subsidy.location");
                    s.SubsidyParis=!s.SubsidyParis;
                    if(s.SubsidyParis)return Record(s,"log.subsidy.start");
                    r.Unrest=Clamp(r.Unrest+8);urban.Approval=Clamp(urban.Approval-8);urban.Radicalism=Clamp(urban.Radicalism+5);
                    Character(s,"lefevre").Relationship=Clamp(Character(s,"lefevre").Relationship-5);s.Power=Clamp(s.Power-2);
                    return Record(s,"log.subsidy.stop");
                default:return Result(false,"error.action");
            }
        }
        public static ActionResult CanMarch(CampaignState s,string id)
        {
            if(Definition(id)==null)return Result(false,"error.region");
            if(s.Troops<=0)return Result(false,"error.army.empty");
            if(s.Moves<=0)return Result(false,"error.moves");
            if(Array.IndexOf(Definition(s.ArmyRegionId).Neighbours,id)<0)return Result(false,"error.adjacent");
            bool hostile=IsHostileRegion(Region(s,id));
            var result=Result(true,hostile?"march.battle":"march.ready");result.RequiresBattle=hostile;return result;
        }
        public static MarchPreview PreviewMarch(CampaignState s,string id)
        {
            if(!CanMarch(s,id).Ok)return null;
            return TravelProjection(s,id);
        }
        private static MarchPreview TravelProjection(CampaignState s,string id)
        {
            var r=Region(s,id);bool difficult=r.Control<55||r.Unrest>=50;
            int cost=(int)Math.Ceiling(s.Troops/100d)+(difficult?6:0);
            bool hungry=s.Food<cost;
            return new MarchPreview { FoodCost=cost,FoodAfter=Stock((long)s.Food-cost),
                MilitarySuppliesAfter=Stock((long)s.MilitarySupplies-5),
                Supply=Clamp(s.Supply-(difficult?12:5)-(hungry?15:0)),
                Fatigue=Clamp(s.Fatigue+(difficult?20:10)),Morale=Clamp(s.Morale-(hungry?8:0)),
                MovesAfter=Math.Max(0,s.Moves-(difficult?2:1)),Difficult=difficult,Hungry=hungry };
        }
        public static float BattleReturnMorale(float arrivalMorale,float endingMorale,bool won)
        { return Clamp(Math.Min(arrivalMorale,endingMorale)+(won?3:-8)); }
        public static void RecoverMilitarySupplies(CampaignState s,int amount)
        { s.MilitarySupplies=Stock((long)s.MilitarySupplies+Math.Max(0,amount)); }
        // Preview and commitment share one travel calculation. The campaign is only charged on resolution.
        private static void Travel(CampaignState s,string id,bool battle)
        {
            var r=Region(s,id);var arrival=TravelProjection(s,id);
            s.PendingVictoryId="";
            s.Food=arrival.FoodAfter;s.MilitarySupplies=arrival.MilitarySuppliesAfter;
            s.Supply=arrival.Supply;s.Fatigue=arrival.Fatigue;s.Morale=arrival.Morale;s.Moves=arrival.MovesAfter;
            if(arrival.Difficult){r.Unrest=Clamp(r.Unrest+2);r.Control=Clamp(r.Control-2);}
            if(arrival.Hungry&&!battle)
            {int lost=(int)Math.Ceiling(s.Troops*.02d);s.Troops-=lost;Record(s,"log.march.attrition",N(lost));}
            if(!battle)RefreshArmyReduction(s);
        }
        public static ActionResult March(CampaignState s,string id)
        {
            var check=CanMarch(s,id);if(!check.Ok)return check;
            if(check.RequiresBattle){check.Ok=false;return check;}
            Travel(s,id,false);s.ArmyRegionId=id;return Record(s,"log.march","region."+id);
        }
        public static ActionResult ResolveBattle(CampaignState s,string target,string battleId,bool won,int casualties,float endingMorale)
        {
            if(string.IsNullOrEmpty(battleId)||casualties<0||casualties>s.Troops||!Percent(endingMorale)||(won&&casualties==s.Troops))return Result(false,"error.battle.result");
            if(s.ResolvedBattles.Contains(battleId))return Result(false,"error.battle.duplicate");
            var check=CanMarch(s,target);if(!check.Ok)return check;
            string expected="battle-"+N(s.Week)+"-"+N(s.Moves)+"-"+s.ArmyRegionId+"-"+target;
            if(!check.RequiresBattle||battleId!=expected)return Result(false,"error.battle.stale");
            Travel(s,target,true);s.Troops-=casualties;s.ResolvedBattles.Add(battleId);
            s.Morale=BattleReturnMorale(s.Morale,endingMorale,won);s.Fatigue=Clamp(s.Fatigue+15);
            var r=Region(s,target);var army=Faction(s,"army");var general=Character(s,"dumas");
            if(won)
            {
                s.ArmyRegionId=target;r.Unrest=Clamp(r.Unrest-22);r.Control=Clamp(r.Control+12);
                Faction(s,"urban").Approval=Clamp(Faction(s,"urban").Approval-3);army.Approval=Clamp(army.Approval+4);
                general.Ambition=Clamp(general.Ambition+3);general.Relationship=Clamp(general.Relationship+2);s.Power=Clamp(s.Power+4);
                s.PendingVictoryId=battleId;
            }
            else {r.Unrest=Clamp(r.Unrest+5);army.Approval=Clamp(army.Approval-6);general.Relationship=Clamp(general.Relationship-4);s.Power=Clamp(s.Power-6);}
            RefreshArmyReduction(s);
            return Record(s,won?"log.battle.victory":"log.battle.defeat","region."+target,N(casualties),N(s.Troops));
        }
        // Port of browser 0.1's single grain-petition event, not a new event system.
        public static ActionResult ChoosePetition(CampaignState s,string id)
        {
            if(!s.PendingPetition||s.PetitionResolved)return Result(false,"error.petition.none");
            if(id!="relief"&&id!="negotiate"&&id!="refuse")return Result(false,"error.petition.choice");
            if(id=="relief"&&s.Food<60)return Result(false,"error.petition.food");
            var urban=Faction(s,"urban");var assembly=Faction(s,"assembly");var crown=Faction(s,"crown");
            if(id=="relief")
            {
                s.Food-=60;urban.Approval=Clamp(urban.Approval+15);assembly.Approval=Clamp(assembly.Approval+5);
                foreach(var r in s.Regions)r.Unrest=Clamp(r.Unrest-8);
            }
            else if(id=="negotiate")
            {
                assembly.Approval=Clamp(assembly.Approval+12);crown.Approval=Clamp(crown.Approval-8);
                Region(s,"ile").Unrest=Clamp(Region(s,"ile").Unrest-10);
            }
            else
            {
                crown.Approval=Clamp(crown.Approval+8);urban.Approval=Clamp(urban.Approval-10);
                foreach(var r in s.Regions)r.Unrest=Clamp(r.Unrest+5);
            }
            s.PendingPetition=false;s.PetitionResolved=true;
            return Record(s,"log.petition."+id);
        }
        public static ActionResult NextWeek(CampaignState s)
        {
            if(s.PendingPetition)return Result(false,"error.petition.pending");
            if(MandateDue(s))return Result(false,"error.mandate.due");
            if(s.Week>=MaximumWeek)return Result(false,"error.week.limit");
            var plan=BuildWeekProjection(new EconomyView(s,HasRegionalAccord(s)?s.AccordRegionId:null));
            s.PendingVictoryId="";
            var f=plan.Economy;bool hunger=(long)s.Food+f.NetFood<0,unpaid=(long)s.Gold+f.NetGold<0;
            ApplyDumasInitiative(s,plan.Initiative);
            int materials=(s.Troops>0||s.MilitarySupplies<120)&&!unpaid?18:0,materialUse=(int)Math.Ceiling(s.Troops/120d);
            bool unequipped=(long)s.MilitarySupplies+materials<materialUse;
            s.Gold=Stock((long)s.Gold+f.NetGold);s.Food=Stock((long)s.Food+f.NetFood);
            s.MilitarySupplies=Stock((long)s.MilitarySupplies+materials-materialUse);s.Week++;s.Moves=2;
            RecordDumasInitiative(s,plan.Initiative);
            var urban=Faction(s,"urban");var army=Faction(s,"army");
            bool strained=hunger||unpaid||unequipped;
            int lost=strained?(int)Math.Ceiling(s.Troops*(hunger?.08d:unpaid?.04d:.02d)):0;
            s.Troops-=lost;s.Supply=Clamp(s.Supply+(hunger?-25:unequipped?-18:unpaid?-12:10)-(Region(s,s.ArmyRegionId).Control<45?8:0));
            s.Morale=Clamp(s.Morale+(strained?-15:3));s.Fatigue=Clamp(s.Fatigue-12);
            army.Approval=Clamp(army.Approval+(strained?-7:1));s.Power=Clamp(s.Power+(strained?-5:.5f));
            if(strained)Character(s,"dumas").Loyalty=Clamp(Character(s,"dumas").Loyalty-5);
            if(s.SubsidyParis)
            {
                var paris=Region(s,"ile");
                if(hunger){urban.Approval=Clamp(urban.Approval-8);paris.Unrest=Clamp(paris.Unrest+6);Record(s,"log.subsidy.failed");}
                else {urban.Approval=Clamp(urban.Approval+3);paris.Unrest=Clamp(paris.Unrest-4);Character(s,"lefevre").Relationship=Clamp(Character(s,"lefevre").Relationship+1);Record(s,"log.subsidy.paid");}
            }
            foreach(var r in s.Regions)
            {
                bool garrison=r.Id==s.ArmyRegionId&&s.Troops>0;
                r.BreadUsed=r.TaxUsed=r.RecruitUsed=false;
                r.Unrest=Clamp(r.Unrest+(urban.Approval<40?2:urban.Approval>=60?-1:0)+(hunger?8:0)+(unpaid?4:0)-(garrison?3:0));
                r.Control=Clamp(r.Control+(garrison?2:0)+(r.EliteLoyalty<35?-2:0)-(r.Unrest>=65?3:0));
            }
            urban.Radicalism=Clamp(urban.Radicalism+(hunger?5:urban.Approval>=60?-1:0));
            s.DumasExtraRecruitUsed=false;
            if(strained)Record(s,"log.shortage",N(lost),hunger?"shortage.food":unpaid?"shortage.pay":"shortage.materials");
            if(s.Week==2&&!s.PetitionResolved){s.PendingPetition=true;Record(s,"log.petition.arrived");}
            CompleteRegionalAccordAfterWeek(s);
            CompleteArmyReductionAfterWeek(s);
            AnnounceDumasInitiativeAfterWeek(s,hunger);
            return Record(s,"log.week",N(s.Week),N(f.TaxIncome),N(f.ArmyCost),N(f.NetFood));
        }
        private static bool Percent(float n) { return !float.IsNaN(n)&&!float.IsInfinity(n)&&n>=0&&n<=100; }
        private static bool Key(string value) { return !string.IsNullOrEmpty(value)&&value.Length<=160; }
        private static void Require(bool condition) { if(!condition)throw new ArgumentException("Invalid campaign state."); }
        public static void Validate(CampaignState s)
        {
            ValidateBase(s);
            ValidateRoleState(s);
            ValidateRegionalAccordState(s);
            ValidateVictoryDecisionState(s);
            ValidateDumasInitiativeState(s);
            ValidateArmyEstablishmentState(s);
            ValidateOfficerCommissionState(s);
        }
        internal static void ValidateBase(CampaignState s)
        {
            Require(s!=null);Require(s.Week>=0&&s.Week<=MaximumWeek&&s.Moves>=0&&s.Moves<=2);
            if(s.Week<2)Require(!s.PendingPetition&&!s.PetitionResolved);
            else if(s.Week==2)Require(s.PendingPetition!=s.PetitionResolved);
            else Require(!s.PendingPetition&&s.PetitionResolved);
            foreach(int n in new[]{s.Gold,s.Food,s.MilitarySupplies,s.Manpower,s.Troops})Require(n>=0&&n<=MaximumStock);
            Require(Definition(s.ArmyRegionId)!=null&&Definition(s.SelectedRegionId)!=null);
            Require(Percent(s.Morale)&&Percent(s.Supply)&&Percent(s.Fatigue)&&Percent(s.Power));
            Require(s.Regions!=null&&s.Regions.Count==Regions.Length);var ids=new HashSet<string>();
            foreach(var r in s.Regions){Require(r!=null);Require(Definition(r.Id)!=null&&ids.Add(r.Id)&&Percent(r.Unrest)&&Percent(r.Control)&&Percent(r.EliteLoyalty));}
            Require(s.Factions!=null&&s.Factions.Count==FactionIds.Length);ids.Clear();
            foreach(var f in s.Factions)
            {Require(f!=null);Require(Array.IndexOf(FactionIds,f.Id)>=0&&ids.Add(f.Id)&&Percent(f.Influence)&&Percent(f.Approval)&&Percent(f.Radicalism)&&f.DemandKey=="faction."+f.Id+".demand");}
            Require(s.Characters!=null&&s.Characters.Count==CharacterIds.Length);ids.Clear();
            foreach(var c in s.Characters)
            {
                Require(c!=null);int index=Array.IndexOf(CharacterIds,c.Id);
                Require(index>=0&&ids.Add(c.Id));Require(c.FactionId==FactionIds[index]&&c.NameKey=="character."+c.Id+".name"&&c.PositionKey=="character."+c.Id+".position"&&c.AgendaKey=="character."+c.Id+".agenda");
                Require(Percent(c.Loyalty)&&Percent(c.Ambition)&&Percent(c.Competence)&&Percent(c.Relationship));
            }
            foreach(var f in s.Factions)Require(f.LeaderId==CharacterIds[Array.IndexOf(FactionIds,f.Id)]);
            Require(s.Journal!=null&&s.Journal.Count<=40);
            foreach(var entry in s.Journal)
            {Require(entry!=null);Require(Key(entry.Key)&&entry.Key.StartsWith("log.",StringComparison.Ordinal)&&entry.Week>=0&&entry.Week<=s.Week&&entry.Args!=null&&entry.Args.Length<=8);foreach(var a in entry.Args)Require(a!=null&&a.Length<=200);}
            Require(s.ResolvedBattles!=null&&s.ResolvedBattles.Count<=2L*(s.Week+1));ids.Clear();
            foreach(var id in s.ResolvedBattles)
            {
                Require(Key(id)&&ids.Add(id));var p=id.Split('-');int week,move;
                Require(p.Length==5&&p[0]=="battle");
                Require(int.TryParse(p[1],NumberStyles.None,CultureInfo.InvariantCulture,out week)&&week>=0&&week<=s.Week);
                Require(int.TryParse(p[2],NumberStyles.None,CultureInfo.InvariantCulture,out move)&&move>=1&&move<=2);
                var source=Definition(p[3]);Require(source!=null&&Array.IndexOf(source.Neighbours,p[4])>=0);
                Require(id=="battle-"+N(week)+"-"+N(move)+"-"+p[3]+"-"+p[4]);
            }
        }
    }
}
