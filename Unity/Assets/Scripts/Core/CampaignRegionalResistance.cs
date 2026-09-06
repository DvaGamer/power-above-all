using System;

namespace PowerAboveAll
{
    public sealed class RegionalResistanceTerms
    {
        public string RegionId;
        public bool RequiresBattle;
        public int EnemyTroops, BaseTax;
        public double MobilizationBase, UnrestPressure, ControlGap, EliteOpposition;
    }

    public static partial class CampaignCore
    {
        private static bool IsHostileRegion(RegionState region) { return region.Unrest >= 65; }

        public static RegionalResistanceTerms GetRegionalResistance(CampaignState state, string regionId)
        {
            try { Validate(state); }
            catch (ArgumentException) { return null; }
            var definition = Definition(regionId);
            if (definition == null) return null;
            var region = Region(state, regionId);
            var terms = new RegionalResistanceTerms {
                RegionId = regionId, RequiresBattle = IsHostileRegion(region), BaseTax = definition.BaseTax,
                MobilizationBase = 30d * definition.BaseTax, UnrestPressure = region.Unrest / 100d,
                ControlGap = (100d - region.Control) / 100d, EliteOpposition = (100d - region.EliteLoyalty) / 100d
            };
            // Yalnız toplam yuvarlanır; oyuncunun ordusu ve kasası bu yerel hesaba girmez.
            if (terms.RequiresBattle)
                terms.EnemyTroops = Round(terms.MobilizationBase * (terms.UnrestPressure + terms.ControlGap + terms.EliteOpposition));
            return terms;
        }
    }
}
