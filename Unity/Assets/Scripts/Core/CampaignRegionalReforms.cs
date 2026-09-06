using System;
using System.Globalization;

namespace PowerAboveAll
{
    public sealed class RegionalReformTerms
    {
        public string RegionId = "", ModeId = "", SponsorId = "", StatusId = "closed", WaitReasonKey = "";
        public string[] WaitReasonArgs = new string[0];
        public bool RegionReadyNow;
        public float RegionUnrest, RegionControl, PowerCost, SponsorRelationship, CompletionRelationshipDelta, EndRelationshipDelta;
        public int GoldCost, StepsRemaining;
        public int EarliestActivationWeek = -1, EarliestFirstReformedBudgetWeek = -1, NextBudgetWeek = -1;
        public int BaseTax, BaseFood, ReformedBaseTax, ReformedBaseFood, NominalTaxDelta, NominalFoodDelta;
        public int CurrentTaxIncome, CurrentProduction, CurrentNetFood;
        public int WithoutReformTaxIncome, WithoutReformProduction, WithReformTaxIncome, WithReformProduction;
        public int TaxIncomeDelta, ProductionDelta;
        public int WithoutReformForageFood, WithReformForageFood;
        public int WithoutReformNetFood, WithReformNetFood, NetFoodDelta;
    }

    public static partial class CampaignCore
    {
        public const int RegionalReformGoldCost = 120;
        public const float RegionalReformPowerCost = 4f;
        public const int RegionalReformPreparationWeeks = 4;
        public const float RegionalReformMinimumControl = 55f;
        public const float RegionalReformUnrestLimit = 65f;
        public const float RegionalReformCompletionRelationshipGain = 4f;
        public const float RegionalReformEndRelationshipLoss = 8f;

        private static bool KnownReformMode(string mode)
        { return mode == "provisioning" || mode == "commerce"; }
        private static string ReformSponsor(string mode)
        { return mode == "provisioning" ? "morel" : "valcourt"; }
        private static bool ReformRegionReady(RegionState region)
        { return region.Unrest < RegionalReformUnrestLimit && region.Control >= RegionalReformMinimumControl; }
        private static string ReformNumber(float value)
        { return value.ToString("0.##", CultureInfo.InvariantCulture); }

        public static bool HasRegionalReform(CampaignState state)
        { return state != null && !string.IsNullOrEmpty(state.ReformRegionId); }

        public static RegionalReformTerms GetRegionalReformTerms(CampaignState state)
        {
            if (!ValidRegionalReformCampaign(state)) return null;
            return BuildRegionalReformTerms(state, state.ReformRegionId, state.ReformModeId, false);
        }

        public static RegionalReformTerms GetRegionalReformTerms(CampaignState state, string regionId, string modeId)
        {
            if (!ValidRegionalReformCampaign(state) || HasRegionalReform(state) ||
                Definition(regionId) == null || !KnownReformMode(modeId)) return null;
            return BuildRegionalReformTerms(state, regionId, modeId, true);
        }

