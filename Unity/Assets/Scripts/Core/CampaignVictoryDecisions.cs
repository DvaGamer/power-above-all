using System;
using System.Globalization;

namespace PowerAboveAll
{
    public sealed class VictoryDecisionTerms
    {
        public string BattleId, RegionId, ChoiceId;
        public int GoldCost;
        public float PowerCost, FatigueDelta, RelationshipDelta, AmbitionDelta, LoyaltyDelta, ControlDelta;
    }

    public static partial class CampaignCore
    {
        public static bool HasPendingVictory(CampaignState state)
        { return ValidVictoryCampaign(state) && state.PendingVictoryId.Length > 0; }

        public static VictoryDecisionTerms GetVictoryDecisionTerms(CampaignState state, string choiceId)
        {
            if (!HasPendingVictory(state) || !VictoryChoice(choiceId)) return null;
            string regionId = state.PendingVictoryId.Split('-')[4];
            var general = Character(state, "dumas");
            var terms = new VictoryDecisionTerms { BattleId = state.PendingVictoryId, RegionId = regionId, ChoiceId = choiceId };
            if (choiceId == "recognize")
            {
                terms.PowerCost = general.Ambition > general.Loyalty ? 4 : 0;
                terms.FatigueDelta = Clamp(state.Fatigue - 12) - state.Fatigue;
                terms.RelationshipDelta = Clamp(general.Relationship + 4) - general.Relationship;
                terms.AmbitionDelta = Clamp(general.Ambition + 3) - general.Ambition;
            }
            else if (choiceId == "bonus")
            {
                terms.GoldCost = (int)Math.Ceiling(state.Troops / 12d);
                terms.LoyaltyDelta = Clamp(general.Loyalty + 5) - general.Loyalty;
                float control = Region(state, regionId).Control;
                terms.ControlDelta = Clamp(control + 3) - control;
            }
            return terms;
        }

        public static ActionResult CanResolveVictory(CampaignState state, string expectedBattleId, string choiceId)
        {
            if (!ValidVictoryCampaign(state)) return Result(false, "error.victory.state");
            if (state.PendingVictoryId.Length == 0) return Result(false, "error.victory.none");
            if (expectedBattleId != state.PendingVictoryId) return Result(false, "error.victory.stale");
            if (!VictoryChoice(choiceId)) return Result(false, "error.victory.choice");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (MandateDue(state)) return Result(false, "error.mandate.due");
            var terms = GetVictoryDecisionTerms(state, choiceId);
            if (choiceId == "bonus" && terms.LoyaltyDelta <= 0 && terms.ControlDelta <= 0)
                return Result(false, "error.victory.bonus_complete");
            if (state.Gold < terms.GoldCost) return Result(false, "error.victory.gold", N(terms.GoldCost));
            if (state.Power < terms.PowerCost) return Result(false, "error.victory.power", VictoryNumber(terms.PowerCost));
            return Result(true, "log.victory.ready");
        }

        public static ActionResult ResolveVictory(CampaignState state, string expectedBattleId, string choiceId)
        {
            var check = CanResolveVictory(state, expectedBattleId, choiceId);
            if (!check.Ok) return check;
            var terms = GetVictoryDecisionTerms(state, choiceId);
            var general = Character(state, "dumas");
            state.Gold -= terms.GoldCost;
            state.Power -= terms.PowerCost;
            state.Fatigue = Clamp(state.Fatigue + terms.FatigueDelta);
            general.Relationship = Clamp(general.Relationship + terms.RelationshipDelta);
            general.Ambition = Clamp(general.Ambition + terms.AmbitionDelta);
            general.Loyalty = Clamp(general.Loyalty + terms.LoyaltyDelta);
            var region = Region(state, terms.RegionId);
            region.Control = Clamp(region.Control + terms.ControlDelta);
            state.PendingVictoryId = "";
            if (choiceId == "recognize")
                return Record(state, "log.victory.recognize", "region." + terms.RegionId, VictoryNumber(terms.PowerCost),
                    VictoryNumber(-terms.FatigueDelta), VictoryNumber(terms.RelationshipDelta), VictoryNumber(terms.AmbitionDelta));
            if (choiceId == "bonus")
                return Record(state, "log.victory.bonus", "region." + terms.RegionId, N(terms.GoldCost),
                    VictoryNumber(terms.LoyaltyDelta), VictoryNumber(terms.ControlDelta));
            return Record(state, "log.victory.decline", "region." + terms.RegionId);
        }

        private static bool VictoryChoice(string choiceId)
        { return choiceId == "recognize" || choiceId == "bonus" || choiceId == "decline"; }

        private static string VictoryNumber(float value)
        { return value.ToString("0.##", CultureInfo.InvariantCulture); }

        private static bool ValidVictoryCampaign(CampaignState state)
        {
            try { Validate(state); return true; }
            catch (ArgumentException) { return false; }
        }

        private static void ValidateVictoryDecisionState(CampaignState state)
        {
            Require(state.PendingVictoryId != null);
            if (state.PendingVictoryId.Length == 0) return;
            Require(state.ResolvedBattles.Count > 0 && state.ResolvedBattles[state.ResolvedBattles.Count - 1] == state.PendingVictoryId);
            // ValidateBase bu çözülmüş savaş kimliğinin biçimini ve komşuluğunu zaten denetledi.
            var parts = state.PendingVictoryId.Split('-');
            int departureMoves = int.Parse(parts[2], CultureInfo.InvariantCulture);
            Require(parts[1] == N(state.Week) && state.ArmyRegionId == parts[4] && state.Troops > 0);
            // Düşman bölgenin huzursuzluğu en az65'tir: her gerçek savaş iki hareket tüketir.
            Require(state.Moves == Math.Max(0, departureMoves - 2));
        }
    }
}
