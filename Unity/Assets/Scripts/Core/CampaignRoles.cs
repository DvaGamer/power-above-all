using System;

namespace PowerAboveAll
{
    [Serializable] public sealed class MandateObligation
    {
        public string Kind, RegionId;
        public int IssuedWeek, DueWeek, GoldDue, FoodDue;
    }

    // Değerler nominal değişimlerdir; siyasi/bölgesel göstergeler uygulanırken 0–100'e sınırlanır.
    public sealed class MandateEffect
    {
        public int Gold, Food, MilitarySupplies;
        public float Power, Unrest, Control, EliteLoyalty, Approval, Relationship;
        public string FactionId, CharacterId;
    }

    public sealed class MandateTerms
    {
        public string Kind, RegionId;
        public int IssuedWeek, DueWeek, NextMandateWeek;
        public MandateEffect Immediate, Fulfil, Break;
    }

    public static partial class CampaignCore
    {
        public const int MandateDelayWeeks = 2;
        public const int MandateCooldownWeeks = 4;
        public const float MandateMinimumPower = 10f;

        public static CampaignState Create(string roleId)
        {
            if (!KnownRole(roleId)) throw new ArgumentException("Unknown starting role.", nameof(roleId));
            var state = Create();
            state.RoleId = roleId;
            if (roleId != "legacy") Record(state, "log.role." + roleId);
            return state;
        }

        private static bool KnownRole(string roleId)
        { return roleId == "legacy" || roleId == "crown" || roleId == "assembly" || roleId == "army"; }

        private static string RoleKind(string roleId)
        {
            switch (roleId)
            {
                case "crown": return "royal_advance";
                case "assembly": return "civic_pledge";
                case "army": return "field_levy";
                default: return null;
            }
        }

        private static MandateTerms BuildMandateTerms(string kind, string regionId, int issuedWeek)
        {
            if (Definition(regionId) == null || issuedWeek < 0 || issuedWeek > MaximumWeek - MandateDelayWeeks) return null;
            var terms = new MandateTerms {
                Kind = kind, RegionId = regionId, IssuedWeek = issuedWeek,
                DueWeek = issuedWeek + MandateDelayWeeks, NextMandateWeek = issuedWeek + MandateCooldownWeeks
            };
            switch (kind)
            {
                case "royal_advance":
                    terms.RegionId = "ile";
                    terms.Immediate = new MandateEffect { Gold = 120, FactionId = "crown", Approval = -3 };
                    terms.Fulfil = new MandateEffect { Gold = -150, FactionId = "crown", Approval = 5, CharacterId = "valcourt", Relationship = 4 };
                    terms.Break = new MandateEffect { FactionId = "crown", Approval = -12, CharacterId = "valcourt", Relationship = -10, Power = -6 };
                    break;
                case "civic_pledge":
                    terms.Immediate = new MandateEffect { Unrest = -18, Control = 6, FactionId = "assembly", Approval = -3 };
                    terms.Fulfil = new MandateEffect { Food = -40, FactionId = "assembly", Approval = 5, CharacterId = "morel", Relationship = 4 };
                    terms.Break = new MandateEffect { Unrest = 18, Control = -6, FactionId = "assembly", Approval = -10, CharacterId = "morel", Relationship = -10, Power = -4 };
                    break;
                case "field_levy":
                    terms.Immediate = new MandateEffect { Food = 40, MilitarySupplies = 15, Unrest = 8, EliteLoyalty = -6 };
                    terms.Fulfil = new MandateEffect { Gold = -80, Unrest = -5, EliteLoyalty = 4, CharacterId = "dumas", Relationship = 4 };
                    terms.Break = new MandateEffect { Unrest = 12, EliteLoyalty = -8, CharacterId = "dumas", Relationship = -6, Power = -5 };
                    break;
                default: return null;
            }
            return terms;
        }

        public static MandateTerms GetMandateTerms(CampaignState state, string regionId)
        {
            if (state == null) return null;
            return BuildMandateTerms(RoleKind(state.RoleId), regionId, state.Week);
        }