        private static RegionalReformTerms BuildRegionalReformTerms(CampaignState state, string regionId, string modeId, bool proposed)
        {
            var terms = new RegionalReformTerms {
                GoldCost = RegionalReformGoldCost, PowerCost = RegionalReformPowerCost,
                NextBudgetWeek = state.Week < MaximumWeek ? state.Week + 1 : -1
            };
            if (!string.IsNullOrEmpty(regionId))
            {
                var definition = Definition(regionId); var region = Region(state, regionId);
                var sponsor = Character(state, ReformSponsor(modeId));
                terms.RegionId = regionId; terms.ModeId = modeId; terms.SponsorId = sponsor.Id;
                terms.RegionUnrest = region.Unrest; terms.RegionControl = region.Control;
                terms.RegionReadyNow = ReformRegionReady(region);
                terms.StepsRemaining = proposed ? RegionalReformPreparationWeeks : state.ReformStepsRemaining;
                terms.StatusId = proposed ? "proposed" : terms.StepsRemaining == 0 ? "active" : terms.RegionReadyNow ? "pending" : "blocked";
                terms.BaseTax = definition.BaseTax; terms.BaseFood = definition.BaseFood;
                terms.ReformedBaseTax = ReformedTaxBase(definition, modeId);
                terms.ReformedBaseFood = ReformedFoodBase(definition, modeId);
                terms.NominalTaxDelta = terms.ReformedBaseTax - terms.BaseTax;
                terms.NominalFoodDelta = terms.ReformedBaseFood - terms.BaseFood;
                terms.SponsorRelationship = sponsor.Relationship;
                terms.CompletionRelationshipDelta = Clamp(sponsor.Relationship + RegionalReformCompletionRelationshipGain) - sponsor.Relationship;
                terms.EndRelationshipDelta = Clamp(sponsor.Relationship - RegionalReformEndRelationshipLoss) - sponsor.Relationship;
                if (terms.StepsRemaining > 0)
                {
                    long earliest = (long)state.Week + terms.StepsRemaining;
                    if (earliest <= MaximumWeek) terms.EarliestActivationWeek = (int)earliest;
                    if (earliest + 1 <= MaximumWeek) terms.EarliestFirstReformedBudgetWeek = (int)earliest + 1;
                    if (!terms.RegionReadyNow)
                    {
                        bool unrest = region.Unrest >= RegionalReformUnrestLimit, control = region.Control < RegionalReformMinimumControl;
                        terms.WaitReasonKey = "reform.wait." + (unrest && control ? "both" : unrest ? "unrest" : "control");
                        terms.WaitReasonArgs = new[] { ReformNumber(region.Unrest), ReformNumber(region.Control), ReformNumber(RegionalReformUnrestLimit), ReformNumber(RegionalReformMinimumControl) };
                    }
                }
            }
            var view = new EconomyView(state, HasRegionalAccord(state) ? state.AccordRegionId : null);
            var actual = BuildWeekProjection(view).Economy;
            var without = BuildWeekProjection(view.WithReform(null, null)).Economy;
            var with = BuildWeekProjection(view.WithReform(regionId, modeId)).Economy;
            terms.CurrentTaxIncome = actual.TaxIncome; terms.CurrentProduction = actual.Production; terms.CurrentNetFood = actual.NetFood;
            terms.WithoutReformTaxIncome = without.TaxIncome; terms.WithoutReformProduction = without.Production;
            terms.WithReformTaxIncome = with.TaxIncome; terms.WithReformProduction = with.Production;
            terms.TaxIncomeDelta = with.TaxIncome - without.TaxIncome; terms.ProductionDelta = with.Production - without.Production;
            terms.WithoutReformForageFood = without.ForageFood; terms.WithReformForageFood = with.ForageFood;
            terms.WithoutReformNetFood = without.NetFood; terms.WithReformNetFood = with.NetFood;
            terms.NetFoodDelta = with.NetFood - without.NetFood;
            return terms;
        }

        public static ActionResult CanBeginRegionalReform(CampaignState state, string regionId, string modeId)
        {
            if (!ValidRegionalReformCampaign(state)) return Result(false, "error.reform.state");
            if (Definition(regionId) == null) return Result(false, "error.region");
            if (!KnownReformMode(modeId)) return Result(false, "error.reform.mode");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (MandateDue(state)) return Result(false, "error.mandate.due");
            if (HasRegionalReform(state)) return Result(false, "error.reform.open");
            if (state.Week > MaximumWeek - RegionalReformPreparationWeeks - 1) return Result(false, "error.reform.calendar");
            if (state.Gold < RegionalReformGoldCost) return Result(false, "error.reform.gold", N(RegionalReformGoldCost));
            if (state.Power < RegionalReformPowerCost) return Result(false, "error.reform.power", ReformNumber(RegionalReformPowerCost));
            return Result(true, "log.reform.ready");
        }

