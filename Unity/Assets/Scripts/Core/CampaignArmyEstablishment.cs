using System;
using System.Globalization;

namespace PowerAboveAll
{
    public sealed class ArmyEstablishmentTerms
    {
        public string PolicyId, Disposition, ReasonKey;
        public string[] ReasonArgs;
        public int CurrentTroops, TargetTroops, DueWeek, WeeksRemaining, FirstReducedBudgetWeek;
        public int ExcessTroops, NextBatchTroops, TroopsAfterBatch, ManpowerAfterBatch;
        public int CurrentArmyCost, ArmyCostAfterBatch, CurrentArmyConsumption, ArmyConsumptionAfterBatch;
        public float DumasRelationshipDelta;
        public bool WillRemoveGarrison;
    }

    public static partial class CampaignCore
    {
        public const int ArmyReductionWeeks = 2;
        public const int ArmyReductionBatch = 200;
        public const int MaximumArmyTarget = MaximumStock;

        // Bugünkü koşullarda yalnızca ordunun gideri; bütün ülkenin iki haftalık tahmini değildir.
        private static int ArmyCostFor(CampaignState state, int troops)
        { return (int)Math.Ceiling(troops / 12d) + (troops > 0 || state.MilitarySupplies < 120 ? 36 : 0); }
        internal static int ArmyFoodFor(int troops)
        { return (int)Math.Ceiling(troops / 30d); }

        public static bool HasArmyEstablishment(CampaignState state)
        { return ValidArmyEstablishmentCampaign(state) && state.ArmyPolicyId == "budget"; }

        public static ArmyEstablishmentTerms GetArmyEstablishmentTerms(CampaignState state)
        {
            if (!ValidArmyEstablishmentCampaign(state)) return null;
            return BuildArmyEstablishmentTerms(state, state.ArmyPolicyId, state.ArmyTargetTroops);
        }

        public static ArmyEstablishmentTerms GetArmyEstablishmentTerms(CampaignState state, string policyId, int targetTroops)
        {
            if (!ValidArmyEstablishmentCampaign(state) || !ArmyPolicy(policyId) || !ArmyTarget(policyId, targetTroops)) return null;
            if (policyId == "budget" && (long)state.Manpower + Math.Max(0, state.Troops - targetTroops) > MaximumStock) return null;
            return BuildArmyEstablishmentTerms(state, policyId, targetTroops);
        }

        private static ArmyEstablishmentTerms BuildArmyEstablishmentTerms(CampaignState state, string policyId, int targetTroops)
        {
            int excess = policyId == "budget" ? Math.Max(0, state.Troops - targetTroops) : 0;
            int due = ArmyDueFor(state, policyId, targetTroops);
            int batch = due > 0 ? Math.Min(ArmyReductionBatch, excess) : 0;
            var general = Character(state, "dumas");
            string disposition = policyId == "campaign" ? "campaign" : excess == 0 ? "at_target" : due > 0 ? "scheduled" : "calendar";
            return new ArmyEstablishmentTerms {
                PolicyId = policyId, Disposition = disposition,
                ReasonKey = "establishment.reason." + disposition, ReasonArgs = new string[0],
                CurrentTroops = state.Troops, TargetTroops = targetTroops, DueWeek = due,
                WeeksRemaining = due > 0 ? due - state.Week : 0,
                FirstReducedBudgetWeek = due > 0 && due < MaximumWeek ? due + 1 : 0,
                ExcessTroops = excess, NextBatchTroops = batch, TroopsAfterBatch = state.Troops - batch,
                ManpowerAfterBatch = state.Manpower + batch,
                CurrentArmyCost = ArmyCostFor(state, state.Troops), ArmyCostAfterBatch = ArmyCostFor(state, state.Troops - batch),
                CurrentArmyConsumption = ArmyFoodFor(state.Troops), ArmyConsumptionAfterBatch = ArmyFoodFor(state.Troops - batch),
                DumasRelationshipDelta = batch > 0 ? Clamp(general.Relationship - 4) - general.Relationship : 0,
                WillRemoveGarrison = batch > 0 && state.Troops == batch
            };
        }

        public static ActionResult CanSetArmyEstablishment(CampaignState state, string policyId, int targetTroops)
        {
            if (!ValidArmyEstablishmentCampaign(state)) return Result(false, "error.establishment.state");
            if (!ArmyPolicy(policyId)) return Result(false, "error.establishment.policy");
            if (!ArmyTarget(policyId, targetTroops)) return Result(false, "error.establishment.target");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (MandateDue(state)) return Result(false, "error.mandate.due");
            if (state.Week >= MaximumWeek) return Result(false, "error.week.limit");
            if (state.ArmyPolicyId == policyId && state.ArmyTargetTroops == targetTroops)
                return Result(false, "error.establishment.unchanged");
            if (policyId == "budget")
            {
                if (state.DumasOfficerCommission) return Result(false, "error.establishment.commission");
                int excess = Math.Max(0, state.Troops - targetTroops);
                if ((long)state.Manpower + excess > MaximumStock) return Result(false, "error.establishment.capacity");
                if (excess > 0 && ArmyDueFor(state, policyId, targetTroops) == 0)
                    return Result(false, "error.establishment.calendar");
            }
            return Result(true, "log.establishment.ready");
        }

