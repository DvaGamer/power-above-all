using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private CampaignState establishmentSource;
        private string establishmentPolicy;
        private int establishmentTarget, establishmentDraft;
        private ArmyEstablishmentTerms establishmentCurrent, establishmentPreview;
        private ActionResult establishmentCheck, establishmentStopCheck;

        private void ObserveArmyEstablishment(CampaignState state)
        {
            if (!ReferenceEquals(establishmentSource, state) || establishmentPolicy != state.ArmyPolicyId ||
                establishmentTarget != state.ArmyTargetTroops)
            {
                establishmentSource = state;
                establishmentPolicy = state.ArmyPolicyId;
                establishmentTarget = state.ArmyTargetTroops;
                establishmentDraft = state.ArmyPolicyId == "budget" ? state.ArmyTargetTroops :
                    Mathf.Max(0, state.Troops - CampaignCore.ArmyReductionBatch);
            }
            establishmentCurrent = CampaignCore.GetArmyEstablishmentTerms(state);
            RefreshEstablishmentDraft(state);
        }

        private void RefreshEstablishmentDraft(CampaignState state)
        {
            establishmentPreview = CampaignCore.GetArmyEstablishmentTerms(state, "budget", establishmentDraft);
            establishmentCheck = CampaignCore.CanSetArmyEstablishment(state, "budget", establishmentDraft);
            establishmentStopCheck = CampaignCore.CanSetArmyEstablishment(state, "campaign", 0);
        }

        private void ArmyEstablishmentEntry(GameApp app, ref float y)
        {
            if (Press(new Rect(4, y, 238, 39), T("ui.establishment.open")))
            { OpenDocument("establishment"); app.Feedback("paper"); }
            y += 49;
            var terms = establishmentCurrent;
            if (terms != null && terms.PolicyId == "budget")
                Paragraph(ref y, terms.DueWeek > 0 ? T("ui.establishment.entry_due", Number(terms.TargetTroops),
                    MandatePresentation.Date(terms.DueWeek)) : T("ui.establishment.entry_idle", Number(terms.TargetTroops)), small, 238, 15);
        }

        private void ArmyEstablishment(GameApp app)
        {
            float y = 0;
            if (Press(new Rect(4, y, 238, 30), T("ui.establishment.back")))
            { OpenDocument("economy"); app.Feedback("paper"); }
            y += 43;
            Paragraph(ref y, T("ui.establishment.title"), heading, 238, 10);
            Paragraph(ref y, T("ui.establishment.rule", CampaignCore.ArmyReductionBatch, CampaignCore.ArmyReductionWeeks), small, 238, 14);
            bool active = app.ViewState.ArmyPolicyId == "budget";
            if (active && establishmentDraft != app.ViewState.ArmyTargetTroops && establishmentCurrent != null)
                Paragraph(ref y, establishmentCurrent.DueWeek > 0 ? T("ui.establishment.entry_due", Number(establishmentCurrent.TargetTroops),
                    MandatePresentation.Date(establishmentCurrent.DueWeek)) : T("ui.establishment.entry_idle", Number(establishmentCurrent.TargetTroops)), small, 238, 12);
            Paragraph(ref y, T(active && establishmentDraft == app.ViewState.ArmyTargetTroops ?
                "ui.establishment.target_active" : "ui.establishment.target_draft"), tiny, 238, 8);
            bool changed = false;
            if (Press(new Rect(4, y, 62, 34), "−200", establishmentDraft > 0))
            { establishmentDraft = Mathf.Max(0, establishmentDraft - CampaignCore.ArmyReductionBatch); changed = true; }
            var countStyle = new GUIStyle(heading) { alignment = TextAnchor.MiddleCenter };
            countStyle.fontSize = establishmentDraft > 999999 ? 16 : 21;
            Text(new Rect(72, y + 3, 102, 31), Number(establishmentDraft), countStyle);
            if (Press(new Rect(180, y, 62, 34), "+200", establishmentDraft < CampaignCore.MaximumArmyTarget))
            { establishmentDraft = (int)System.Math.Min(CampaignCore.MaximumArmyTarget, (long)establishmentDraft + CampaignCore.ArmyReductionBatch); changed = true; }
            y += 42;
            for (int i = 0; i < 4; i++)
            {
                int target = i * 400;
                if (Press(new Rect(4 + i * 61, y, 55, 29), Number(target), establishmentDraft != target))
                { establishmentDraft = target; changed = true; }
            }
            y += 43;
            if (changed) { RefreshEstablishmentDraft(app.ViewState); app.Feedback("paper"); }
            var terms = establishmentPreview;
            if (terms == null)
            {
                Paragraph(ref y, establishmentCheck != null ? L.Text(establishmentCheck.Key, establishmentCheck.Args) :
                    T("error.establishment.state"), small, 238, 12);
                ArmyEstablishmentStop(app, active, ref y);
                documentContentHeight = y + 12; return;
            }

            Rule(4, y, 238); y += 14;
            Paragraph(ref y, T("ui.establishment.now", Number(terms.CurrentTroops)), body, 238, 7);
            Paragraph(ref y, T("ui.establishment.cost", Number(terms.CurrentArmyCost), Number(terms.CurrentArmyConsumption)), small, 238, 14);
            if (terms.NextBatchTroops > 0)
            {
                Paragraph(ref y, T("ui.establishment.conditions"), small, 238, 12);
                Paragraph(ref y, T(active && establishmentDraft == app.ViewState.ArmyTargetTroops ? "ui.establishment.first" :
                    "ui.establishment.draft_first", MandatePresentation.Date(terms.DueWeek)), body, 238, 7);
                Paragraph(ref y, T("ui.establishment.transfer", Number(terms.TroopsAfterBatch), Number(terms.NextBatchTroops)), body, 238, 7);
                Paragraph(ref y, T("ui.establishment.cost", Number(terms.ArmyCostAfterBatch), Number(terms.ArmyConsumptionAfterBatch)), small, 238, 7);
                Paragraph(ref y, terms.FirstReducedBudgetWeek > 0 ? T("ui.establishment.delayed", MandatePresentation.Date(terms.FirstReducedBudgetWeek)) :
                    T("ui.establishment.final_calculation"), small, 238, 14);
            }
            else Paragraph(ref y, L.Text(terms.ReasonKey, terms.ReasonArgs), body, 238, 14);
            Paragraph(ref y, T("ui.establishment.target", Number(terms.TargetTroops), Number(terms.ExcessTroops)), body, 238, 10);
            if (terms.NextBatchTroops > 0)
            {
                Paragraph(ref y, T("ui.establishment.dumas", Change(terms.DumasRelationshipDelta)), body, 238, 12);
            }
            if (establishmentDraft == 0)
            {
                var warning = new GUIStyle(small); warning.normal.textColor = red;
                Paragraph(ref y, T(app.ViewState.Troops == 0 ? "ui.establishment.no_garrison_now" : "ui.establishment.no_garrison",
                    T("region." + app.ViewState.ArmyRegionId)), warning, 238, 14);
            }
            Rule(4, y, 238); y += 15;
            if (establishmentCheck != null && !establishmentCheck.Ok && establishmentCheck.Key != "error.establishment.unchanged")
            {
                var warning = new GUIStyle(small); warning.normal.textColor = red;
                Paragraph(ref y, L.Text(establishmentCheck.Key, establishmentCheck.Args), warning, 238, 12);
                if(establishmentCheck.Key=="error.establishment.commission")
                {
                    if(Press(new Rect(4,y,238,36),T("ui.commission.open")))
                    {OpenDocument("officers");app.Feedback("paper");}
                    y+=48;
                }
            }
            if (Press(new Rect(4, y, 238, 43), T(active ? "ui.establishment.update" : "ui.establishment.start"),
                establishmentCheck != null && establishmentCheck.Ok, true)) app.SetArmyEstablishment("budget", establishmentDraft);
            y += 55;
            ArmyEstablishmentStop(app, active, ref y);
            documentContentHeight = y + 12;
        }

        private void ArmyEstablishmentStop(GameApp app, bool active, ref float y)
        {
            if (active)
            {
                if (Press(new Rect(4, y, 238, 38), T("ui.establishment.stop"), establishmentStopCheck != null && establishmentStopCheck.Ok))
                    app.SetArmyEstablishment("campaign", 0);
                y += 50;
            }
            Paragraph(ref y, T("ui.establishment.stop_note"), small, 238, 10);
        }
    }
}
