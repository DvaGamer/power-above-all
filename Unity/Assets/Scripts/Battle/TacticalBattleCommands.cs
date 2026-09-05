using System;
using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    public enum BattleSelectionMode { Replace, Add, Toggle }
    public enum BattleFormation { Line, Column, Square }

    public sealed class BattleOrderResult
    {
        public bool Ok;
        public string ReasonKey;
        public int AffectedCount;
    }

    [Serializable] public sealed class BattleRegimentSnapshot
    {
        public int Id, PlayerSlot, Original, Men, Ammo;
        public bool Player, Selected, Commandable, FireAtWill, Moving, Routed, Withdrawn, CanVolley, AimedVolleyPending;
        public string Kind, Formation, Condition, VolleyReasonKey;
        public float Morale, Cohesion, Fatigue, Reload, ContactReload, Facing, PositionX, PositionZ, DestinationX, DestinationZ;
    }

    [Serializable] public sealed class BattleSnapshot
    {
        public int SchemaVersion = 1;
        public bool Active, Paused, Ended, Delivered, HasOutcome, Won, CanVolley, SelectionArrived;
        public int OriginalTroops, Casualties, MilitarySuppliesRecovered;
        public float ElapsedSeconds, PlayerHold, EnemyHold, ConvoyX, ConvoyZ, EndingMorale, CampaignReturnMorale;
        public int[] SelectedIds;
        public BattleRegimentSnapshot[] Regiments;
    }

    public sealed partial class TacticalBattle
    {
        public bool Paused => paused;
        public bool Ended => ended;

        static BattleOrderResult OrderResult(bool ok, string reason = null, int affected = 0)
        { return new BattleOrderResult { Ok = ok, ReasonKey = reason, AffectedCount = affected }; }

        BattleOrderResult OrderGate()
        { return Active && !ended ? OrderResult(true) : OrderResult(false, "battle.order_unavailable"); }

        int CommandableSelectionCount()
        {
            int count = 0;
            foreach (Regiment regiment in selected) if (Commandable(regiment)) count++;
            return count;
        }

        public BattleOrderResult SelectPlayerRegiment(int slot, BattleSelectionMode mode = BattleSelectionMode.Replace)
        {
            var gate = OrderGate(); if (!gate.Ok) return gate;
            if (slot < 1 || slot > 4 || !Enum.IsDefined(typeof(BattleSelectionMode), mode))
                return OrderResult(false, "battle.order_invalid");
            Regiment target = null;
            int at = 0;
            foreach (Regiment regiment in regiments)
                if (regiment.Player && ++at == slot) { target = regiment; break; }
            if (target == null || !Commandable(target)) return OrderResult(false, "battle.select_to_command");
            if (mode == BattleSelectionMode.Replace) selected.Clear();
            if (mode == BattleSelectionMode.Toggle && selected.Contains(target)) selected.Remove(target);
            else if (!selected.Contains(target)) selected.Add(target);
            Feedback?.Invoke("select");
            return OrderResult(true, affected: 1);
        }

        public BattleOrderResult MoveSelected(Vector2 worldXZ)
        {
            var gate = OrderGate(); if (!gate.Ok) return gate;
            if (float.IsNaN(worldXZ.x) || float.IsInfinity(worldXZ.x) || float.IsNaN(worldXZ.y) || float.IsInfinity(worldXZ.y))
                return OrderResult(false, "battle.order_invalid");
            int count = CommandableSelectionCount();
            if (count == 0) return OrderResult(false, "battle.select_to_command");
            Vector3 centre = Vector3.zero;
            foreach (Regiment regiment in selected) if (Commandable(regiment)) centre += regiment.Position;
            centre /= count;
            Vector3 point = new Vector3(worldXZ.x, 0, worldXZ.y);
            foreach (Regiment regiment in selected)
            {
                if (!Commandable(regiment)) continue;
                Vector3 offset = count > 1 ? regiment.Position - centre : Vector3.zero;
                regiment.Destination = Bound(point + offset);
                regiment.Moving = true;
            }
            Feedback?.Invoke("move");
            return OrderResult(true, affected: count);
        }

        public BattleOrderResult SetSelectedFormation(BattleFormation formation)
        {
            var gate = OrderGate(); if (!gate.Ok) return gate;
            if (!Enum.IsDefined(typeof(BattleFormation), formation)) return OrderResult(false, "battle.order_invalid");
            int eligible = 0, changed = 0;
            foreach (Regiment regiment in selected)
            {
                if (!Commandable(regiment)) continue;
                if (formation == BattleFormation.Square && (regiment.Kind == Kind.Cavalry || regiment.Kind == Kind.Artillery)) continue;
                eligible++;
                if ((int)regiment.Formation != (int)formation) changed++;
            }
            if (eligible == 0) return OrderResult(false, CommandableSelectionCount() == 0 ? "battle.select_to_command" : "battle.square_infantry");
            OrderFormation((Formation)formation);
            return OrderResult(true, affected: changed);
        }

        public BattleOrderResult SetSelectedFireAtWill(bool fire)
        {
            var gate = OrderGate(); if (!gate.Ok) return gate;
            int count = CommandableSelectionCount();
            if (count == 0) return OrderResult(false, "battle.select_to_command");
            SetFireOrder(fire);
            return OrderResult(true, affected: count);
        }

        bool ReviewCanVolley(Regiment regiment)
        {
            if (!Active || ended || paused || !Commandable(regiment)) return false;
            Regiment enemy = FindEnemy(regiment);
            return enemy != null && CanVolley(regiment, enemy);
        }

        public BattleOrderResult VolleySelected()
        {
            var gate = OrderGate(); if (!gate.Ok) return gate;
            if (paused) return OrderResult(false, "battle.volley_reason_pause");
            int count = 0, newlyQueued = 0;
            foreach (Regiment regiment in selected)
                if (ReviewCanVolley(regiment)) { count++; if (!regiment.AimedVolleyPending) newlyQueued++; }
            if (count == 0)
            {
                Regiment primary = FirstCommandable();
                return OrderResult(false, primary == null ? "battle.select_to_command" : VolleyReason(primary));
            }
            OrderVolley();
            return OrderResult(true, affected: newlyQueued);
        }

        public BattleOrderResult SetPaused(bool value)
        {
            var gate = OrderGate(); if (!gate.Ok) return gate;
            bool changed = paused != value;
            paused = value;
            return OrderResult(true, affected: changed ? 1 : 0);
        }

        public BattleOrderResult Retreat()
        {
            var gate = OrderGate(); if (!gate.Ok) return gate;
            Finish(false, true);
            return OrderResult(true, affected: 1);
        }

        public BattleOrderResult AcceptReport()
        {
            if (!Active || !ended || delivered || outcome == null) return OrderResult(false, "battle.order_unavailable");
            AcceptOutcome();
            return OrderResult(true, affected: 1);
        }

        public BattleSnapshot CaptureSnapshot()
        {
            var units = new List<BattleRegimentSnapshot>();
            var selection = new List<int>();
            foreach (Regiment regiment in selected) selection.Add(regiment.Id);
            bool arrived = selected.Count > 0;
            foreach (Regiment regiment in selected) arrived &= Commandable(regiment) && !regiment.Moving;
            bool volley = false;
            int playerSlot = 0;
            foreach (Regiment regiment in regiments)
            {
                bool chosen = selected.Contains(regiment), ready = ReviewCanVolley(regiment);
                if (chosen && ready) volley = true;
                units.Add(new BattleRegimentSnapshot {
                    Id = regiment.Id, PlayerSlot = regiment.Player ? ++playerSlot : 0,
                    Player = regiment.Player, Selected = chosen, Commandable = Commandable(regiment),
                    Original = regiment.Original, Men = regiment.Men, Ammo = regiment.Ammo,
                    Kind = regiment.Kind.ToString(), Formation = regiment.Formation.ToString(), Condition = regiment.Condition.ToString(),
                    FireAtWill = regiment.FireAtWill, Moving = regiment.Moving, Routed = regiment.Routed, Withdrawn = regiment.Withdrawn,
                    Morale = regiment.Morale, Cohesion = regiment.Cohesion, Fatigue = regiment.Fatigue, Reload = regiment.Reload, Facing = regiment.Facing,
                    ContactReload = regiment.ContactReload, AimedVolleyPending = regiment.AimedVolleyPending,
                    PositionX = regiment.Position.x, PositionZ = regiment.Position.z,
                    DestinationX = regiment.Destination.x, DestinationZ = regiment.Destination.z,
                    CanVolley = ready, VolleyReasonKey = !Active || ended ? "battle.order_unavailable" : !Commandable(regiment) ? "battle.select_to_command" : VolleyReason(regiment)
                });
            }
            return new BattleSnapshot {
                Active = Active, Paused = paused, Ended = ended, Delivered = delivered, ElapsedSeconds = elapsed,
                OriginalTroops = originalTroops, PlayerHold = playerHold, EnemyHold = enemyHold, ConvoyX = convoy.x, ConvoyZ = convoy.z,
                SelectedIds = selection.ToArray(), Regiments = units.ToArray(), CanVolley = volley, SelectionArrived = arrived,
                HasOutcome = ended && outcome != null, Won = outcome != null && outcome.Won,
                Casualties = outcome == null ? 0 : outcome.Casualties, EndingMorale = outcome == null ? 0 : outcome.EndingMorale,
                MilitarySuppliesRecovered = outcome == null ? 0 : outcome.MilitarySuppliesRecovered, CampaignReturnMorale = campaignReturnMorale
            };
        }
    }
}
