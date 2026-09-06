using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    // Sayılar çekirdek koşullarından gelir; metinler bir denge tablosu kopyası değildir.
    public static class MandatePresentation
    {
        public static string RoleName(string roleId) => L.Text("ui.mandate.role." + (string.IsNullOrEmpty(roleId) ? "legacy" : roleId));
        public static string PrivilegeName(string kind) => L.Text("ui.mandate.privilege." + kind);
        public static string PatronId(string kind) => kind == "civic_pledge" ? "morel" : kind == "field_levy" ? "dumas" : "valcourt";
        public static int PortraitIndex(string kind) => kind == "civic_pledge" ? 1 : kind == "field_levy" ? 3 : 0;
        public static string Date(int week)
        {
            DateTime start = new DateTime(1789, 5, 5);
            long days = Math.Max(0L, (long)week * 7);
            DateTime date = start.AddDays(Math.Min(days, (DateTime.MaxValue.Date - start).Days));
            return L.Text("ui.date", date.Day, L.Text("ui.month." + date.Month), date.Year);
        }
        public static string Effects(MandateEffect effect, bool compact = true)
        {
            if (effect == null) return L.Text("ui.mandate.effect.none");
            var lines = new List<string>();
            Add(lines, "ui.mandate.effect.gold", effect.Gold);
            Add(lines, "ui.mandate.effect.food", effect.Food);
            Add(lines, "ui.mandate.effect.supplies", effect.MilitarySupplies);
            Add(lines, "ui.mandate.effect.power", effect.Power);
            Add(lines, "ui.mandate.effect.unrest", effect.Unrest);
            Add(lines, "ui.mandate.effect.control", effect.Control);
            Add(lines, "ui.mandate.effect.elite", effect.EliteLoyalty);
            if (effect.Approval != 0) AddLabel(lines, L.Text("ui.mandate.effect.approval", L.Text("faction." + effect.FactionId)), effect.Approval);
            if (effect.Relationship != 0) AddLabel(lines, L.Text("ui.mandate.effect.relationship", L.Text("character." + effect.CharacterId + ".name")), effect.Relationship);
            return lines.Count == 0 ? L.Text("ui.mandate.effect.none") : string.Join(compact ? " · " : "\n", lines);
        }
        private static void Add(List<string> lines, string key, float value)
        {
            if (value != 0) AddLabel(lines, L.Text(key), value);
        }
        private static void AddLabel(List<string> lines, string label, float value)
        {
            var culture = CultureInfo.GetCultureInfo(L.Language == "tr" ? "tr-TR" : "ru-RU");
            lines.Add(L.Text("ui.mandate.effect.delta", label, value.ToString("+0.#;−0.#;0", culture)));
        }
    }

    // Süresi gelmiş tek sözün mektubu; belge hiçbir haftayı veya ekonomik işlemi kendisi çalıştırmaz.
    public sealed class MandateDocument : MonoBehaviour
    {
        private static readonly Color Paper = C("#F3E7CA"), Ink = C("#243B37"), Wine = C("#58464D"),
            Muted = C("#53604D"), Brass = C("#CAB36F"), Pale = C("#E9DCB7");
        private GUIStyle title, body, small, choice, caption, lightText;
        private Font serif, sans;
        private Texture2D portraits;
        private static Color C(string hex) { ColorUtility.TryParseHtmlString(hex, out var colour); return colour; }
        private void Prepare()
        {
            if (title != null) return;
            serif = Font.CreateDynamicFontFromOSFont(new[] { "Georgia", "Times New Roman", "Liberation Serif" }, 30);
            sans = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "DejaVu Sans" }, 17);
            title = Style(32, serif, Ink); body = Style(16, sans, Ink); small = Style(14, sans, Muted);
            choice = Style(23, serif, Ink); caption = Style(12, sans, Muted); lightText = Style(14, sans, Paper);
            portraits = Resources.Load<Texture2D>("Art/PoliticalPortraits-v1");
        }
        private static GUIStyle Style(int size, Font font, Color colour)
        {
            var style = new GUIStyle(GUI.skin.label) { fontSize = size, font = font, wordWrap = true,
                padding = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0) };
            style.normal.textColor = style.hover.textColor = style.active.textColor = style.focused.textColor = colour;
            return style;
        }
        private static void Fill(Rect rect, Color colour)
        {
            Color old = GUI.color; GUI.color = colour; GUI.DrawTexture(rect, Texture2D.whiteTexture); GUI.color = old;
        }
        public void Draw(GameApp app)
        {
            if (app == null || app.State == null || app.State.PendingPetition || !CampaignCore.MandateDue(app.State)) return;
            MandateTerms terms = CampaignCore.GetObligationTerms(app.State);
            if (terms == null) return;
            Prepare();
            CampaignState state = app.State;
            string expectedId = CampaignCore.MandateId(state.Obligation);
            ActionResult fulfil = CampaignCore.CanResolveMandate(state, expectedId, "fulfil");
            ActionResult broken = CampaignCore.CanResolveMandate(state, expectedId, "break");
            Fill(new Rect(0, 0, 1440, 900), new Color(.14f, .23f, .216f, .72f));
            Fill(new Rect(210, 79, 1040, 768), new Color(.15f, .12f, .13f, .36f));
            Fill(new Rect(200, 68, 1040, 768), Paper);
            Fill(new Rect(200, 68, 1040, 52), Wine);
            GUI.Label(new Rect(232, 85, 760, 23), L.Text("ui.mandate.due.kicker"), lightText);
            DrawLanguageControls(app);

            Fill(new Rect(464, 146, 1, 650), C("#C9C29E"));
            DrawPortrait(new Rect(254, 154, 172, 186), MandatePresentation.PortraitIndex(terms.Kind));
            string patron = MandatePresentation.PatronId(terms.Kind);
            GUI.Label(new Rect(232, 357, 208, 61), L.Text("character." + patron + ".name"), choice);
            GUI.Label(new Rect(232, 425, 208, 62), MandatePresentation.RoleName(state.RoleId), small);
            Fill(new Rect(232, 501, 208, 1), Brass);
            GUI.Label(new Rect(232, 522, 208, 57), L.Text("ui.mandate.region", L.Text("region." + terms.RegionId)), body);
            GUI.Label(new Rect(232, 592, 208, 52), L.Text("ui.mandate.issued", MandatePresentation.Date(terms.IssuedWeek)), small);
            GUI.Label(new Rect(232, 651, 208, 52), L.Text("ui.mandate.due", MandatePresentation.Date(terms.DueWeek)), body);
            GUI.Label(new Rect(232, 722, 208, 75), L.Text("ui.mandate.original_region"), small);

            GUI.Label(new Rect(488, 146, 704, 48), L.Text("ui.mandate.due.title"), title);
            GUI.Label(new Rect(488, 211, 704, 66), L.Text("ui.mandate.reminder." + terms.Kind), body);
            GUI.Label(new Rect(488, 287, 704, 21), L.Text("ui.mandate.agreed_terms"), caption);
            GUI.Label(new Rect(488, 312, 704, 65), MandatePresentation.Effects(terms.Immediate), small);

            if (DrawChoice(new Rect(488, 394, 336, 370), "fulfil", terms.Fulfil, fulfil))
            { app.ResolveMandate(expectedId, "fulfil"); return; }
            if (DrawChoice(new Rect(852, 394, 336, 370), "break", terms.Break, broken))
            { app.ResolveMandate(expectedId, "break"); return; }
            GUI.Label(new Rect(488, 782, 704, 23), L.Text("ui.mandate.stocks", state.Gold, state.Food), small);
            GUI.Label(new Rect(488, 810, 704, 20), L.Text("ui.mandate.meter_limits"), caption);
        }
        private bool DrawChoice(Rect rect, string id, MandateEffect effect, ActionResult check)
        {
            Color accent = id == "fulfil" ? Ink : Wine;
            Fill(new Rect(rect.x, rect.y, rect.width, 2), accent);
            GUI.Label(new Rect(rect.x + 16, rect.y + 18, rect.width - 32, 59), L.Text("ui.mandate.choice." + id), choice);
            GUI.Label(new Rect(rect.x + 16, rect.y + 83, rect.width - 32, 178), MandatePresentation.Effects(effect, false), body);
            if (!check.Ok)
            {
                var warning = new GUIStyle(small); warning.normal.textColor = Wine;
                GUI.Label(new Rect(rect.x + 16, rect.y + 263, rect.width - 32, 48), L.Text(check.Key, check.Args), warning);
            }
            Rect button = new Rect(rect.x + 16, rect.y + 320, rect.width - 32, 42);
            bool enabled = GUI.enabled && check.Ok, hover = enabled && button.Contains(Event.current.mousePosition);
            Fill(button, enabled ? hover ? Color.Lerp(accent, Brass, .20f) : accent : Pale);
            var label = new GUIStyle(lightText) { alignment = TextAnchor.MiddleCenter };
            if (!enabled) label.normal.textColor = Muted;
            GUI.Label(button, L.Text("ui.mandate.action." + id), label);
            bool previous = GUI.enabled; GUI.enabled = enabled;
            bool clicked = GUI.Button(button, GUIContent.none, GUIStyle.none); GUI.enabled = previous;
            return clicked;
        }
        private void DrawLanguageControls(GameApp app)
        {
            var label = new GUIStyle(lightText) { alignment = TextAnchor.MiddleCenter };
            for (int i = 0; i < 2; i++)
            {
                string language = i == 0 ? "ru" : "tr";
                Rect rect = new Rect(1092 + i * 62, 80, 53, 29);
                if (L.Language == language) Fill(new Rect(rect.x + 9, rect.yMax - 2, rect.width - 18, 2), Brass);
                if (GUI.Button(rect, language.ToUpperInvariant(), label)) app.SetLanguage(language);
            }
        }
        private void DrawPortrait(Rect rect, int index)
        {
            if (!portraits) return;
            var uv = new Rect((index % 2) * .5f, index < 2 ? .5f : 0f, .5f, .5f);
            float aspect = (float)portraits.width / portraits.height;
            float width = Mathf.Min(rect.width, rect.height * aspect), height = width / aspect;
            Rect fitted = new Rect(rect.x + (rect.width - width) * .5f, rect.y + (rect.height - height) * .5f, width, height);
            if (index == 3)
            {
                float trim = 17f / portraits.width;
                fitted.xMin += fitted.width * trim / uv.width; uv.xMin += trim;
            }
            GUI.DrawTextureWithTexCoords(fitted, portraits, uv, true);
        }
        private void OnDestroy() { if (serif) Destroy(serif); if (sans) Destroy(sans); }
    }
}
