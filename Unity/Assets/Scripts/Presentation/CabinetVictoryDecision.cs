using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        private bool showVictory;
        private VictoryDecisionTerms victoryRecognition, victoryBonus;
        private ActionResult recognitionCheck, bonusCheck;

        public void CloseVictoryDecision() { showVictory = false; }

        private void ObserveVictoryDecision(CampaignState state)
        {
            victoryRecognition = CampaignCore.GetVictoryDecisionTerms(state, "recognize");
            victoryBonus = CampaignCore.GetVictoryDecisionTerms(state, "bonus");
            recognitionCheck = CampaignCore.CanResolveVictory(state, state.PendingVictoryId, "recognize");
            bonusCheck = CampaignCore.CanResolveVictory(state, state.PendingVictoryId, "bonus");
        }

        private void VictoryDecisionEntry(GameApp app, ref float y)
        {
            if (victoryRecognition == null) return;
            Paragraph(ref y, T("ui.victory.entry", T("region." + victoryRecognition.RegionId)), small, 238, 10);
            if (Press(new Rect(4, y, 238, 39), T("ui.victory.open"), true, true))
            { OpenDocument("victory"); app.Feedback("paper"); }
            y += 55;
        }

        private void CommanderPoliticalTerms(CharacterState commander, ref float y)
        {
            Pair(5, y, 236, T("ui.victory.loyalty"), AccordMeasure(commander.Loyalty)); y += 29;
            Pair(5, y, 236, T("ui.victory.ambition"), AccordMeasure(commander.Ambition)); y += 36;
            Paragraph(ref y, T(commander.Ambition > commander.Loyalty ? "ui.victory.commander_cost" : "ui.victory.commander_free"), small, 236, 19);
        }

        private void VictoryDecision(GameApp app)
        {
            if (victoryRecognition == null || victoryBonus == null) return;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            { CloseVictoryDecision(); Event.current.Use(); return; }
            CampaignState state = app.ViewState;
            var commander = state.Characters.Find(person => person.Id == "dumas");
            Fill(new Rect(0, 36, 1440, 864), new Color(.10f, .16f, .14f, .54f));
            Fill(new Rect(291, 150, 872, 624), new Color(.06f, .10f, .08f, .32f));
            Fill(new Rect(284, 142, 872, 624), paper);
            Fill(new Rect(284, 142, 872, 5), brass);
            Text(new Rect(311, 166, 610, 22), T("ui.victory.header"), small);
            Text(new Rect(311, 197, 680, 43), T("ui.victory.title"), title);
            Text(new Rect(311, 247, 680, 25), T("ui.victory.place", T("region." + victoryRecognition.RegionId)), body);
            Seal(new Rect(1034, 176, 89, 101), 3);
            if (Press(new Rect(982, 150, 62, 23), "RU", L.Language != "ru")) app.SetLanguage("ru");
            if (Press(new Rect(1050, 150, 62, 23), "TR", L.Language != "tr")) app.SetLanguage("tr");
            Text(new Rect(311, 284, 810, 52), T(victoryRecognition.PowerCost > 0 ? "ui.victory.comparison" : "ui.victory.comparison_free", AccordMeasure(commander.Loyalty),
                AccordMeasure(commander.Ambition), AccordMeasure(victoryRecognition.PowerCost)), body);
            VictoryChoice(app, new Rect(310, 344, 395, 278), victoryRecognition, recognitionCheck, commander);
            VictoryChoice(app, new Rect(721, 344, 395, 278), victoryBonus, bonusCheck, commander);
            Text(new Rect(311, 637, 805, 43), T("ui.victory.expiry"), small);
            var decline = CampaignCore.CanResolveVictory(state, victoryRecognition.BattleId, "decline");
            if (Press(new Rect(311, 692, 541, 40), T("ui.victory.decline"), decline.Ok))
                app.ResolveVictory(victoryRecognition.BattleId, "decline");
            if (Press(new Rect(868, 692, 248, 40), T("ui.victory.close")))
            { CloseVictoryDecision(); app.Feedback("paper"); }
        }

        private void VictoryChoice(GameApp app, Rect rect, VictoryDecisionTerms terms, ActionResult check, CharacterState commander)
        {
            bool recognize = terms.ChoiceId == "recognize";
            Fill(rect, recognize ? C("#E5DDC0") : C("#E1E4CF"));
            Fill(new Rect(rect.x, rect.y, rect.width, 3), recognize ? C("#C98270") : C("#83B0B6"));
            float x = rect.x + 15, width = rect.width - 30;
            Text(new Rect(x, rect.y + 15, width, 48), T(recognize ? "ui.victory.recognize_title" : "ui.victory.bonus_title"), heading);
            Text(new Rect(x, rect.y + 65, width, 40), T(recognize ? "ui.victory.recognize_note" : "ui.victory.bonus_note"), small);
            var state = app.ViewState;
            string effects = recognize
                ? T("ui.victory.recognize_effects", AccordMeasure(state.Fatigue), AccordMeasure(state.Fatigue + terms.FatigueDelta),
                    Change(terms.RelationshipDelta), Change(terms.AmbitionDelta), AccordMeasure(state.Power), AccordMeasure(state.Power - terms.PowerCost))
                : T("ui.victory.bonus_effects", Number(terms.GoldCost), AccordMeasure(commander.Loyalty), AccordMeasure(commander.Loyalty + terms.LoyaltyDelta),
                    Change(terms.ControlDelta));
            if (recognize && state.Power < terms.PowerCost)
                effects = T("ui.victory.recognize_unaffordable", AccordMeasure(state.Fatigue), AccordMeasure(state.Fatigue + terms.FatigueDelta),
                    Change(terms.RelationshipDelta), Change(terms.AmbitionDelta), AccordMeasure(terms.PowerCost), AccordMeasure(state.Power));
            Text(new Rect(x, rect.y + 115, width, 74), effects, body);
            if (Press(new Rect(x, rect.y + 198, width, 40), T(recognize ? "ui.victory.recognize" : "ui.victory.bonus"), check.Ok, true))
                app.ResolveVictory(terms.BattleId, terms.ChoiceId);
            if (!check.Ok)
            {
                var warning = new GUIStyle(small); warning.normal.textColor = red;
                Text(new Rect(x, rect.y + 245, width, 31), L.Text(check.Key, check.Args), warning);
            }
        }
    }
}
