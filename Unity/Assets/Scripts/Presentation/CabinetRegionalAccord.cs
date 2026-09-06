using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private RegionalAccordTerms accordPreview;
        private ActionResult accordCheck;

        private static string AccordMeasure(float value)
        { return value.ToString("0.#",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU")); }

        private void ObserveRegionalAccord(CampaignState state)
        {
            accordPreview=CampaignCore.HasRegionalAccord(state)
                ?CampaignCore.GetActiveRegionalAccordTerms(state)
                :CampaignCore.GetRegionalAccordTerms(state,state.SelectedRegionId);
            accordCheck=CampaignCore.CanGrantRegionalAccord(state,state.SelectedRegionId);
        }

        private void RegionalAccordEntry(GameApp app,ref float y)
        {
            bool active=accordPreview!=null&&accordPreview.IsActive;
            string regionId=active?accordPreview.RegionId:app.State.SelectedRegionId;
            Paragraph(ref y,T(active?"ui.accord.entry_active":"ui.accord.entry_offer",T("region."+regionId)),small,238,8);
            if(Press(new Rect(4,y,238,39),T("ui.accord.entry_button"),true,true))
            { OpenDocument("accord");app.Feedback("paper"); }
            y+=55;
        }

        private void RegionalAccord(GameApp app)
        {
            float y=0;
            if(Press(new Rect(4,y,238,30),T("ui.accord.back")))
            { OpenDocument("council");app.Feedback("paper"); }
            y+=45;
            Paragraph(ref y,T("ui.accord.title"),heading,238,14);
            var terms=accordPreview;
            if(terms==null)
            {
                Paragraph(ref y,accordCheck==null?T("ui.mandate.select_region"):L.Text(accordCheck.Key,accordCheck.Args),body);
                documentContentHeight=y+14;return;
            }
            var patron=app.State.Characters.Find(person=>person.Id=="morel");
            Seal(new Rect(2,y,82,96),1);
            string name=T("character.morel.name");
            float nameHeight=body.CalcHeight(new GUIContent(name),146);
            Text(new Rect(96,y+4,146,nameHeight),name,body);
            Text(new Rect(96,y+nameHeight+12,146,64),T("ui.accord.intermediary"),small);
            y+=Mathf.Max(112,nameHeight+84);
            if(patron!=null)
            { Pair(4,y,238,T("ui.person.relationship"),Number(patron.Relationship));y+=34; }
            Paragraph(ref y,T("region."+terms.RegionId),heading,238,12);
            Paragraph(ref y,T(terms.IsActive?"ui.accord.active":"ui.accord.offer"),body,238,14);
            if(terms.IsActive)
            {
                Paragraph(ref y,T("ui.accord.remaining",terms.RemainingWeeks,MandatePresentation.Date(terms.UntilWeek)),body,238,12);
                if(terms.RegionId!=app.State.SelectedRegionId)
                {
                    if(Press(new Rect(4,y,238,38),T("ui.accord.show_region")))app.SelectRegion(terms.RegionId);
                    y+=51;
                }
            }
            else
            {
                var region=CampaignCore.Region(app.State,terms.RegionId);
                Paragraph(ref y,T("ui.accord.immediate",AccordMeasure(region.Unrest),AccordMeasure(Mathf.Clamp(region.Unrest+terms.Immediate.Unrest,0,100)),
                    AccordMeasure(region.Control),AccordMeasure(Mathf.Clamp(region.Control+terms.Immediate.Control,0,100))),body,238,12);
                Paragraph(ref y,T("ui.accord.proposed_end",MandatePresentation.Date(terms.UntilWeek)),small,238,14);
            }
            Rule(4,y,238);y+=17;
            Paragraph(ref y,T("ui.accord.cost_title"),heading,238,10);
            if(!terms.IsActive)
                Paragraph(ref y,T("ui.accord.income_change",Number(terms.CurrentTaxIncome),Number(terms.ProjectedTaxIncome)),body,238,12);
            Paragraph(ref y,T(terms.IsActive?"ui.accord.forgone":"ui.accord.proposed_forgone",Number(terms.TaxForgone)),body,238,10);
            Paragraph(ref y,T("ui.accord.no_debt"),small,238,16);
            Paragraph(ref y,T("ui.accord.route"),small,238,16);
            Rule(4,y,238);y+=17;
            Paragraph(ref y,T("ui.accord.kept_title"),heading,238,10);
            Paragraph(ref y,T("ui.accord.kept",Signed(Mathf.RoundToInt(terms.Fulfil.Relationship)),Signed(Mathf.RoundToInt(terms.Fulfil.Approval))),body,238,16);
            Paragraph(ref y,T("ui.accord.broken_title"),heading,238,10);
            Paragraph(ref y,T("ui.accord.broken",Change(terms.Break.Unrest),Change(terms.Break.Control),Change(terms.Break.Relationship),
                Change(terms.Break.Approval),Change(terms.Break.Power)),body,238,12);
            Paragraph(ref y,T("ui.accord.separate"),small,238,19);
            if(!terms.IsActive)
            {
                bool available=accordCheck!=null&&accordCheck.Ok;
                if(!available&&accordCheck!=null)
                {
                    var warning=new GUIStyle(small);warning.normal.textColor=red;
                    Paragraph(ref y,L.Text(accordCheck.Key,accordCheck.Args),warning,238,12);
                }
                if(Press(new Rect(4,y,238,46),T("ui.accord.sign"),available,true))app.GrantRegionalAccord();
                y+=62;
            }
            documentContentHeight=y+12;
        }
    }
}
