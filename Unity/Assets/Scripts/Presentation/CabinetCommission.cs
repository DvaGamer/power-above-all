using System;
using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        public bool ShowCommission;
        private FirstCommission observedCommission;
        private static readonly Rect CommissionRect = new Rect(972,142,446,660);
        public void OpenCommission() { ShowCommission=true; PanelsHidden=false; ShowWorldSupply=false; showDocument=false; }
        public void CloseCommission(GameApp app)
        {
            var c=CampaignCore.Commission(app.State);
            if(c!=null&&c.Resolved)c.Seen=true;
            ShowCommission=false;
        }
        private void ObserveCommission(GameApp app)
        {
            var c=CampaignCore.Commission(app.State);
            if(c!=observedCommission)
            {
                observedCommission=c;ShowCommission=false;
                if(c!=null&&app.State.World.Clock.Milliseconds==c.StartedAt)OpenCommission();
            }
            if(c!=null&&c.Resolved&&!c.Seen)OpenCommission();
        }
        private void CommissionBadge(GameApp app)
        {
            var c=CampaignCore.Commission(app.State);if(c==null)return;
            string text=c.Resolved?T("commission.badge_result"):T("commission.badge",Math.Max(0,(int)Math.Ceiling((c.DueAt-app.State.World.Clock.Milliseconds)/(double)WorldClock.Day)));
            if(Press(new Rect(1190,95,228,28),text)) { if(ShowCommission)CloseCommission(app);else OpenCommission(); }
        }
        private static string CommissionDate(long time) => new DateTime(1789,5,5).AddMilliseconds(time).ToString("d MMM · HH:mm",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"));
        private void CommissionDesk(GameApp app)
        {
            var c=CampaignCore.Commission(app.State);if(!ShowCommission||PanelsHidden||c==null)return;
            Fill(CommissionRect,paper);Border(CommissionRect,rule);Fill(new Rect(972,142,4,660),brass);
            Text(new Rect(990,155,375,27),T(c.Resolved?"commission.result_title":"commission.title"),heading);
            if(Press(new Rect(1383,154,23,24),"×")){CloseCommission(app);return;}
            Text(new Rect(990,188,400,24),T("commission.patron",T("character."+CampaignCore.PatronIdForRole(c.RoleId)+".name"),CommissionDate(c.DueAt)),small);
            Text(new Rect(990,217,400,47),T(c.Resolved?(c.Succeeded?"commission.success_detail":"commission.failure_detail"):"commission.brief."+c.RoleId),body);
            var measures=CampaignCore.CommissionMeasures(app.State);
            for(int i=0;i<measures.Count;i++)
            {
                var m=measures[i];float y=274+i*57;
                Fill(new Rect(990,y+3,3,44),m.Met?forest:red);
                Text(new Rect(1002,y,235,20),T("commission.measure."+m.Id),body);
                double shown=(m.AtMost?Math.Ceiling(m.Value*10):Math.Floor(m.Value*10))/10;
                Text(new Rect(1234,y,166,20),T(m.AtMost?"commission.at_most":"commission.at_least",shown.ToString("0.#",CultureInfo.CurrentCulture),m.Target.ToString("0",CultureInfo.CurrentCulture)),small);
                Text(new Rect(1002,y+22,396,33),T("commission.hint."+m.Id),tiny);
            }
            float footer=679;
            if(c.Resolved)
            {
                Text(new Rect(990,footer,400,40),T("commission.frozen",CommissionDate(c.ResolvedAt)),small);
                if(Press(new Rect(990,738,410,36),T("commission.continue"))){CloseCommission(app);app.SetWorldSpeed(1);}
            }
            else
            {
                string timing=app.State.Obligation==null?T(c.Broken>0?"commission.promise_broken":c.Kept>0?"commission.promise_kept":"commission.get_promise"):T("commission.promise_dates",CommissionDate(app.State.Obligation.DueWeek*WorldClock.Week),CommissionDate(app.State.Obligation.DueWeek*WorldClock.Week+CampaignCore.MandateGrace));
                Text(new Rect(990,footer,407,49),timing,small);
                if(Press(new Rect(990,740,199,35),T("commission.open_mandate"))){CloseCommission(app);ShowMandateTerms();}
                if(Press(new Rect(1197,740,203,35),T("commission.action."+c.RoleId)))
                {
                    CloseCommission(app);
                    if(c.RoleId=="crown")OpenDocument("economy");
                    else if(c.RoleId=="assembly"){app.SelectRegion("champagne");app.StrategyCamera.Focus(app.Map.RegionWorld("champagne"),100);}
                    else {app.FocusWorldArmy();app.OpenWorldSupply();}
                }
            }
        }
    }
}
