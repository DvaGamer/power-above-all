using System;
using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private RegionalResistanceTerms resistancePreview;
        private bool seekResistanceReport;
        private float resistanceReportOffset;

        private void ResistanceMarchSummary(GameApp app,ref float y)
        {
            if(resistancePreview==null)return;
            string text=resistancePreview.RequiresBattle
                ?T("ui.resistance.march",Number(resistancePreview.EnemyTroops)):T("ui.resistance.peace_link");
            var link=new GUIStyle(tiny);link.normal.textColor=resistancePreview.RequiresBattle?red:ink;
            link.hover.textColor=ink;link.active.textColor=ink;
            float height=link.CalcHeight(new GUIContent(text),195);
            string ourText=T("ui.resistance.ours",Number(app.ViewState.Troops));
            float ourHeight=tiny.CalcHeight(new GUIContent(ourText),195);
            if(GUI.Button(new Rect(4,y,195,height+ourHeight+3),GUIContent.none,GUIStyle.none))
            {seekResistanceReport=true;app.Feedback("paper");}
            Text(new Rect(4,y,195,height),text,link);
            y+=height+3;
            Paragraph(ref y,ourText,tiny,195,8);
        }

        private void RegionalResistanceReport(ref float y)
        {
            if(resistancePreview==null)return;
            resistanceReportOffset=y;
            Rule(4,y,195);y+=14;
            Paragraph(ref y,T("ui.resistance.title"),heading,195,8);
            Paragraph(ref y,T(resistancePreview.RequiresBattle?"ui.resistance.forces":"ui.resistance.peace",
                Number(resistancePreview.EnemyTroops)),body,195,8);
            if(resistancePreview.RequiresBattle)
            {
                Paragraph(ref y,T("ui.resistance.origin",ResistanceNumber(resistancePreview.MobilizationBase)),tiny,195,8);
                Paragraph(ref y,T("ui.resistance.factors",
                    ResistanceNumber(resistancePreview.UnrestPressure*100),
                    ResistanceNumber(resistancePreview.ControlGap*100),
                    ResistanceNumber(resistancePreview.EliteOpposition*100)),tiny,195,8);
            }
            Paragraph(ref y,T("ui.resistance.threshold"),tiny,195,12);
            Rule(4,y,195);y+=16;
        }

        private static string ResistanceNumber(double value)
        {return value.ToString("0.##",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"));}
    }
}
