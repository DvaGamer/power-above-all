using UnityEngine;

namespace PowerAboveAll
{
    // Atama masası: seçim tamamlanana kadar gerçek sefer değiştirilmez.
    public sealed class RoleSelection : MonoBehaviour
    {
        private static readonly string[] Roles = { "crown", "assembly", "army" };
        private static readonly string[] Numerals = { "I", "II", "III" };
        private readonly Color ink = C("#243B37"), paper = C("#F3E7CA"), pale = C("#E9DCB7"),
            muted = C("#53604D"), brass = C("#CAB36F"), rule = C("#B9B995");
        private readonly Color[] accents = { C("#A9BA88"), C("#83B0B6"), C("#C98270") };
        private GUIStyle title, roleTitle, body, small, kicker, action, footerTitle;
        private Font serif, sans;
        private Texture2D portraits;
        private readonly MandateTerms[] openingTerms = new MandateTerms[3];
        private string selected = "crown";
        private bool ready;

        private void Update()
        {
            var app = GetComponent<GameApp>();
            if (app == null || !app.ChoosingRole) return;
            int direction = Input.GetKeyDown(KeyCode.RightArrow) ? 1 : Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0;
            if (direction != 0)
            {
                selected = Roles[(System.Array.IndexOf(Roles, selected) + direction + Roles.Length) % Roles.Length];
                app.Feedback("paper");
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) app.StartCampaign(selected);
            else if (Input.GetKeyDown(KeyCode.Escape)) app.CancelRoleSelection();
        }

        public void Open(string roleId)
        {
            selected = System.Array.IndexOf(Roles, roleId) >= 0 ? roleId : "crown";
        }

        public void Draw(GameApp app)
        {
            EnsureStyles();
            Fill(new Rect(0, 0, 1440, 900), ink);
            Fill(new Rect(24, 24, 1392, 852), paper);
            Fill(new Rect(24, 24, 1392, 6), brass);
            Label(new Rect(64, 56, 930, 22), "ui.start.kicker", kicker);
            Languages(app);
            Label(new Rect(64, 100, 1240, 65), "ui.start.title", title);
            Label(new Rect(64, 179, 1230, 52), "ui.start.intro", body);
            for (int i = 0; i < Roles.Length; i++) Document(app, i);
            Fill(new Rect(64, 709, 1312, 1), rule);
            GUI.Label(new Rect(64, 733, 830, 35), L.Text("ui.start.selected", L.Text("ui.start.role." + selected)), footerTitle);
            Label(new Rect(64, 778, 830, 43), "ui.start.connection." + selected, body);
            if (app.CanCancelRoleSelection && Button(new Rect(64, 834, 430, 28), L.Text("ui.start.cancel"), false))
                app.CancelRoleSelection();
            if (Button(new Rect(982, 757, 394, 62), L.Text("ui.start.begin"), true))
                app.StartCampaign(selected);
            Label(new Rect(982, 832, 394, 25), "ui.start.note", small);
        }

        private void Document(GameApp app, int index)
        {
            string role = Roles[index];
            Rect card = new Rect(52 + 452 * index, 253, 432, 431);
            bool chosen = selected == role;
            bool hovered = card.Contains(Event.current.mousePosition);
            Fill(new Rect(card.x + 4, card.y + 5, card.width, card.height), new Color(.14f, .23f, .21f, .12f));
            Fill(card, chosen ? paper : pale);
            Border(card, chosen ? ink : hovered ? brass : rule, chosen ? 2 : 1);
            Fill(new Rect(card.x + 1, card.y + 1, card.width - 2, 7), accents[index]);
            GUI.Label(new Rect(card.x + 24, card.y + 25, 360, 19), Numerals[index] + "  ·  " + L.Text("ui.start.sponsor." + role), kicker);
            Portrait(index, new Rect(card.x + 20, card.y + 60, 112, 112));
            Label(new Rect(card.x + 141, card.y + 63, 264, 70), "ui.start.role." + role, roleTitle);
            Label(new Rect(card.x + 141, card.y + 142, 264, 54), "ui.start.voice." + role, small);
            Fill(new Rect(card.x + 24, card.y + 203, 384, 1), rule);
            Label(new Rect(card.x + 24, card.y + 224, 384, 25), "ui.start.privilege." + role, footerTitle);
            GUI.Label(new Rect(card.x + 24, card.y + 265, 384, 70), ImmediateText(index), body);
            var terms = openingTerms[index];
            int payment = terms.Fulfil.Food != 0 ? -terms.Fulfil.Food : -terms.Fulfil.Gold;
            GUI.Label(new Rect(card.x + 24, card.y + 348, 384, 64), L.Text("ui.start.later." + role, payment), small);
            if (GUI.Button(card, GUIContent.none, GUIStyle.none) && selected != role)
            { selected = role; app.Feedback("paper"); }
        }

