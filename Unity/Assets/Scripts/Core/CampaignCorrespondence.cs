using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PowerAboveAll
{
    public enum IntelligenceConfidence { Confirmed, Reported, Estimated, Rumored, Unknown, Outdated }

    [Serializable] public sealed class RegionalDispatch
    {
        public int ObservedDay, ArrivalDay, OrderId;
        public float Unrest, Control, EliteLoyalty;
        public string Outcome = "routine";
    }
    [Serializable] public sealed class CabinetOrder
    {
        public int Id, IssuedDay, ArrivalDay, ExecutionDay, ReportDay;
        public string Intent, Autonomy, ExecutorId;
        public bool Express, Executed, ReportReceived;
        public string Outcome = "";
    }
    [Serializable] public sealed class CorrespondenceDesk
    {
        public string PlayerId = "adrien", PlayerRegionId = "ile", RegionId = "guyenne", ExecutorId = "delmas";
        public float Competence = 68, Loyalty = 48, Ambition = 76;
        public int OpenedDay, NextOrderId = 1;
        public RegionalDispatch LastReport;
        public List<CabinetOrder> Orders = new List<CabinetOrder>();
        public List<RegionalDispatch> InTransit = new List<RegionalDispatch>();
    }
    public sealed class RegionKnowledge
    {
        public string RegionId, SourceId;
        public int ObservedDay, ReceivedDay, AgeDays;
        public float Unrest, Control, EliteLoyalty;
        public IntelligenceConfidence Confidence;
    }

    public static partial class CampaignCore
    {
        public static CorrespondenceDesk Desk(CampaignState s) => s.Correspondence != null && s.Correspondence.Count == 1 ? s.Correspondence[0] : null;
        public static ActionResult OpenCorrespondence(CampaignState s)
        {
            if(Desk(s)!=null)return Result(false,"dispatch.already_open");
            if(s.PendingPetition||MandateDue(s))return Result(false,"dispatch.resolve_first");
            if(s.Week>MaximumWeek-4)return Result(false,"error.week.limit");
            int today=s.Week*7;
            var desk=new CorrespondenceDesk{OpenedDay=today};
            // Başlangıç dosyası kurgusal senaryo geçmişidir; geçmiş simülasyon replay'i değildir.
            desk.LastReport=MakeDispatch(Region(s,desk.RegionId),today-4,today,0,"routine");
            s.Correspondence.Add(desk);
            return Record(s,"log.dispatch.opened");
        }
        public static RegionKnowledge Knowledge(CampaignState s,string regionId)
        {
            var desk=Desk(s);
            if(desk!=null&&desk.RegionId==regionId)
            {
                var report=desk.LastReport;int age=s.Week*7-report.ObservedDay;
                return new RegionKnowledge{RegionId=regionId,SourceId=desk.ExecutorId,ObservedDay=report.ObservedDay,
                    ReceivedDay=report.ArrivalDay,AgeDays=age,Unrest=report.Unrest,Control=report.Control,EliteLoyalty=report.EliteLoyalty,
                    Confidence=age>7?IntelligenceConfidence.Outdated:IntelligenceConfidence.Reported};
            }
            var r=Region(s,regionId);
            return new RegionKnowledge{RegionId=regionId,SourceId="administration",ObservedDay=s.Week*7,ReceivedDay=s.Week*7,
                Unrest=r.Unrest,Control=r.Control,EliteLoyalty=r.EliteLoyalty,Confidence=IntelligenceConfidence.Confirmed};
        }
        public static ActionResult CanSendCabinetOrder(CampaignState s,string intent,string autonomy,bool express)
        {
            var desk=Desk(s);if(desk==null)return Result(false,"dispatch.not_open");
            if(desk.NextOrderId>=2000001)return Result(false,"error.week.limit");
            if(s.PendingPetition||MandateDue(s))return Result(false,"dispatch.resolve_first");
            if(s.Week>MaximumWeek-4)return Result(false,"error.week.limit");
            if(intent!="bread"&&intent!="tax"&&intent!="order"&&intent!="report")return Result(false,"error.action");
            if(autonomy!="strict"&&autonomy!="mission")return Result(false,"error.action");
            foreach(var old in desk.Orders)
                if(!old.ReportReceived&&((intent=="report")== (old.Intent=="report")))return Result(false,"dispatch.pending");
            if(intent=="bread"&&s.Food<40)return Result(false,"error.bread.cost");
            if(express&&s.Gold<12)return Result(false,"dispatch.express_cost");
            if(intent=="tax"&&HasRegionalAccord(s)&&s.AccordRegionId==desk.RegionId)return Result(false,"dispatch.accord");
            if(intent=="tax"&&s.Gold>MaximumStock-100)return Result(false,"error.capacity");
            return Result(true,"dispatch.ready");
        }
        public static ActionResult SendCabinetOrder(CampaignState s,string intent,string autonomy,bool express)
        {
            var check=CanSendCabinetOrder(s,intent,autonomy,express);if(!check.Ok)return check;
            var desk=Desk(s);int today=s.Week*7,journey=express?3:6;
            if(express)s.Gold-=12;if(intent=="bread")s.Food-=40;
            desk.Orders.RemoveAll(o=>o.ReportReceived);
            desk.Orders.Add(new CabinetOrder{Id=desk.NextOrderId++,IssuedDay=today,ArrivalDay=today+journey,
                ExecutionDay=today+journey+(intent=="report"?0:2),ReportDay=today+journey*2+(intent=="report"?0:2),
                Intent=intent,Autonomy=autonomy,ExecutorId=desk.ExecutorId,Express=express});
            return Record(s,"log.dispatch.sent","dispatch.intent."+intent);
        }
        private static RegionalDispatch MakeDispatch(RegionState r,int observed,int arrival,int order,string outcome)
        { return new RegionalDispatch{ObservedDay=observed,ArrivalDay=arrival,OrderId=order,Outcome=outcome,Unrest=r.Unrest,Control=r.Control,EliteLoyalty=r.EliteLoyalty}; }

        private static void AdvanceCorrespondence(CampaignState s)
        {
            var desk=Desk(s);if(desk==null)return;
            int today=s.Week*7;
            for(int day=today-6;day<=today;day++)
            {
                foreach(var order in desk.Orders)
                {
                    if(order.Executed||day<order.ExecutionDay)continue;
                    var r=Region(s,desk.RegionId);
                    bool coercion=order.Autonomy=="mission"&&desk.Ambition>desk.Loyalty;
                    if(order.Intent=="bread")
                    {
                        bool rationed=coercion&&desk.Competence<75;
                        r.Unrest=Clamp(r.Unrest-(rationed?8:15));r.Control=Clamp(r.Control+(rationed?5:2));
                        order.Outcome=rationed?"rationed":"relief";
                    }
                    else if(order.Intent=="tax")
                    {r.Unrest=Clamp(r.Unrest+12);r.EliteLoyalty=Clamp(r.EliteLoyalty-4);order.Outcome="tax";}
                    else if(order.Intent=="order")
                    {
                        r.Unrest=Clamp(r.Unrest-(coercion?16:8));r.Control=Clamp(r.Control+(coercion?10:4));
                        r.EliteLoyalty=Clamp(r.EliteLoyalty+(coercion?6:-2));
                        order.Outcome=coercion?"force":"negotiated";
                    }
                    else order.Outcome="report";
                    order.Executed=true;
                    desk.InTransit.Add(MakeDispatch(r,day,order.ReportDay,order.Id,order.Outcome));
                }
                // Haftalık durum raporu da yolculuk eder; eski paket daha yeni gözlemi geri alamaz.
                if(day==today)desk.InTransit.Add(MakeDispatch(Region(s,desk.RegionId),day,day+4,0,"routine"));
                for(int i=0;i<desk.InTransit.Count;)
                {
                    var report=desk.InTransit[i];if(report.ArrivalDay>day){i++;continue;}
                    desk.InTransit.RemoveAt(i);
                    if(report.ObservedDay>=desk.LastReport.ObservedDay)desk.LastReport=report;
                    var order=desk.Orders.Find(o=>o.Id==report.OrderId);
                    if(order==null)continue;
                    order.ReportReceived=true;
                    if(order.Outcome=="tax")s.Gold=Stock((long)s.Gold+100);
                    if(order.Outcome=="force")
                    {s.Power=Clamp(s.Power-2);Faction(s,"urban").Approval=Clamp(Faction(s,"urban").Approval-4);desk.Ambition=Clamp(desk.Ambition+3);}
                    if(order.Outcome=="relief")
                    {Faction(s,"urban").Approval=Clamp(Faction(s,"urban").Approval+2);Character(s,"lefevre").Relationship=Clamp(Character(s,"lefevre").Relationship+2);}
                    if(order.Outcome=="negotiated")desk.Loyalty=Clamp(desk.Loyalty+2);
                    Record(s,"log.dispatch.returned","dispatch.outcome."+order.Outcome);
                }
            }
        }
        private static void ValidateCorrespondence(CampaignState s)
        {
            Require(s.Correspondence!=null&&s.Correspondence.Count<=1);
            var d=Desk(s);if(d==null)return;
            int today=s.Week*7;
            Require(d.PlayerId=="adrien"&&d.PlayerRegionId=="ile"&&d.RegionId=="guyenne"&&d.ExecutorId=="delmas");
            Require(d.OpenedDay>=0&&d.OpenedDay<=today&&d.NextOrderId>0&&d.NextOrderId<=2000001);
            Require(Percent(d.Competence)&&Percent(d.Loyalty)&&Percent(d.Ambition));
            Require(d.Orders!=null&&d.Orders.Count<=2&&d.InTransit!=null&&d.InTransit.Count<=4&&d.LastReport!=null);
            ValidateDispatch(d.LastReport,d.OpenedDay-4,today);
            Require(d.LastReport.ArrivalDay<=today);
            var ids=new HashSet<int>();int activeAction=0,activeReport=0;
            foreach(var o in d.Orders)
            {
                Require(o!=null&&o.Id>0&&o.Id<d.NextOrderId&&ids.Add(o.Id)&&o.ExecutorId==d.ExecutorId);
                Require(o.Intent=="bread"||o.Intent=="tax"||o.Intent=="order"||o.Intent=="report");
                Require(o.Autonomy=="strict"||o.Autonomy=="mission");int journey=o.Express?3:6;
                Require(o.IssuedDay>=d.OpenedDay&&o.IssuedDay<=today&&o.IssuedDay%7==0&&o.ArrivalDay==o.IssuedDay+journey);
                Require(o.ExecutionDay==o.ArrivalDay+(o.Intent=="report"?0:2)&&o.ReportDay==o.ExecutionDay+journey);
                Require(o.Executed==(o.ExecutionDay<=today)&&o.ReportReceived==(o.ReportDay<=today));
                Require(o.Executed?OutcomeMatches(o.Intent,o.Outcome):o.Outcome=="");
                if(!o.ReportReceived){if(o.Intent=="report")activeReport++;else activeAction++;}
            }
            Require(activeAction<=1&&activeReport<=1);
            var reportIds=new HashSet<int>();
            foreach(var p in d.InTransit)
            {
                ValidateDispatch(p,d.OpenedDay,today);Require(p.ArrivalDay>today&&p.ArrivalDay<=today+6);
                if(p.OrderId==0){Require(p.Outcome=="routine"&&p.ArrivalDay==p.ObservedDay+4);continue;}
                Require(reportIds.Add(p.OrderId));var o=d.Orders.Find(item=>item.Id==p.OrderId);
                Require(o!=null&&o.Executed&&!o.ReportReceived&&o.ReportDay==p.ArrivalDay&&o.ExecutionDay==p.ObservedDay&&o.Outcome==p.Outcome);
            }
            foreach(var o in d.Orders)Require(!o.Executed||o.ReportReceived||reportIds.Contains(o.Id));
        }
        private static bool OutcomeMatches(string intent,string outcome)=>intent=="bread"?(outcome=="relief"||outcome=="rationed"):intent=="order"?(outcome=="force"||outcome=="negotiated"):intent=="tax"?outcome=="tax":intent=="report"&&outcome=="report";
        private static bool ValidOutcome(string value)=>value=="routine"||value=="relief"||value=="rationed"||value=="tax"||value=="negotiated"||value=="force"||value=="report";
        private static void ValidateDispatch(RegionalDispatch p,int earliest,int today)
        {
            Require(p!=null&&p.ObservedDay>=earliest&&p.ObservedDay<=today&&p.ArrivalDay>=p.ObservedDay&&p.OrderId>=0&&ValidOutcome(p.Outcome));
            Require(Percent(p.Unrest)&&Percent(p.Control)&&Percent(p.EliteLoyalty));
        }
    }
}
