using System;
using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private string cabinetAutonomy = "strict";
        private bool cabinetExpress;
        private static string DispatchDate(int day) => new DateTime(1789,5,5).AddDays(day).ToString("d MMM",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"));

        private void CorrespondenceProvince(GameApp app)
        {
            var desk=CampaignCore.Desk(app.State);
            var known=CampaignCore.Knowledge(app.State,desk.RegionId);
            Fill(new Rect(0,94,245,706),paper);Fill(new Rect(244,94,1,706),rule);
            Text(new Rect(18,110,195,22),T("dispatch.file"),tiny);Rule(18,141,208);
            Text(new Rect(18,154,208,40),T("city.guyenne"),title);
            Text(new Rect(18,199,208,38),T("dispatch.recipient"),small);
            provinceScroll=BeginMatteScroll(new Rect(12,246,226,540),provinceScroll,new Rect(0,0,205,Mathf.Max(540,provinceContentHeight)),178904);
            float y=0;
            string status=known.Confidence==IntelligenceConfidence.Outdated?"dispatch.outdated":"dispatch.reported";
            Paragraph(ref y,T(status,DispatchDate(known.ObservedDay),known.AgeDays),body,195,6);
            Paragraph(ref y,T("dispatch.received_date",DispatchDate(known.ReceivedDay)),tiny,195,12);
            Pair(4,y,195,T("ui.unrest"),Number(known.Unrest));y+=32;
            Pair(4,y,195,T("ui.control"),Number(known.Control));y+=32;
            Paragraph(ref y,T("dispatch.knowledge"),tiny,195,12);
            Rule(4,y,195);y+=15;
            CorrespondenceOrders(desk,ref y);
            Paragraph(ref y,T("dispatch.person",Number(desk.Competence),Number(desk.Loyalty),Number(desk.Ambition)),small,195,12);
            Paragraph(ref y,T("dispatch.authority"),tiny,195,5);
            if(Press(new Rect(4,y,195,30),T("dispatch.strict"),true,cabinetAutonomy=="strict"))cabinetAutonomy="strict";y+=35;
            if(Press(new Rect(4,y,195,30),T("dispatch.mission"),true,cabinetAutonomy=="mission"))cabinetAutonomy="mission";y+=37;
            Paragraph(ref y,T("dispatch.risk."+cabinetAutonomy),tiny,195,10);
            if(Press(new Rect(4,y,195,30),T(cabinetExpress?"dispatch.express":"dispatch.normal"),true,cabinetExpress))cabinetExpress=!cabinetExpress;y+=39;
            int day=app.State.Week*7,trip=cabinetExpress?3:6;
            Paragraph(ref y,T("dispatch.schedule",DispatchDate(day+trip),DispatchDate(day+trip*2+2)),tiny,195,10);
            foreach(string intent in new[]{"order","bread","tax","report"})
            {
                var check=CampaignCore.CanSendCabinetOrder(app.State,intent,cabinetAutonomy,cabinetExpress);
                if(Press(new Rect(4,y,195,33),T("dispatch.send."+intent),check.Ok))app.SendCabinetOrder(intent,cabinetAutonomy,cabinetExpress);
                y+=39;
                if(!check.Ok&&check.Key!="dispatch.pending")Paragraph(ref y,L.Text(check.Key,check.Args),tiny,195,8);
            }
            provinceContentHeight=y+15;GUI.EndScrollView();
        }
        private void CorrespondenceOrders(CorrespondenceDesk desk,ref float y)
        {
            foreach(var order in desk.Orders)
            {
                Paragraph(ref y,T("dispatch.intent."+order.Intent),heading,195,6);
                Paragraph(ref y,T("dispatch.departed",DispatchDate(order.IssuedDay)),tiny,195,5);
                Paragraph(ref y,T(order.Autonomy=="mission"?"dispatch.mission":"dispatch.strict"),tiny,195,5);
                if(order.ReportReceived)
                {
                    Paragraph(ref y,T("dispatch.result_dates",DispatchDate(order.ExecutionDay),DispatchDate(order.ReportDay)),tiny,195,8);
                    Paragraph(ref y,T("dispatch.outcome."+order.Outcome),body,195,8);
                    Paragraph(ref y,T("dispatch.reason."+order.Outcome),small,195,10);
                }
                else
                    Paragraph(ref y,T("dispatch.awaiting",DispatchDate(order.ReportDay)),small,195,10);
                Rule(4,y,195);y+=16;
            }
        }
    }
}
