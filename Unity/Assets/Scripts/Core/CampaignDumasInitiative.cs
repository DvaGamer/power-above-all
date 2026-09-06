using System;
using System.Globalization;

namespace PowerAboveAll
{
    public sealed class DumasInitiativeTerms
    {
        public string RegionId, Disposition, ReasonKey;
        public string[] ReasonArgs = new string[0];
        public int DueWeek, NextForageWeek, FoodGathered, FoodShortfall;
        public float UnrestDelta, EliteLoyaltyDelta, AmbitionDelta, PowerCost, VetoRelationshipDelta;
    }

    public static partial class CampaignCore
    {
        public const int DumasForageLimit = 40;
        public const int DumasForageCooldown = 4;

        // Küçük salt okunur görünüm; senaryo ve girişim gerçek state'i değiştirmez.
        private sealed class EconomyView
        {
            public readonly CampaignState State;
            public readonly string ExemptRegion;
            private readonly string adjustedRegion, forageRegion;
            private readonly float unrestDelta, controlDelta;
            public EconomyView(CampaignState state, string exemptRegion, string adjustedRegion = null,
                float unrestDelta = 0, float controlDelta = 0, string forageRegion = null)
            {
                State = state; ExemptRegion = exemptRegion; this.adjustedRegion = adjustedRegion;
                this.unrestDelta = unrestDelta; this.controlDelta = controlDelta; this.forageRegion = forageRegion;
            }
            public float Unrest(RegionState region)
            {
                float value = region.Id == adjustedRegion ? Clamp(region.Unrest + unrestDelta) : region.Unrest;
                return region.Id == forageRegion ? Clamp(value + 8) : value;
            }
            public float Control(RegionState region)
            { return region.Id == adjustedRegion ? Clamp(region.Control + controlDelta) : region.Control; }
            public EconomyView WithForage(string regionId)
            { return new EconomyView(State, ExemptRegion, adjustedRegion, unrestDelta, controlDelta, regionId); }
            public EconomyView WithExemption(string regionId)
            { return new EconomyView(State, regionId, adjustedRegion, unrestDelta, controlDelta, forageRegion); }
        }

        private sealed class WeekProjection
        {
            public EconomyView View;
            public EconomyForecast Economy;
            public DumasInitiativeTerms Initiative;
        }

        private static WeekProjection BuildWeekProjection(EconomyView view)
        {
            var state = view.State;
            var plan = new WeekProjection { View = view, Economy = CalculateEconomy(view) };
            if (state.DumasForageDueWeek == 0) return plan;
            var general = Character(state, "dumas");
            var region = Region(state, state.ArmyRegionId);
            var terms = new DumasInitiativeTerms {
                RegionId = state.ArmyRegionId, DueWeek = state.DumasForageDueWeek,
                NextForageWeek = state.DumasNextForageWeek,
                VetoRelationshipDelta = Clamp(general.Relationship - 4) - general.Relationship,
                FoodShortfall = FoodShortfall(state, plan.Economy)
            };
            plan.Initiative = terms;
            if (state.Troops <= 0)
            { SetDumasDisposition(terms, "no_army"); return plan; }
            if (terms.FoodShortfall == 0)
            { SetDumasDisposition(terms, "sufficient"); return plan; }
            var candidateView = view.WithForage(state.ArmyRegionId);
            var candidate = CalculateEconomy(candidateView);
            terms.FoodShortfall = FoodShortfall(state, candidate);
            if (terms.FoodShortfall > DumasForageLimit)
            { SetDumasDisposition(terms, "too_large", N(terms.FoodShortfall), N(DumasForageLimit)); return plan; }
            // Yerel üretim kaybından sonraki açığın tamamı kapanır; kısmi yardımın cezası yoktur.
            terms.FoodGathered = terms.FoodShortfall;
            terms.UnrestDelta = candidateView.Unrest(region) - view.Unrest(region);
            terms.EliteLoyaltyDelta = Clamp(region.EliteLoyalty - 6) - region.EliteLoyalty;
            terms.AmbitionDelta = Clamp(general.Ambition + 3) - general.Ambition;
            terms.PowerCost = general.Ambition > general.Loyalty ? Math.Min(state.Power, 4) : 0;
            SetDumasDisposition(terms, "gather", N(terms.FoodGathered), "region." + terms.RegionId);
            candidate.ForageFood = terms.FoodGathered;
            candidate.NetFood += terms.FoodGathered;
            plan.View = candidateView; plan.Economy = candidate;
            return plan;
        }

        private static int FoodShortfall(CampaignState state, EconomyForecast economy)
        { return (int)Math.Max(0L, -((long)state.Food + economy.NetFood)); }

