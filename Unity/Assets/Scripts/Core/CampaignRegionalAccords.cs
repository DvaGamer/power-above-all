using System;

namespace PowerAboveAll
{
    public sealed class RegionalAccordTerms
    {
        public string RegionId;
        public int UntilWeek, RemainingWeeks, CurrentTaxIncome, ProjectedTaxIncome, TaxForgone;
        public bool IsActive;
        public MandateEffect Immediate, Fulfil, Break;
    }

    public static partial class CampaignCore
    {
        public const int RegionalAccordWeeks = 4;

        public static bool HasRegionalAccord(CampaignState state)
        { return state != null && !string.IsNullOrEmpty(state.AccordRegionId) && state.Week < state.AccordUntilWeek; }

        public static bool TaxBreaksRegionalAccord(CampaignState state, string regionId)
        { return HasRegionalAccord(state) && regionId == state.AccordRegionId; }

        private static RegionalAccordTerms BuildRegionalAccordTerms(string regionId, int untilWeek, int currentWeek)
        {
            return new RegionalAccordTerms {
                RegionId = regionId, UntilWeek = untilWeek, RemainingWeeks = untilWeek - currentWeek,
                Immediate = new MandateEffect { Unrest = -10, Control = 3 },
                Fulfil = new MandateEffect { FactionId = "assembly", Approval = 5, CharacterId = "morel", Relationship = 4 },
                Break = new MandateEffect { Unrest = 10, Control = -3, Power = -4, FactionId = "assembly", Approval = -10, CharacterId = "morel", Relationship = -10 }
            };
        }

        public static RegionalAccordTerms GetRegionalAccordTerms(CampaignState state, string regionId)
        {
            if (!ValidRegionalAccordCampaign(state) || Definition(regionId) == null || HasRegionalAccord(state) ||
                state.Week > MaximumWeek - RegionalAccordWeeks) return null;
            var terms = BuildRegionalAccordTerms(regionId, state.Week + RegionalAccordWeeks, state.Week);
            terms.CurrentTaxIncome = Forecast(state).TaxIncome;
            var plan = BuildWeekProjection(new EconomyView(state, regionId, regionId, terms.Immediate.Unrest, terms.Immediate.Control));
            terms.ProjectedTaxIncome = plan.Economy.TaxIncome;
            int withoutHoliday = CalculateEconomy(plan.View.WithExemption(null)).TaxIncome;
            terms.TaxForgone = withoutHoliday - terms.ProjectedTaxIncome;
            return terms;
        }

        public static RegionalAccordTerms GetActiveRegionalAccordTerms(CampaignState state)
        {
            if (!ValidRegionalAccordCampaign(state) || !HasRegionalAccord(state)) return null;
            var terms = BuildRegionalAccordTerms(state.AccordRegionId, state.AccordUntilWeek, state.Week);
            terms.IsActive = true;
            var plan = BuildWeekProjection(new EconomyView(state, state.AccordRegionId));
            terms.CurrentTaxIncome = terms.ProjectedTaxIncome = plan.Economy.TaxIncome;
            terms.TaxForgone = CalculateEconomy(plan.View.WithExemption(null)).TaxIncome - terms.CurrentTaxIncome;
            return terms;
        }

        public static ActionResult CanGrantRegionalAccord(CampaignState state, string regionId)
        {
            if (!ValidRegionalAccordCampaign(state)) return Result(false, "error.accord.state");
            if (Definition(regionId) == null) return Result(false, "error.region");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (MandateDue(state)) return Result(false, "error.mandate.due");
            if (HasRegionalAccord(state)) return Result(false, "error.accord.active", "region." + state.AccordRegionId);
            if (state.Week < state.AccordUntilWeek) return Result(false, "error.accord.cooldown", N(state.AccordUntilWeek - state.Week));
            if (state.Week > MaximumWeek - RegionalAccordWeeks) return Result(false, "error.accord.calendar");
            return Result(true, "log.accord.ready");
        }

        public static ActionResult GrantRegionalAccord(CampaignState state, string regionId)
        {
            var check = CanGrantRegionalAccord(state, regionId);
            if (!check.Ok) return check;
            var terms = BuildRegionalAccordTerms(regionId, state.Week + RegionalAccordWeeks, state.Week);
            ApplyMandateEffect(state, regionId, terms.Immediate);
            state.AccordRegionId = regionId;
            state.AccordUntilWeek = terms.UntilWeek;
            return Record(state, "log.accord.granted", "region." + regionId, N(RegionalAccordWeeks));
        }

        // Çağrı tax emrinin bütün mevcut ret koşullarından sonra yapılır.
        private static void BreakRegionalAccordForTax(CampaignState state, string regionId)
        {
            if (!TaxBreaksRegionalAccord(state, regionId)) return;
            var terms = BuildRegionalAccordTerms(regionId, state.AccordUntilWeek, state.Week);
            ApplyMandateEffect(state, regionId, terms.Break);
            state.AccordRegionId = "";
            // İlk imzanın tarihi kalır: iptal, aynı haftada yeni taviz üretmez.
            Record(state, "log.accord.broken", "region." + regionId);
        }

        private static void CompleteRegionalAccordAfterWeek(CampaignState state)
        {
            if (string.IsNullOrEmpty(state.AccordRegionId) || state.Week < state.AccordUntilWeek) return;
            string regionId = state.AccordRegionId;
            var terms = BuildRegionalAccordTerms(regionId, state.AccordUntilWeek, state.Week);
            ApplyMandateEffect(state, regionId, terms.Fulfil);
            state.AccordRegionId = "";
            Record(state, "log.accord.completed", "region." + regionId);
        }

        private static bool ValidRegionalAccordCampaign(CampaignState state)
        {
            try { Validate(state); return true; }
            catch (ArgumentException) { return false; }
        }

        private static void ValidateRegionalAccordState(CampaignState state)
        {
            Require(state.AccordRegionId != null);
            int until = state.AccordUntilWeek;
            Require(until == 0 || (until >= RegionalAccordWeeks && until <= MaximumWeek && until <= (long)state.Week + RegionalAccordWeeks));
            if (state.AccordRegionId.Length == 0) return;
            Require(Definition(state.AccordRegionId) != null && state.Week < until && state.Week >= until - RegionalAccordWeeks);
        }
    }
}
