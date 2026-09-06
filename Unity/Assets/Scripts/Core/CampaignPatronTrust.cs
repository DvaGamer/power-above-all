using System;

namespace PowerAboveAll
{
    public sealed class PatronRepairTerms
    {
        public string PatronId;
        public float PowerCost, RelationshipGain;
    }

    public static partial class CampaignCore
    {
        public static string PatronIdForRole(string roleId)
        {
            switch (roleId)
            {
                case "crown": return "valcourt";
                case "assembly": return "morel";
                case "army": return "dumas";
                default: return null;
            }
        }

        // Bu teklif eski borcu veya bölgesel etkileri geri almaz; yalnız kişisel erişimi onarır.
        public static PatronRepairTerms GetPatronRepairTerms(CampaignState state)
        {
            if (!ValidRoleData(state)) return null;
            string patronId = PatronIdForRole(state.RoleId);
            if (patronId == null) return null;
            var patron = state.Characters == null ? null : Character(state, patronId);
            if (patron == null || !Percent(patron.Relationship)) return null;
            var mandate = BuildMandateTerms(RoleKind(state.RoleId), "ile", 0);
            return new PatronRepairTerms {
                PatronId = patronId,
                PowerCost = Math.Min(state.Power, -mandate.Break.Power),
                RelationshipGain = Math.Min(100 - patron.Relationship, mandate.Fulfil.Relationship)
            };
        }

        public static ActionResult CanRepairPatronTrust(CampaignState state)
        {
            if (!ValidRoleData(state)) return Result(false, "error.role.invalid");
            if (state.RoleId == "legacy") return Result(false, "error.role.legacy");
            var terms = GetPatronRepairTerms(state);
            if (terms == null) return Result(false, "error.role.invalid");
            if (state.PendingPetition) return Result(false, "error.mandate.petition");
            if (state.Obligation != null) return Result(false, "error.trust.open");
            if (Character(state, terms.PatronId).Relationship > 0) return Result(false, "error.trust.not_broken");
            return Result(true, "log.trust.ready");
        }

        public static ActionResult RepairPatronTrust(CampaignState state)
        {
            var check = CanRepairPatronTrust(state);
            if (!check.Ok) return check;
            var terms = GetPatronRepairTerms(state);
            state.Power = Clamp(state.Power - terms.PowerCost);
            var patron = Character(state, terms.PatronId);
            patron.Relationship = Clamp(patron.Relationship + terms.RelationshipGain);
            return Record(state, "log.trust.repaired", patron.NameKey);
        }
    }
}