        private string ImmediateText(int index)
        {
            var effect = openingTerms[index].Immediate;
            if (index == 0) return L.Text("ui.start.now.crown", effect.Gold, -effect.Approval);
            if (index == 1) return L.Text("ui.start.now.assembly", -effect.Unrest, effect.Control, -effect.Approval);
            return L.Text("ui.start.now.army", effect.Food, effect.MilitarySupplies, effect.Unrest, -effect.EliteLoyalty);
        }

        private void Portrait(int index, Rect bounds)
        {
            if (!portraits) return;
            int quadrant = index == 2 ? 3 : index;
            Rect uv = new Rect(quadrant % 2 * .5f, quadrant < 2 ? .5f : 0, .5f, .5f);
            if (quadrant == 3)
            {
                float trim = 17f / portraits.width;
                bounds.xMin += bounds.width * trim / uv.width; uv.xMin += trim;
            }
            GUI.DrawTextureWithTexCoords(bounds, portraits, uv, true);
        }

        private void Languages(GameApp app)
        {
            for (int i = 0; i < 2; i++)
            {
                string language = i == 0 ? "ru" : "tr";
                Rect r = new Rect(1244 + i * 67, 48, 59, 32);
                bool active = L.Language == language;
                var style = new GUIStyle(kicker) { alignment = TextAnchor.MiddleCenter };
                GUI.Label(r, language.ToUpperInvariant(), style);
                if (active) Fill(new Rect(r.x + 10, r.yMax, r.width - 20, 2), ink);
                if (GUI.Button(r, GUIContent.none, GUIStyle.none)) app.SetLanguage(language);
            }
        }

        private bool Button(Rect r, string text, bool primary)
        {
            bool hover = r.Contains(Event.current.mousePosition);
            Fill(r, primary ? (hover ? C("#35594C") : ink) : paper);
            if (!primary && hover) Fill(new Rect(r.x, r.yMax - 1, r.width, 1), brass);
            var style = new GUIStyle(primary ? action : small) { alignment = TextAnchor.MiddleCenter };
            GUI.Label(r, text, style);
            return GUI.Button(r, GUIContent.none, GUIStyle.none);
        }

        private void EnsureStyles()
        {
            if (ready) return;
            ready = true;
            serif = Font.CreateDynamicFontFromOSFont(new[] { "Georgia", "Times New Roman", "Liberation Serif" }, 24);
            sans = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "DejaVu Sans" }, 16);
            title = Style(40, ink, true); roleTitle = Style(26, ink, true);
            body = Style(16, ink); small = Style(14, muted); kicker = Style(12, muted);
            footerTitle = Style(20, ink, true); action = Style(23, paper, true);
            portraits = Resources.Load<Texture2D>("Art/PoliticalPortraits-v1");
            for (int i = 0; i < Roles.Length; i++) openingTerms[i] = CampaignCore.GetMandateTerms(CampaignCore.Create(Roles[i]), "ile");
        }

        private GUIStyle Style(int size, Color color, bool historical = false)
        {
            var result = new GUIStyle(GUI.skin.label) { font = historical ? serif : sans, fontSize = size,
                wordWrap = true, richText = false, padding = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0) };
            result.normal.textColor = result.hover.textColor = result.active.textColor = result.focused.textColor = color;
            return result;
        }
        private static Color C(string html) { ColorUtility.TryParseHtmlString(html, out Color result); return result; }
        private static void Fill(Rect r, Color color)
        { Color previous = GUI.color; GUI.color = color; GUI.DrawTexture(r, Texture2D.whiteTexture); GUI.color = previous; }
        private static void Border(Rect r, Color color, float size)
        {
            Fill(new Rect(r.x, r.y, r.width, size), color); Fill(new Rect(r.x, r.yMax - size, r.width, size), color);
            Fill(new Rect(r.x, r.y, size, r.height), color); Fill(new Rect(r.xMax - size, r.y, size, r.height), color);
        }
        private static void Label(Rect r, string key, GUIStyle style) { GUI.Label(r, L.Text(key), style); }
        private void OnDestroy() { if (serif) Destroy(serif); if (sans) Destroy(sans); }
    }
}
