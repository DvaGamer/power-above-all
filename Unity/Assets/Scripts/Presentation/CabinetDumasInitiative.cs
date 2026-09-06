using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private DumasInitiativeTerms foragePreview;
        private ActionResult forageVetoCheck;

        private void ObserveDumasInitiative(CampaignState state)
        {
            foragePreview = CampaignCore.GetDumasInitiativeTerms(state);
            forageVetoCheck = foragePreview == null ? null : CampaignCore.CanVetoDumasInitiative(state, foragePreview.DueWeek);
        }

        private void DumasInitiativeNotice(GameApp app)
        {
            Fill(new Rect(265, 862, 3, 26), C("#C98270"));
            string notice = T("ui.forage.notice", MandatePresentation.Date(foragePreview.DueWeek));
            if (GUI.Button(new Rect(278, 864, 817, 24), notice, lightBody))
            { OpenDocument("initiative"); app.Feedback("paper"); }
        }

        private void DumasInitiativeEntry(GameApp app, ref float y)
        {
            if (foragePreview == null) return;
            Paragraph(ref y, T("ui.forage.entry", T("region." + foragePreview.RegionId), MandatePresentation.Date(foragePreview.DueWeek)), body, 238, 10);
            if (Press(new Rect(4, y, 238, 39), T("ui.forage.open"), true, true))
            { OpenDocument("initiative"); app.Feedback("paper"); }
            y += 55;
        }

        private void DumasInitiative(GameApp app)
        {
            float y = 0;
            if (Press(new Rect(4, y, 238, 30), T("ui.accord.back")))
            { OpenDocument("council"); app.Feedback("paper"); }
            y += 45;
            Paragraph(ref y, T("ui.forage.title"), heading, 238, 15);
            var terms = foragePreview;
            if (terms == null)
            {
                Paragraph(ref y, T("ui.forage.none"), body, 238, 16);
                if (Press(new Rect(4, y, 238, 38), T("ui.forage.journal"))) OpenDocument("journal");
                documentContentHeight = y + 58; return;
            }
            Seal(new Rect(2, y, 82, 96), 3);
            string name = T("character.dumas.name");
            float nameHeight = body.CalcHeight(new GUIContent(name), 146);
            Text(new Rect(96, y + 4, 146, nameHeight), name, body);
            Text(new Rect(96, y + nameHeight + 12, 146, 66), T("ui.forage.author"), small);
            y += Mathf.Max(112, nameHeight + 86);
            Paragraph(ref y, T("ui.forage.voice"), body, 238, 16);
            Paragraph(ref y, T("ui.forage.due", MandatePresentation.Date(terms.DueWeek)), body, 238, 10);
            Paragraph(ref y, T("ui.forage.camp", T("region." + terms.RegionId)), small, 238, 16);
            Rule(4, y, 238); y += 17;
            Paragraph(ref y, T("ui.forage.forecast"), heading, 238, 12);
            if (terms.Disposition != "gather")
                Paragraph(ref y, L.Text(terms.ReasonKey, terms.ReasonArgs), body, 238, 14);
            if (terms.Disposition == "gather")
            {
                LedgerLine(ref y, T("ui.forage.economy"), terms.FoodGathered, true);
                Paragraph(ref y, T("ui.forage.local", Change(terms.UnrestDelta), Change(terms.EliteLoyaltyDelta)), body, 238, 12);
                Paragraph(ref y, T("ui.forage.political", Change(terms.AmbitionDelta), Change(-terms.PowerCost)), body, 238, 12);
                Paragraph(ref y, T("ui.forage.payroll_separate"), small, 238, 16);
            }
            if (terms.RegionId != app.State.SelectedRegionId)
            {
                if (Press(new Rect(4, y, 238, 38), T("ui.forage.show_camp"))) app.SelectRegion(terms.RegionId);
                y += 52;
            }
            Rule(4, y, 238); y += 17;
            Paragraph(ref y, T("ui.forage.intervene"), heading, 238, 12);
            Paragraph(ref y, T("ui.forage.intervene_body"), body, 238, 14);
            Paragraph(ref y, T("ui.forage.veto_cost", Change(terms.VetoRelationshipDelta)), body, 238, 14);
            if (forageVetoCheck != null && !forageVetoCheck.Ok)
            {
                var warning = new GUIStyle(small); warning.normal.textColor = red;
                Paragraph(ref y, L.Text(forageVetoCheck.Key, forageVetoCheck.Args), warning, 238, 12);
            }
            if (Press(new Rect(4, y, 238, 45), T("ui.forage.veto"), forageVetoCheck != null && forageVetoCheck.Ok, true))
                app.VetoDumasInitiative(terms.DueWeek);
            y += 61;
            documentContentHeight = y + 12;
        }
    }
}