        public static ActionResult SetArmyEstablishment(CampaignState state, string policyId, int targetTroops)
        {
            var check = CanSetArmyEstablishment(state, policyId, targetTroops);
            if (!check.Ok) return check;
            int due = ArmyDueFor(state, policyId, targetTroops);
            state.ArmyPolicyId = policyId; state.ArmyTargetTroops = targetTroops; state.ArmyReductionDueWeek = due;
            if (policyId == "campaign") return Record(state, "log.establishment.campaign");
            if (due > 0) return Record(state, "log.establishment.budget_scheduled", N(targetTroops), N(due));
            return Record(state, "log.establishment.budget_idle", N(targetTroops));
        }

        private static bool ArmyPolicy(string policyId)
        { return policyId == "campaign" || policyId == "budget"; }

        private static bool ArmyTarget(string policyId, int targetTroops)
        { return targetTroops >= 0 && targetTroops <= MaximumStock && (policyId != "campaign" || targetTroops == 0); }

        private static int ArmyDueFor(CampaignState state, string policyId, int targetTroops)
        {
            if (policyId != "budget" || state.Troops <= targetTroops) return 0;
            if (state.ArmyPolicyId == "budget" && state.ArmyReductionDueWeek > 0) return state.ArmyReductionDueWeek;
            return state.Week <= MaximumWeek - ArmyReductionWeeks ? state.Week + ArmyReductionWeeks : 0;
        }

        // Kayıplar henüz ayrılmamış askeri de azaltır. Yeni asker alımı mevcut tarihi değiştirmez.
        private static void RefreshArmyReduction(CampaignState state)
        { state.ArmyReductionDueWeek = ArmyDueFor(state, state.ArmyPolicyId, state.ArmyTargetTroops); }

        private static void CompleteArmyReductionAfterWeek(CampaignState state)
        {
            if (state.ArmyPolicyId != "budget") return;
            if (state.ArmyReductionDueWeek != 0 && state.ArmyReductionDueWeek <= state.Week)
            {
                int batch = Math.Min(ArmyReductionBatch, Math.Max(0, state.Troops - state.ArmyTargetTroops));
                state.ArmyReductionDueWeek = 0;
                if (batch > 0)
                {
                    // Tüm fazla kuvvetin yeri politika imzalanırken ayrıldı; sessiz Stock kırpması yoktur.
                    state.Troops -= batch; state.Manpower += batch;
                    var general = Character(state, "dumas");
                    float relationshipLoss = general.Relationship - Clamp(general.Relationship - 4);
                    general.Relationship -= relationshipLoss;
                    Record(state, "log.establishment.reduced", N(batch), N(state.Troops),
                        relationshipLoss.ToString("0.##", CultureInfo.InvariantCulture), N(state.ArmyTargetTroops));
                }
            }
            RefreshArmyReduction(state);
        }

        private static bool ValidArmyEstablishmentCampaign(CampaignState state)
        {
            try { Validate(state); return true; }
            catch (ArgumentException) { return false; }
        }

        private static void ValidateArmyEstablishmentState(CampaignState state)
        {
            Require(ArmyPolicy(state.ArmyPolicyId) && ArmyTarget(state.ArmyPolicyId, state.ArmyTargetTroops));
            Require(!state.DumasOfficerCommission || state.ArmyPolicyId == "campaign");
            if (state.ArmyPolicyId == "campaign") { Require(state.ArmyReductionDueWeek == 0); return; }
            int excess = Math.Max(0, state.Troops - state.ArmyTargetTroops);
            Require((long)state.Manpower + excess <= MaximumStock);
            if (state.ArmyReductionDueWeek == 0)
            {
                // Son iki haftada yeniden alınan asker veya son grubun kalanı için yeni süre kurulamaz.
                Require(excess == 0 || state.Week > MaximumWeek - ArmyReductionWeeks);
                return;
            }
            Require(excess > 0 && state.ArmyReductionDueWeek >= ArmyReductionWeeks &&
                state.ArmyReductionDueWeek > state.Week && state.ArmyReductionDueWeek <= MaximumWeek &&
                state.ArmyReductionDueWeek <= state.Week + ArmyReductionWeeks);
        }
    }
}