        public static ActionResult BeginRegionalReform(CampaignState state, string regionId, string modeId)
        {
            var check = CanBeginRegionalReform(state, regionId, modeId); if (!check.Ok) return check;
            state.Gold -= RegionalReformGoldCost; state.Power -= RegionalReformPowerCost;
            state.ReformRegionId = regionId; state.ReformModeId = modeId; state.ReformStepsRemaining = RegionalReformPreparationWeeks;
            return Record(state, "log.reform.started", "region." + regionId, "reform.mode." + modeId,
                Character(state, ReformSponsor(modeId)).NameKey, N(RegionalReformGoldCost), ReformNumber(RegionalReformPowerCost), N(RegionalReformPreparationWeeks));
        }

        public static ActionResult CanEndRegionalReform(CampaignState state)
        {
            if (!ValidRegionalReformCampaign(state)) return Result(false, "error.reform.state");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (MandateDue(state)) return Result(false, "error.mandate.due");
            if (!HasRegionalReform(state)) return Result(false, "error.reform.none");
            return Result(true, "log.reform.ready");
        }

        public static ActionResult EndRegionalReform(CampaignState state)
        {
            var check = CanEndRegionalReform(state); if (!check.Ok) return check;
            string regionId = state.ReformRegionId, modeId = state.ReformModeId;
            string previous = state.ReformStepsRemaining == 0 ? "active" : ReformRegionReady(Region(state, regionId)) ? "pending" : "blocked";
            var sponsor = Character(state, ReformSponsor(modeId)); float before = sponsor.Relationship;
            sponsor.Relationship = Clamp(before - RegionalReformEndRelationshipLoss);
            state.ReformRegionId = state.ReformModeId = ""; state.ReformStepsRemaining = 0;
            return Record(state, "log.reform.ended", "region." + regionId, "reform.mode." + modeId,
                sponsor.NameKey, ReformNumber(sponsor.Relationship - before), "reform.status." + previous);
        }

        private static void CompleteRegionalReformAfterWeek(CampaignState state)
        {
            if (!HasRegionalReform(state) || state.ReformStepsRemaining == 0 || !ReformRegionReady(Region(state, state.ReformRegionId))) return;
            state.ReformStepsRemaining--;
            if (state.ReformStepsRemaining != 0)
            { Record(state, "log.reform.progress", "region." + state.ReformRegionId, N(state.ReformStepsRemaining)); return; }
            var sponsor = Character(state, ReformSponsor(state.ReformModeId)); float before = sponsor.Relationship;
            sponsor.Relationship = Clamp(before + RegionalReformCompletionRelationshipGain);
            Record(state, "log.reform.completed", "region." + state.ReformRegionId, "reform.mode." + state.ReformModeId,
                sponsor.NameKey, ReformNumber(sponsor.Relationship - before));
        }

        private static int ReformedTaxBase(RegionDefinition definition, string mode)
        {
            int share = Round(definition.BaseTax * .25d);
            return definition.BaseTax + (mode == "provisioning" ? -share : mode == "commerce" ? share : 0);
        }
        private static int ReformedFoodBase(RegionDefinition definition, string mode)
        {
            int share = Round(definition.BaseFood * .25d);
            return definition.BaseFood + (mode == "provisioning" ? share : mode == "commerce" ? -share : 0);
        }
        private static bool ValidRegionalReformCampaign(CampaignState state)
        {
            try { Validate(state); return true; }
            catch (ArgumentException) { return false; }
        }
        private static void ValidateRegionalReformState(CampaignState state)
        {
            Require(state.ReformRegionId != null && state.ReformModeId != null);
            Require(state.ReformStepsRemaining >= 0 && state.ReformStepsRemaining <= RegionalReformPreparationWeeks);
            if (state.ReformRegionId.Length == 0)
            { Require(state.ReformModeId.Length == 0 && state.ReformStepsRemaining == 0); return; }
            Require(Definition(state.ReformRegionId) != null && KnownReformMode(state.ReformModeId));
            // Adım sayısı geçmiş başarılı haftalardan fazla olamaz; bekleme geleceğe son tarih uydurmaz.
            Require(state.Week >= RegionalReformPreparationWeeks - state.ReformStepsRemaining);
        }
    }
}
