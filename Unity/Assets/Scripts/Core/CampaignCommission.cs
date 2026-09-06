using System;
using System.Collections.Generic;

namespace PowerAboveAll
{
    [Serializable] public sealed class CommissionMeasure
    {
        public string Id;
        public double Value, Target;
        public bool AtMost;
        public bool Met => AtMost ? Value <= Target : Value >= Target;
    }

    [Serializable] public sealed class FirstCommission
    {
        public string RoleId;
        public long StartedAt, DueAt, ResolvedAt = -1;
        public int Kept, Broken;
        public bool Seen;
        public List<CommissionMeasure> Report = new List<CommissionMeasure>();
        public bool Resolved => ResolvedAt >= 0;
        public bool Succeeded => Resolved && Report.Count > 0 && Report.TrueForAll(m => m.Met);
    }

    public static partial class CampaignCore
    {
        public const long CommissionDuration = 28 * WorldClock.Day;
        public const long MandateGrace = 2 * WorldClock.Day;
        public static FirstCommission Commission(CampaignState state) =>
            state?.Commissions != null && state.Commissions.Count == 1 ? state.Commissions[0] : null;

        internal static void StartCommission(CampaignState state)
        {
            if (state.RoleId == "legacy" || state.World == null || state.Commissions.Count != 0) return;
            state.Commissions.Add(new FirstCommission { RoleId = state.RoleId,
                StartedAt = state.World.Clock.Milliseconds, DueAt = state.World.Clock.Milliseconds + CommissionDuration });
        }

        public static List<CommissionMeasure> CommissionMeasures(CampaignState state)
        {
            var c = Commission(state);
            if (c == null) return new List<CommissionMeasure>();
            if (c.Resolved) return c.Report;
            return LiveCommissionMeasures(state, c);
        }

        private static List<CommissionMeasure> LiveCommissionMeasures(CampaignState state, FirstCommission c)
        {
            var values = new List<CommissionMeasure>();
            AddMeasure(values, "power", state.Power, 35);
            AddMeasure(values, "trust", Character(state, PatronIdForRole(c.RoleId)).Relationship, 50);
            AddMeasure(values, "kept", c.Kept, 1);
            AddMeasure(values, "broken", c.Broken, 0, true);
            if (c.RoleId == "crown")
            {
                AddMeasure(values, "gold", state.Gold, 950);
                AddMeasure(values, "food", state.Food, 120);
            }
            else if (c.RoleId == "assembly")
            {
                var region = Region(state, "champagne");
                AddMeasure(values, "unrest", region.Unrest, 40, true);
                AddMeasure(values, "control", region.Control, 65);
            }
            else
            {
                var army = state.World.Army(state.World.PlayerArmyId);
                AddMeasure(values, "troops", army.Men, 1000);
                AddMeasure(values, "morale", army.Morale, 65);
                AddMeasure(values, "rations", WorldSupply.DaysLeft(army), 3);
            }
            return values;
        }

        private static void AddMeasure(List<CommissionMeasure> list, string id, double value, double target, bool atMost = false)
            => list.Add(new CommissionMeasure { Id = id, Value = value, Target = target, AtMost = atMost });

        internal static long NextCommissionBoundary(CampaignState state)
        {
            long boundary = long.MaxValue;
            var c = Commission(state);
            if (c != null && !c.Resolved) boundary = c.DueAt;
            if (state.Obligation != null) boundary = Math.Min(boundary, state.Obligation.DueWeek * WorldClock.Week + MandateGrace);
            return boundary;
        }

        internal static void CountCommissionPromise(CampaignState state, bool kept)
        {
            var c = Commission(state);
            if (c == null || c.Resolved) return;
            if (kept) c.Kept++; else c.Broken++;
        }

        // Borç sahibinin kararı, oyuncunun başka bir belgeyi açık tutmasını beklemez.
        internal static bool ProcessCommissionTime(CampaignState state)
        {
            var clock = state.World.Clock;
            if (state.Obligation != null && clock.Milliseconds >= state.Obligation.DueWeek * WorldClock.Week + MandateGrace)
            {
                var obligation = state.Obligation;
                var terms = GetObligationTerms(state);
                ApplyMandateEffect(state, obligation.RegionId, terms.Break);
                state.Obligation = null;
                CountCommissionPromise(state, false);
                Record(state, "log.commission.promise_expired");
                state.World.LastNoticeKey = "log.commission.promise_expired";
            }
            var c = Commission(state);
            if (c == null || c.Resolved || clock.Milliseconds < c.DueAt) return false;
            c.Report = LiveCommissionMeasures(state, c);
            c.ResolvedAt = clock.Milliseconds;
            int change = c.Succeeded ? 4 : -4;
            state.Power = Clamp(state.Power + change);
            var patron = Character(state, PatronIdForRole(c.RoleId));
            patron.Relationship = Clamp(patron.Relationship + change);
            var key = c.Succeeded ? "log.commission.success" : "log.commission.failure";
            Record(state, key);
            state.World.LastNoticeKey = key;
            return true;
        }

        private static void ValidateCommission(CampaignState state)
        {
            Require(state.Commissions != null && state.Commissions.Count <= 1);
            if (state.Commissions.Count == 0) return;
            var c = state.Commissions[0];
            Require(c != null && state.World != null && c.RoleId == state.RoleId && c.RoleId != "legacy");
            Require(c.StartedAt >= 0 && c.StartedAt <= state.World.Clock.Milliseconds && c.DueAt == c.StartedAt + CommissionDuration);
            Require(c.Kept >= 0 && c.Kept <= 1000000 && c.Broken >= 0 && c.Broken <= 1000000 && c.Report != null);
            if (!c.Resolved) { Require(c.ResolvedAt == -1 && !c.Seen && c.Report.Count == 0 && state.World.Clock.Milliseconds <= c.DueAt); return; }
            Require(c.ResolvedAt == c.DueAt && c.ResolvedAt <= state.World.Clock.Milliseconds);
            var expected = LiveCommissionMeasures(state, c);
            Require(c.Report.Count == expected.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                var m = c.Report[i];
                Require(m != null && m.Id == expected[i].Id && m.Target == expected[i].Target && m.AtMost == expected[i].AtMost);
                Require(!double.IsNaN(m.Value) && !double.IsInfinity(m.Value) && m.Value >= 0 && m.Value <= MaximumStock);
            }
        }
    }
}
