using System;
using System.Globalization;

namespace PowerAboveAll
{
    public sealed class OfficerCommissionTerms
    {
        public bool IsActive, ExtraRecruitUsed;
        public int CurrentTroops, RecruitTroops, GoldCost, FoodCost, MilitarySuppliesCost, ManpowerCost, RevokeGoldCost;
        public int TroopsAfterRecruit, ManpowerAfterRecruit, CurrentArmyCost, ArmyCostAfterRecruit;
        public int CurrentArmyConsumption, ArmyConsumptionAfterRecruit;
        public float UnrestDelta, MoraleDelta, ArmyApprovalDelta, LoyaltyDelta;
    }

    public static partial class CampaignCore
    {
        private const int RecruitTroops = 200, RecruitGold = 120, RecruitFood = 20, RecruitSupplies = 15;

        // Eski ret sırası korunur: kullanılmış yer, konum, maliyet, kapasite.
        private static ActionResult CheckRecruitment(CampaignState state, RegionState region, bool extra)
        {
            if (!extra && region.RecruitUsed) return Result(false, "error.used");
            if (region.Id != state.ArmyRegionId) return Result(false, "error.recruit.location");
            if (state.Gold < RecruitGold || state.Food < RecruitFood || state.MilitarySupplies < RecruitSupplies || state.Manpower < RecruitTroops)
                return Result(false, "error.recruit.cost");
            if (state.Troops > MaximumStock - RecruitTroops) return Result(false, "error.capacity");
            return Result(true, "log.commission.ready");
        }

        private static void ApplyRecruitment(CampaignState state, RegionState region)
        {
            state.Gold -= RecruitGold; state.Food -= RecruitFood; state.MilitarySupplies -= RecruitSupplies;
            state.Manpower -= RecruitTroops; state.Troops += RecruitTroops; region.RecruitUsed = true;
            region.Unrest = Clamp(region.Unrest + 2); state.Morale = Clamp(state.Morale - 2);
            var army = Faction(state, "army"); army.Approval = Clamp(army.Approval + 2);
            RefreshArmyReduction(state);
        }

        public static bool HasOfficerCommission(CampaignState state)
        { return ValidOfficerCommissionCampaign(state) && state.DumasOfficerCommission; }

        public static OfficerCommissionTerms GetOfficerCommissionTerms(CampaignState state)
        {
            if (!ValidOfficerCommissionCampaign(state)) return null;
            var region = Region(state, state.ArmyRegionId); var general = Character(state, "dumas");
            bool resourcesFit = CheckRecruitment(state, region, true).Ok;
            int troopsAfter = resourcesFit ? state.Troops + RecruitTroops : state.Troops;
            return new OfficerCommissionTerms {
                IsActive = state.DumasOfficerCommission, ExtraRecruitUsed = state.DumasExtraRecruitUsed,
                CurrentTroops = state.Troops, RecruitTroops = RecruitTroops,
                GoldCost = RecruitGold, FoodCost = RecruitFood, MilitarySuppliesCost = RecruitSupplies, ManpowerCost = RecruitTroops,
                RevokeGoldCost = (int)Math.Ceiling(state.Troops / 12d),
                TroopsAfterRecruit = troopsAfter, ManpowerAfterRecruit = resourcesFit ? state.Manpower - RecruitTroops : state.Manpower,
                CurrentArmyCost = ArmyCostFor(state, state.Troops), ArmyCostAfterRecruit = ArmyCostFor(state, troopsAfter),
                CurrentArmyConsumption = ArmyFoodFor(state.Troops), ArmyConsumptionAfterRecruit = ArmyFoodFor(troopsAfter),
                UnrestDelta = Clamp(region.Unrest + 2) - region.Unrest,
                MoraleDelta = Clamp(state.Morale - 2) - state.Morale,
                ArmyApprovalDelta = Clamp(Faction(state, "army").Approval + 2) - Faction(state, "army").Approval,
                LoyaltyDelta = Clamp(general.Loyalty + 1) - general.Loyalty
            };
        }

        private static ActionResult CheckOfficerCommissionCommand(CampaignState state)
        {
            if (!ValidOfficerCommissionCampaign(state)) return Result(false, "error.commission.state");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (MandateDue(state)) return Result(false, "error.mandate.due");
            if (state.Week >= MaximumWeek) return Result(false, "error.week.limit");
            return Result(true, "log.commission.ready");
        }

        public static ActionResult CanGrantOfficerCommission(CampaignState state)
        {
            var check = CheckOfficerCommissionCommand(state); if (!check.Ok) return check;
            if (state.DumasOfficerCommission) return Result(false, "error.commission.active");
            if (state.Troops <= 0) return Result(false, "error.army.empty");
            if (state.ArmyPolicyId != "campaign") return Result(false, "error.commission.policy");
            return Result(true, "log.commission.ready");
        }

        public static ActionResult GrantOfficerCommission(CampaignState state)
        {
            var check = CanGrantOfficerCommission(state); if (!check.Ok) return check;
            state.DumasOfficerCommission = true;
            return Record(state, "log.commission.granted");
        }

        public static ActionResult CanRecruitThroughDumas(CampaignState state)
        {
            var check = CheckOfficerCommissionCommand(state); if (!check.Ok) return check;
            if (!state.DumasOfficerCommission) return Result(false, "error.commission.none");
            if (state.DumasExtraRecruitUsed) return Result(false, "error.commission.used");
            var region = Region(state, state.ArmyRegionId);
            if (!region.RecruitUsed) return Result(false, "error.commission.normal_required");
            return CheckRecruitment(state, region, true);
        }

        public static ActionResult RecruitThroughDumas(CampaignState state)
        {
            var check = CanRecruitThroughDumas(state); if (!check.Ok) return check;
            var region = Region(state, state.ArmyRegionId); var general = Character(state, "dumas");
            float loyaltyGain = Clamp(general.Loyalty + 1) - general.Loyalty;
            ApplyRecruitment(state, region);
            general.Loyalty += loyaltyGain; state.DumasExtraRecruitUsed = true;
            return Record(state, "log.commission.recruited", "region." + region.Id, N(RecruitTroops),
                loyaltyGain.ToString("0.##", CultureInfo.InvariantCulture));
        }

        public static ActionResult CanRevokeOfficerCommission(CampaignState state)
        {
            var check = CheckOfficerCommissionCommand(state); if (!check.Ok) return check;
            if (!state.DumasOfficerCommission) return Result(false, "error.commission.none");
            int cost = (int)Math.Ceiling(state.Troops / 12d);
            if (state.Gold < cost) return Result(false, "error.commission.gold", N(cost));
            return Result(true, "log.commission.ready");
        }

        public static ActionResult RevokeOfficerCommission(CampaignState state)
        {
            var check = CanRevokeOfficerCommission(state); if (!check.Ok) return check;
            int cost = (int)Math.Ceiling(state.Troops / 12d);
            state.Gold -= cost; state.DumasOfficerCommission = false;
            // Kullanım haftası, geri alınan hakla beraber unutulmaz; yeniden imza ikinci grup üretmez.
            return Record(state, "log.commission.revoked", N(cost));
        }

        private static bool ValidOfficerCommissionCampaign(CampaignState state)
        {
            try { Validate(state); return true; }
            catch (ArgumentException) { return false; }
        }

        private static void ValidateOfficerCommissionState(CampaignState state)
        {
            // İlave grup bu haftanın gerçek bir normal alımından sonra kurulmuş olmalıdır.
            Require(!state.DumasExtraRecruitUsed || state.Regions.Exists(region => region.RecruitUsed));
        }
    }
}