        public static MandateTerms GetObligationTerms(CampaignState state)
        {
            var obligation = state == null ? null : state.Obligation;
            if (obligation == null) return null;
            var terms = BuildMandateTerms(obligation.Kind, obligation.RegionId, obligation.IssuedWeek);
            if (terms == null) return null;
            terms.DueWeek = obligation.DueWeek;
            terms.Fulfil.Gold = -obligation.GoldDue;
            terms.Fulfil.Food = -obligation.FoodDue;
            return terms;
        }

        public static string MandateId(MandateObligation obligation)
        {
            return obligation == null ? null : obligation.Kind + ":" + N(obligation.IssuedWeek) + ":" + obligation.RegionId;
        }

        public static bool MandateDue(CampaignState state)
        { return state != null && state.Obligation != null && state.Week >= state.Obligation.DueWeek; }

        public static ActionResult CanIssueMandate(CampaignState state, string regionId)
        {
            if (!ValidRoleData(state)) return Result(false, "error.role.invalid");
            if (state.RoleId == "legacy") return Result(false, "error.role.legacy");
            if (Definition(regionId) == null) return Result(false, "error.mandate.region");
            if (Desk(state)?.RegionId == regionId) return Result(false, "dispatch.outside_slice");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (state.Obligation != null) return Result(false, "error.mandate.open");
            string patronId = PatronIdForRole(state.RoleId);
            var patron = state.Characters == null ? null : Character(state, patronId);
            if (patron == null || !Percent(patron.Relationship)) return Result(false, "error.role.invalid");
            if (patron.Relationship <= 0) return Result(false, "error.trust.closed", patron.NameKey);
            if (state.Week < state.NextMandateWeek) return Result(false, "error.mandate.cooldown", N(state.NextMandateWeek - state.Week));
            if (state.Power < MandateMinimumPower) return Result(false, "error.mandate.power", N((int)MandateMinimumPower));
            if (state.Week > MaximumWeek - MandateDelayWeeks) return Result(false, "error.mandate.calendar");
            if (state.RoleId == "army")
            {
                if (state.Troops <= 0) return Result(false, "error.mandate.army.empty");
                if (regionId != state.ArmyRegionId) return Result(false, "error.mandate.army.location");
            }
            var terms = GetMandateTerms(state, regionId);
            if (!ResourcesFit(state, terms.Immediate)) return Result(false, "error.mandate.capacity");
            return Result(true, "log.mandate.ready");
        }

        public static ActionResult IssueMandate(CampaignState state, string regionId)
        {
            var check = CanIssueMandate(state, regionId);
            if (!check.Ok) return check;
            var terms = GetMandateTerms(state, regionId);
            ApplyMandateEffect(state, terms.RegionId, terms.Immediate);
            state.NextMandateWeek = terms.NextMandateWeek;
            state.Obligation = new MandateObligation {
                Kind = terms.Kind, RegionId = terms.RegionId, IssuedWeek = terms.IssuedWeek,
                DueWeek = terms.DueWeek, GoldDue = -terms.Fulfil.Gold, FoodDue = -terms.Fulfil.Food
            };
            return Record(state, "log.mandate." + terms.Kind + ".issued", "region." + terms.RegionId,
                N(terms.DueWeek), N(state.Obligation.GoldDue), N(state.Obligation.FoodDue));
        }

        public static ActionResult CanResolveMandate(CampaignState state, string expectedId, string choice)
        {
            if (!ValidRoleData(state)) return Result(false, "error.role.invalid");
            if (state.Obligation == null) return Result(false, "error.mandate.none");
            if (string.IsNullOrEmpty(expectedId) || expectedId != MandateId(state.Obligation)) return Result(false, "error.mandate.stale");
            if (choice != "fulfil" && choice != "break") return Result(false, "error.mandate.choice");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (choice == "fulfil")
            {
                if (state.Gold < state.Obligation.GoldDue) return Result(false, "error.mandate.gold", N(state.Obligation.GoldDue - state.Gold));
                if (state.Food < state.Obligation.FoodDue) return Result(false, "error.mandate.food", N(state.Obligation.FoodDue - state.Food));
            }
            return Result(true, "log.mandate.ready");
        }

