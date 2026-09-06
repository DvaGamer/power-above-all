using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private OfficerCommissionTerms commissionPreview;
        private ActionResult commissionGrantCheck, commissionRecruitCheck, commissionRevokeCheck;

        private void ObserveOfficerCommission(CampaignState state)
        {
            commissionPreview = CampaignCore.GetOfficerCommissionTerms(state);
            commissionGrantCheck = CampaignCore.CanGrantOfficerCommission(state);
            commissionRecruitCheck = CampaignCore.CanRecruitThroughDumas(state);
            commissionRevokeCheck = CampaignCore.CanRevokeOfficerCommission(state);
        }

        private void CommissionRefusal(ActionResult check, ref float y)
        {
            if (check == null || check.Ok) return;
            var warning = new GUIStyle(small); warning.normal.textColor = red;
            Paragraph(ref y, L.Text(check.Key, check.Args), warning, 238, 12);
        }

        private void OfficerCommission(GameApp app)
        {
            float y = 0;
            if (Press(new Rect(4, y, 238, 30), T("ui.accord.back")))
            { OpenDocument("council"); app.Feedback("paper"); }
            y += 43;
            Paragraph(ref y, T("ui.commission.title"), heading, 238, 12);
            var terms = commissionPreview;
            if (terms == null)
            {
                Paragraph(ref y, T("error.commission.state"), small, 238, 12);
                documentContentHeight = y + 12; return;
            }
            Seal(new Rect(2, y, 76, 88), 3);
            string name = T("character.dumas.name");
            float nameHeight = body.CalcHeight(new GUIContent(name), 152);
            Text(new Rect(90, y + 2, 152, nameHeight), name, body);
            var general = app.ViewState.Characters.Find(person => person.Id == "dumas");
            Text(new Rect(90, y + nameHeight + 10, 152, 58),
                T("ui.commission.character", Number(general.Loyalty), Number(general.Ambition)), small);
            y += Mathf.Max(105, nameHeight + 80);
            Paragraph(ref y, T(terms.IsActive ? "ui.commission.active" : "ui.commission.offer"), body, 238, 12);
            Paragraph(ref y, T("ui.commission.camp", T("region." + app.ViewState.ArmyRegionId)), small, 238, 12);
            if (app.ViewState.SelectedRegionId != app.ViewState.ArmyRegionId)
            {
                if (Press(new Rect(4, y, 238, 34), T("ui.forage.show_camp"))) app.SelectRegion(app.ViewState.ArmyRegionId);
                y += 46;
            }
            Rule(4, y, 238); y += 15;
            Paragraph(ref y, T("ui.commission.recruit_title", terms.RecruitTroops), heading, 238, 10);
            Paragraph(ref y, T("ui.commission.recruit_rule"), small, 238, 12);
            Paragraph(ref y, T("ui.commission.recruit_cost", Number(terms.GoldCost), Number(terms.FoodCost),
                Number(terms.MilitarySuppliesCost), Number(terms.ManpowerCost)), body, 238, 10);
            Paragraph(ref y, T("ui.commission.recruit_effects", Change(terms.UnrestDelta), Change(terms.MoraleDelta),
                Change(terms.ArmyApprovalDelta), Change(terms.LoyaltyDelta)), small, 238, 10);
            if(terms.TroopsAfterRecruit>terms.CurrentTroops)
                Paragraph(ref y, T("ui.commission.upkeep", Number(terms.CurrentArmyCost), Number(terms.ArmyCostAfterRecruit),
                    Number(terms.CurrentArmyConsumption), Number(terms.ArmyConsumptionAfterRecruit)), small, 238, 15);
            else if(!terms.IsActive)
                Paragraph(ref y, T("ui.commission.recruit_unaffordable"), small, 238, 15);
            if (terms.IsActive)
            {
                CommissionRefusal(commissionRecruitCheck, ref y);
                if (Press(new Rect(4, y, 238, 43), T("ui.commission.recruit", terms.RecruitTroops),
                    commissionRecruitCheck != null && commissionRecruitCheck.Ok, true)) app.RecruitThroughDumas();
                y += 57;
            }
            Rule(4, y, 238); y += 15;
            Paragraph(ref y, T("ui.commission.authority"), heading, 238, 10);
            Paragraph(ref y, T("ui.commission.authority_cost", Number(terms.RevokeGoldCost), Number(terms.CurrentTroops)), body, 238, 12);
            Paragraph(ref y, T("ui.commission.authority_rule"), small, 238, 14);
            if (terms.IsActive)
            {
                CommissionRefusal(commissionRevokeCheck, ref y);
                if (Press(new Rect(4, y, 238, 43), T("ui.commission.revoke", Number(terms.RevokeGoldCost)),
                    commissionRevokeCheck != null && commissionRevokeCheck.Ok)) app.RevokeOfficerCommission();
                y += 57;
            }
            else
            {
                CommissionRefusal(commissionGrantCheck, ref y);
                if (Press(new Rect(4, y, 238, 43), T("ui.commission.grant"),
                    commissionGrantCheck != null && commissionGrantCheck.Ok, true)) app.GrantOfficerCommission();
                y += 57;
            }
            if (Press(new Rect(4, y, 238, 34), T("ui.establishment.open")))
            { OpenDocument("establishment"); app.Feedback("paper"); }
            y += 46;
            documentContentHeight = y + 12;
        }
    }
}
