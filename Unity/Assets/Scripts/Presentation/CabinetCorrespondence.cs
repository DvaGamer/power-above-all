using System;
using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private string cabinetAutonomy = "strict", cabinetIntent = "order", correspondencePage = "report";
        private bool cabinetExpress, correspondenceVisible;
        private CorrespondenceDesk correspondenceSource;
        private int correspondenceOrderId, correspondenceReceivedDay;
        private GUIStyle letterBody, letterSmall, letterHeading;
        private const float LetterWidth = 308;
        private static string AuthorityKey(string intent,string autonomy) => intent=="bread" ? "dispatch.bread_"+autonomy : "dispatch."+autonomy;
        private float ProvinceWidth => correspondenceVisible ? 360 : 245;
        private static string DispatchDate(int day) => new DateTime(1789,5,5).AddDays(day).ToString("d MMM",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"));

        private void ObserveCorrespondence(CampaignState state)
        {
            var desk = CampaignCore.Desk(state);
            correspondenceVisible = desk != null && state.SelectedRegionId == desk.RegionId;
            if (desk == null) { correspondenceSource = null; return; }
            bool replaced = !ReferenceEquals(desk, correspondenceSource);
            if (replaced) SetCorrespondencePage("report");
            else if (desk.NextOrderId != correspondenceOrderId) SetCorrespondencePage("outbox");
            else if (desk.LastReport.ArrivalDay != correspondenceReceivedDay) SetCorrespondencePage("report");
            correspondenceSource = desk;
            correspondenceOrderId = desk.NextOrderId;
            correspondenceReceivedDay = desk.LastReport.ArrivalDay;
        }
        private void SetCorrespondencePage(string page)
        {
            correspondencePage = page; provinceScroll = Vector2.zero; provinceContentHeight = 480;
        }
        public void OpenCorrespondencePage(CampaignState state, string page)
        {
            if (page != "report" && page != "outbox" && page != "draft") throw new ArgumentException("Unknown correspondence page.", nameof(page));
            ObserveCorrespondence(state); SetCorrespondencePage(page);
        }
        private void ComposeLetter(string intent = "order")
        {
            cabinetIntent = intent; SetCorrespondencePage("draft");
        }
        private void CorrespondenceProvince(GameApp app)
        {
            var desk = CampaignCore.Desk(app.State);
            var known = CampaignCore.Knowledge(app.State, desk.RegionId);
            letterBody ??= Style(16, ink);
            letterSmall ??= Style(14, muted);
            letterHeading ??= Style(22, ink, true);
            Fill(new Rect(15,146,349,658),new Color(ink.r,ink.g,ink.b,.12f));
            Fill(new Rect(12,142,348,658),paper);
            Fill(new Rect(12,142,3,658),brass);
            Text(new Rect(28,154,268,33),T("city.guyenne"),title);
            Text(new Rect(28,193,306,22),T("dispatch.recipient"),letterSmall);
            string[] pages = { "report", "outbox", "draft" };
            for (int i = 0; i < pages.Length; i++)
            {
                var tab = new Rect(28+i*104,224,99,33);
                bool current = correspondencePage == pages[i];
                if (current) Fill(tab,pale);
                if (GUI.Button(tab,T("dispatch.page."+pages[i]),quietButton)) SetCorrespondencePage(pages[i]);
                if (current) Fill(new Rect(tab.x,tab.yMax-2,tab.width,2),forest);
            }
            Rule(28,263,308);
            provinceScroll = BeginMatteScroll(new Rect(24,279,325,451),provinceScroll,new Rect(0,0,312,Mathf.Max(451,provinceContentHeight)),178904);
            float y = 0;
            if (correspondencePage == "report") DrawReceivedLetter(app,desk,known,ref y);
            else if (correspondencePage == "outbox") DrawSentLetters(desk,ref y);
            else DrawLetterDraft(app,ref y);
            provinceContentHeight = y+14; GUI.EndScrollView();
            Rule(28,743,308);
            if (correspondencePage == "draft")
            {
                var check=CampaignCore.CanSendCabinetOrder(app.State,cabinetIntent,cabinetAutonomy,cabinetExpress);
                string cost=cabinetIntent=="bread"?(cabinetExpress?"bread_express":"bread"):(cabinetExpress?"express":"free");
                if (Press(new Rect(28,754,308,34),T("dispatch.seal_send",T("dispatch.postage."+cost)),check.Ok,true)) app.SendCabinetOrder(cabinetIntent,cabinetAutonomy,cabinetExpress);
            }
            else if (Press(new Rect(28,754,308,34),T("dispatch.write_next"),true,true)) ComposeLetter();
        }
        private void DrawReceivedLetter(GameApp app, CorrespondenceDesk desk, RegionKnowledge known, ref float y)
        {
            var stamp = new GUIStyle(letterSmall);
            stamp.normal.textColor = known.Confidence == IntelligenceConfidence.Outdated ? red : muted;
            Paragraph(ref y,T(known.Confidence==IntelligenceConfidence.Outdated?"dispatch.outdated":"dispatch.reported",DispatchDate(known.ObservedDay),known.AgeDays),stamp,LetterWidth,8);
            Paragraph(ref y,T("dispatch.received_date",DispatchDate(known.ReceivedDay)),letterSmall,LetterWidth,19);
            LetterMetric(ref y,T("ui.unrest"),Number(known.Unrest));
            LetterMetric(ref y,T("ui.control"),Number(known.Control));
            Paragraph(ref y,T("dispatch.knowledge"),letterSmall,LetterWidth,18);
            Rule(4,y,LetterWidth);y+=18;
            bool hasReply = false;
            foreach (var order in desk.Orders)
            {
                if (!order.ReportReceived) continue;
                hasReply = true;
                Paragraph(ref y,T("dispatch.outcome."+order.Outcome),letterHeading,LetterWidth,12);
                Paragraph(ref y,T("dispatch.result_dates",DispatchDate(order.ExecutionDay),DispatchDate(order.ReportDay)),letterSmall,LetterWidth,12);
                Paragraph(ref y,T("dispatch.reason."+order.Outcome),letterBody,LetterWidth,14);
                if(order.Intent=="order"||order.Intent=="bread") Paragraph(ref y,T("dispatch.sent_terms",T("dispatch.intent."+order.Intent),T(AuthorityKey(order.Intent,order.Autonomy))),letterSmall,LetterWidth,18);
            }
            if (!hasReply) Paragraph(ref y,T("dispatch.initial_note"),letterBody,LetterWidth,16);
            int today=CampaignCore.CurrentDay(app.State);
            int next= today>desk.OpenedDay ? today+4 : today+11;
            foreach(var order in desk.Orders) if(!order.ReportReceived) next=Mathf.Min(next,order.ReportDay);
            Paragraph(ref y,T("dispatch.next_report",DispatchDate(next)),letterSmall,LetterWidth,12);
            if(Press(new Rect(4,y,LetterWidth,34),T("dispatch.prepare_query")))ComposeLetter("report");y+=44;
            if(Press(new Rect(4,y,LetterWidth,30),T("dispatch.open_history")))OpenDocument("journal");y+=40;
        }
        private void DrawSentLetters(CorrespondenceDesk desk, ref float y)
        {
            bool pending=false;
            foreach(var order in desk.Orders)
            {
                if(order.ReportReceived)continue;
                pending=true;
                Paragraph(ref y,T("dispatch.intent."+order.Intent),letterHeading,LetterWidth,12);
                Paragraph(ref y,T("dispatch.departed",DispatchDate(order.IssuedDay)),letterSmall,LetterWidth,14);
                Paragraph(ref y,T("dispatch.to_person"),letterBody,LetterWidth,10);
                if(order.Intent=="order"||order.Intent=="bread") Paragraph(ref y,T(AuthorityKey(order.Intent,order.Autonomy)),letterBody,LetterWidth,10);
                Paragraph(ref y,T(order.Express?"dispatch.express":"dispatch.normal"),letterSmall,LetterWidth,18);
                Paragraph(ref y,T("dispatch.expected_arrival",DispatchDate(order.ArrivalDay)),letterSmall,LetterWidth,8);
                Paragraph(ref y,T("dispatch.awaiting",DispatchDate(order.ReportDay)),letterBody,LetterWidth,14);
                Paragraph(ref y,T("dispatch.no_receipt"),letterSmall,LetterWidth,18);
                Rule(4,y,LetterWidth);y+=18;
            }
            if(!pending) Paragraph(ref y,T("dispatch.outbox_empty"),letterBody,LetterWidth,16);
            if(Press(new Rect(4,y,LetterWidth,34),T("dispatch.read_report")))SetCorrespondencePage("report");y+=44;
        }
        private void DrawLetterDraft(GameApp app, ref float y)
        {
            Paragraph(ref y,T("dispatch.draft_from",DispatchDate(CampaignCore.CurrentDay(app.State))),letterSmall,LetterWidth,12);
            string[] intents={"order","bread","tax","report"};
            for(int i=0;i<intents.Length;i++)
            {
                Rect choice=new Rect(4+(i%2)*158,y+(i/2)*45,150,39);
                if(Press(choice,T("dispatch.topic."+intents[i]),true,cabinetIntent==intents[i]))cabinetIntent=intents[i];
            }
            y+=102;
            if(cabinetIntent=="order"||cabinetIntent=="bread")
            {
                Paragraph(ref y,T("dispatch.authority"),letterSmall,LetterWidth,8);
                if(Press(new Rect(4,y,LetterWidth,34),T(AuthorityKey(cabinetIntent,"strict")),true,cabinetAutonomy=="strict"))cabinetAutonomy="strict";y+=40;
                if(Press(new Rect(4,y,LetterWidth,34),T(AuthorityKey(cabinetIntent,"mission")),true,cabinetAutonomy=="mission"))cabinetAutonomy="mission";y+=44;
                Paragraph(ref y,T((cabinetIntent=="bread"?"dispatch.bread_risk.":"dispatch.risk.")+cabinetAutonomy),letterSmall,LetterWidth,16);
            }
            else if(cabinetIntent=="report") Paragraph(ref y,T("dispatch.query_only"),letterBody,LetterWidth,16);
            if(Press(new Rect(4,y,LetterWidth,34),T(cabinetExpress?"dispatch.express":"dispatch.normal"),true,cabinetExpress))cabinetExpress=!cabinetExpress;y+=46;
            int today=CampaignCore.CurrentDay(app.State),trip=cabinetExpress?3:6,preparation=cabinetIntent=="report"?0:2;
            Paragraph(ref y,T("dispatch.schedule",DispatchDate(today+trip),DispatchDate(today+trip*2+preparation)),letterSmall,LetterWidth,12);
            Paragraph(ref y,T("dispatch.cost."+cabinetIntent),letterSmall,LetterWidth,12);
            var check=CampaignCore.CanSendCabinetOrder(app.State,cabinetIntent,cabinetAutonomy,cabinetExpress);
            if(!check.Ok)
            {
                var warning=new GUIStyle(letterSmall);warning.normal.textColor=red;
                Paragraph(ref y,L.Text(check.Key,check.Args),warning,LetterWidth,14);
            }
        }
        private void LetterMetric(ref float y,string label,string value)
        {
            Text(new Rect(4,y,230,25),label,letterBody);
            var number=new GUIStyle(letterHeading);number.alignment=TextAnchor.UpperRight;
            Text(new Rect(239,y-3,73,30),value,number);y+=36;
        }
    }
}