        public static ActionResult ResolveMandate(CampaignState state, string expectedId, string choice)
        {
            var check = CanResolveMandate(state, expectedId, choice);
            if (!check.Ok) return check;
            var obligation = state.Obligation;
            var terms = GetObligationTerms(state);
            ApplyMandateEffect(state, obligation.RegionId, choice == "fulfil" ? terms.Fulfil : terms.Break);
            state.Obligation = null;
            return Record(state, "log.mandate." + obligation.Kind + "." + choice,
                "region." + obligation.RegionId, N(obligation.GoldDue), N(obligation.FoodDue));
        }

        private static bool ResourcesFit(CampaignState state, MandateEffect effect)
        {
            long gold = (long)state.Gold + effect.Gold, food = (long)state.Food + effect.Food,
                supplies = (long)state.MilitarySupplies + effect.MilitarySupplies;
            return gold >= 0 && gold <= MaximumStock && food >= 0 && food <= MaximumStock && supplies >= 0 && supplies <= MaximumStock;
        }

        private static void ApplyMandateEffect(CampaignState state, string regionId, MandateEffect effect)
        {
            state.Gold += effect.Gold;
            state.Food += effect.Food;
            state.MilitarySupplies += effect.MilitarySupplies;
            state.Power = Clamp(state.Power + effect.Power);
            var region = Region(state, regionId);
            region.Unrest = Clamp(region.Unrest + effect.Unrest);
            region.Control = Clamp(region.Control + effect.Control);
            region.EliteLoyalty = Clamp(region.EliteLoyalty + effect.EliteLoyalty);
            if (effect.FactionId != null)
            {
                var faction = Faction(state, effect.FactionId);
                faction.Approval = Clamp(faction.Approval + effect.Approval);
            }
            if (effect.CharacterId != null)
            {
                var character = Character(state, effect.CharacterId);
                character.Relationship = Clamp(character.Relationship + effect.Relationship);
            }
        }

        private static bool ValidRoleData(CampaignState state)
        {
            try { ValidateRoleState(state); return true; }
            catch (ArgumentException) { return false; }
        }

        private static void ValidateRoleState(CampaignState state)
        {
            Require(state != null && state.Week >= 0 && state.Week <= MaximumWeek && Percent(state.Power));
            Require(KnownRole(state.RoleId));
            Require(state.Mandates != null && state.Mandates.Count <= 1);
            if (state.Mandates.Count == 1) Require(state.Mandates[0] != null);
            Require(state.NextMandateWeek >= 0 && state.NextMandateWeek <= MaximumWeek + MandateCooldownWeeks);
            Require(state.NextMandateWeek == 0 || (state.NextMandateWeek >= MandateCooldownWeeks && state.NextMandateWeek <= (long)state.Week + MandateCooldownWeeks));
            if (state.RoleId == "legacy") Require(state.NextMandateWeek == 0 && state.Obligation == null);
            if (state.Obligation == null) return;
            var obligation = state.Obligation;
            Require(obligation.Kind == RoleKind(state.RoleId) && Definition(obligation.RegionId) != null);
            Require(obligation.IssuedWeek >= 0 && obligation.IssuedWeek <= state.Week && obligation.IssuedWeek <= MaximumWeek - MandateDelayWeeks);
            Require(obligation.DueWeek == obligation.IssuedWeek + MandateDelayWeeks && state.Week <= obligation.DueWeek);
            Require(state.NextMandateWeek == obligation.IssuedWeek + MandateCooldownWeeks);
            var terms = BuildMandateTerms(obligation.Kind, obligation.RegionId, obligation.IssuedWeek);
            Require(terms != null && terms.RegionId == obligation.RegionId);
            // Arşiv v2'nin ilk sözleşmeleri sabittir; serbest/negatif ödeme tutarı kabul edilmez.
            Require(obligation.GoldDue == -terms.Fulfil.Gold && obligation.FoodDue == -terms.Fulfil.Food);
        }
    }
}