        private static void SetDumasDisposition(DumasInitiativeTerms terms, string disposition, params string[] args)
        { terms.Disposition = disposition; terms.ReasonKey = "dumas.reason." + disposition; terms.ReasonArgs = args; }

        public static bool HasDumasInitiative(CampaignState state)
        { return ValidDumasCampaign(state) && state.DumasForageDueWeek != 0; }

        public static DumasInitiativeTerms GetDumasInitiativeTerms(CampaignState state)
        {
            if (!HasDumasInitiative(state)) return null;
            return BuildWeekProjection(new EconomyView(state, HasRegionalAccord(state) ? state.AccordRegionId : null)).Initiative;
        }

        public static ActionResult CanVetoDumasInitiative(CampaignState state, int expectedDueWeek)
        {
            if (!ValidDumasCampaign(state)) return Result(false, "error.dumas.state");
            if (state.DumasForageDueWeek == 0) return Result(false, "error.dumas.none");
            if (expectedDueWeek != state.DumasForageDueWeek) return Result(false, "error.dumas.stale");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (MandateDue(state)) return Result(false, "error.mandate.due");
            return Result(true, "log.dumas.veto_ready");
        }

        public static ActionResult VetoDumasInitiative(CampaignState state, int expectedDueWeek)
        {
            var check = CanVetoDumasInitiative(state, expectedDueWeek);
            if (!check.Ok) return check;
            var general = Character(state, "dumas");
            float before = general.Relationship;
            general.Relationship = Clamp(before - 4);
            state.DumasForageDueWeek = 0;
            return Record(state, "log.dumas.vetoed", "region." + state.ArmyRegionId, DumasNumber(before - general.Relationship));
        }

        private static void ApplyDumasInitiative(CampaignState state, DumasInitiativeTerms terms)
        {
            if (terms == null) return;
            state.DumasForageDueWeek = 0;
            if (terms.Disposition == "gather")
            {
                var region = Region(state, terms.RegionId);
                region.Unrest = Clamp(region.Unrest + terms.UnrestDelta);
                region.EliteLoyalty = Clamp(region.EliteLoyalty + terms.EliteLoyaltyDelta);
                var general = Character(state, "dumas");
                general.Ambition = Clamp(general.Ambition + terms.AmbitionDelta);
                state.Power = Clamp(state.Power - terms.PowerCost);
                // Food ayrıca eklenmez: aynı planın NetFood bileşeninde zaten bir kez bulunur.
            }
        }

        // Uygulama öncesi etkiler hesaplanır; rapor ancak ilan edilen yeni haftada kaydedilir.
        private static void RecordDumasInitiative(CampaignState state, DumasInitiativeTerms terms)
        {
            if (terms == null) return;
            if (terms.Disposition == "gather")
            {
                Record(state, "log.dumas.gathered", "region." + terms.RegionId, N(terms.FoodGathered),
                    DumasNumber(terms.UnrestDelta), DumasNumber(-terms.EliteLoyaltyDelta),
                    DumasNumber(terms.AmbitionDelta), DumasNumber(terms.PowerCost));
            }
            else if (terms.Disposition == "too_large")
                Record(state, "log.dumas.too_large", "region." + terms.RegionId, N(terms.FoodShortfall), N(DumasForageLimit));
            else Record(state, "log.dumas." + terms.Disposition);
        }

        private static void AnnounceDumasInitiativeAfterWeek(CampaignState state, bool hunger)
        {
            if (!hunger || state.Troops <= 0 || state.DumasForageDueWeek != 0 ||
                state.Week < state.DumasNextForageWeek || state.Week > MaximumWeek - DumasForageCooldown) return;
            state.DumasForageDueWeek = state.Week + 1;
            state.DumasNextForageWeek = state.Week + DumasForageCooldown;
            Record(state, "log.dumas.announced", "region." + state.ArmyRegionId, N(state.DumasForageDueWeek), N(DumasForageLimit));
        }

        private static string DumasNumber(float value)
        { return value.ToString("0.##", CultureInfo.InvariantCulture); }

        private static bool ValidDumasCampaign(CampaignState state)
        {
            try { Validate(state); return true; }
            catch (ArgumentException) { return false; }
        }

        private static void ValidateDumasInitiativeState(CampaignState state)
        {
            int due = state.DumasForageDueWeek, next = state.DumasNextForageWeek;
            Require(due >= 0 && due <= MaximumWeek);
            Require(next == 0 || (next >= 1 + DumasForageCooldown && next <= MaximumWeek &&
                next <= (long)state.Week + DumasForageCooldown));
            if (due != 0)
                Require(due == state.Week + 1 && next == due + DumasForageCooldown - 1);
        }
    }
}
