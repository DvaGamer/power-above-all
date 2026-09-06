using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private string reformDraftMode = "provisioning";
        private RegionalReformTerms reformCurrent, reformDraft;
        private ActionResult reformBeginCheck, reformEndCheck;

        public void PreviewRegionalReform(string mode)
        {
            if (mode != "provisioning" && mode != "commerce") return;
            reformDraftMode = mode;
            OpenDocument("reform");
            if (previewSource != null) ObserveRegionalReform(previewSource);
        }

        private void ObserveRegionalReform(CampaignState state)
        {
            reformCurrent = CampaignCore.GetRegionalReformTerms(state);
            bool closed = reformCurrent != null && reformCurrent.StatusId == "closed";
            reformDraft = closed ? CampaignCore.GetRegionalReformTerms(state, state.SelectedRegionId, reformDraftMode) : null;
            reformBeginCheck = CampaignCore.CanBeginRegionalReform(state, state.SelectedRegionId, reformDraftMode);
            reformEndCheck = CampaignCore.CanEndRegionalReform(state);
        }

        private void RegionalReformEntry(GameApp app, ref float y, float width)
        {
            if (Press(new Rect(4, y, width, 37), T("ui.reform.open")))
            { OpenDocument("reform"); app.Feedback("paper"); }
            y += 47;
        }

        private void RegionalReform(GameApp app)
        {
            float y = 0;
            if (Press(new Rect(4, y, 238, 30), T("ui.establishment.back")))
            { OpenDocument("economy"); app.Feedback("paper"); }
            y += 43;
            Paragraph(ref y, T("ui.reform.title"), heading, 238, 10);
            bool closed = reformCurrent != null && reformCurrent.StatusId == "closed";
            if (closed)
            {
                Paragraph(ref y, T("region." + app.ViewState.SelectedRegionId), heading, 238, 10);
                string[] modes = { "provisioning", "commerce" };
                for (int i = 0; i < modes.Length; i++)
                {
                    if (Press(new Rect(4 + i * 123, y, 115, 34), T("reform.mode." + modes[i]), true, reformDraftMode == modes[i]))
                    { reformDraftMode = modes[i]; ObserveRegionalReform(app.ViewState); app.Feedback("paper"); }
                }
                y += 45;
            }
            var terms = closed ? reformDraft : reformCurrent;
            if (terms == null)
            {
                Paragraph(ref y, T("error.reform.state"), small, 238, 12);
                documentContentHeight = y + 12; return;
            }
            if (!closed)
            {
                Paragraph(ref y, T("region." + terms.RegionId), heading, 238, 8);
                Paragraph(ref y, T("reform.mode." + terms.ModeId), body, 238, 8);
                Paragraph(ref y, T("ui.reform.original"), small, 238, 12);
            }
            string sponsor = T("character." + terms.SponsorId + ".name");
            Paragraph(ref y, T("ui.reform.sponsor", sponsor), small, 238, 10);
            Paragraph(ref y, T("reform.status." + terms.StatusId), body, 238, 12);
            Paragraph(ref y, T("ui.reform.direction." + terms.ModeId), small, 238, 14);

            Rule(4, y, 238); y += 16;
            Paragraph(ref y, T("ui.reform.comparison"), tiny, 238, 10);
            Paragraph(ref y, T("ui.reform.tax", Number(terms.WithoutReformTaxIncome), Number(terms.WithReformTaxIncome), Change(terms.TaxIncomeDelta)), body, 238, 8);
            Paragraph(ref y, T("ui.reform.production", Number(terms.WithoutReformProduction), Number(terms.WithReformProduction), Change(terms.ProductionDelta)), body, 238, 8);
            Paragraph(ref y, T("ui.reform.food_balance", Signed(terms.WithoutReformNetFood), Signed(terms.WithReformNetFood), Change(terms.NetFoodDelta)), small, 238, 8);
            if (terms.WithoutReformForageFood != terms.WithReformForageFood)
                Paragraph(ref y, T("ui.reform.forage", Number(terms.WithoutReformForageFood), Number(terms.WithReformForageFood)), small, 238, 8);
            Paragraph(ref y, T("ui.reform.conditional"), small, 238, 14);
            Paragraph(ref y, T("ui.reform.nominal", Number(terms.BaseTax), Number(terms.ReformedBaseTax), Number(terms.BaseFood), Number(terms.ReformedBaseFood)), small, 238, 14);

            Rule(4, y, 238); y += 16;
            bool active = terms.StatusId == "active";
            if (!active)
            {
                Paragraph(ref y, T("ui.reform.steps", terms.StepsRemaining), body, 238, 10);
                Paragraph(ref y, T("ui.reform.conditions", ResistanceNumber(CampaignCore.RegionalReformUnrestLimit), ResistanceNumber(CampaignCore.RegionalReformMinimumControl)), small, 238, 10);
                Paragraph(ref y, T("ui.reform.region_now", ResistanceNumber(terms.RegionUnrest), ResistanceNumber(terms.RegionControl)), small, 238, 10);
                if (!string.IsNullOrEmpty(terms.WaitReasonKey))
                {
                    var warning = new GUIStyle(small); warning.normal.textColor = red;
                    Paragraph(ref y, L.Text(terms.WaitReasonKey, terms.WaitReasonArgs), warning, 238, 10);
                }
                if (terms.EarliestActivationWeek >= 0 && terms.EarliestFirstReformedBudgetWeek >= 0)
                    Paragraph(ref y, T("ui.reform.earliest", MandatePresentation.Date(terms.EarliestActivationWeek), MandatePresentation.Date(terms.EarliestFirstReformedBudgetWeek)), small, 238, 12);
                else Paragraph(ref y, T("ui.reform.no_date"), small, 238, 12);
                Paragraph(ref y, T("ui.reform.completion", sponsor, Change(terms.CompletionRelationshipDelta)), small, 238, 12);
            }
            else if (terms.NextBudgetWeek >= 0)
                Paragraph(ref y, T("ui.reform.next_budget", MandatePresentation.Date(terms.NextBudgetWeek)), small, 238, 12);

            Paragraph(ref y, T("ui.reform.action_context", T("region." + terms.RegionId), T("reform.mode." + terms.ModeId)), body, 238, 10);
            Paragraph(ref y, T(closed ? "ui.reform.cost" : "ui.reform.paid", Number(terms.GoldCost), ResistanceNumber(terms.PowerCost)), body, 238, 10);
            Paragraph(ref y, T(closed ? "ui.reform.exit_proposed" : "ui.reform.exit_now", sponsor, Change(terms.EndRelationshipDelta), ResistanceNumber(CampaignCore.RegionalReformEndRelationshipLoss)), small, 238, 14);
            ActionResult check = closed ? reformBeginCheck : reformEndCheck;
            if (check != null && !check.Ok)
            {
                var refusal = new GUIStyle(small); refusal.normal.textColor = red;
                Paragraph(ref y, L.Text(check.Key, check.Args), refusal, 238, 10);
            }
            if (Press(new Rect(4, y, 238, 43), T(closed ? "ui.reform.begin" : "ui.reform.end"), check != null && check.Ok, true))
            {
                if (closed) app.BeginRegionalReform(terms.RegionId, terms.ModeId);
                else app.EndRegionalReform();
            }
            y += 55;
            documentContentHeight = y + 12;
        }
    }
}
