using System;
using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    // Presentation of the existing 0.1 grain petition; no event scheduler or new rules.
    public sealed class PetitionDocument : MonoBehaviour
    {
        private GUIStyle title, body, small, choice;
        private Font serif, sans;
        private static readonly Color Paper=C("#F3E7CA"),Ink=C("#243B37"),Muted=C("#53604D"),Brass=C("#CAB36F"),Wine=C("#58464D");
        private static Color C(string hex){ColorUtility.TryParseHtmlString(hex,out var color);return color;}
        private void Prepare()
        {
            if (title != null) return;
            serif = Font.CreateDynamicFontFromOSFont(new[] { "Georgia", "Times New Roman", "Liberation Serif" }, 30);
            sans = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "DejaVu Sans" }, 17);
            title = Style(34, serif); body = Style(18, sans); small = Style(14, sans); choice = Style(19, serif);
        }
        private GUIStyle Style(int size, Font font)
        {
            var style = new GUIStyle(GUI.skin.label) { fontSize = size, font = font, wordWrap = true, padding = new RectOffset(0, 0, 0, 0) };
            style.normal.textColor=style.hover.textColor=style.active.textColor=style.focused.textColor=Ink;return style;
        }
        private static void Fill(Rect rect, Color color)
        {
            var previous = GUI.color; GUI.color = color; GUI.DrawTexture(rect, Texture2D.whiteTexture); GUI.color = previous;
        }
        private static void Line(Vector2 a, Vector2 b, Color color, float thickness = 1)
        {
            var matrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(Mathf.Atan2(b.y-a.y, b.x-a.x)*Mathf.Rad2Deg, a);
            Fill(new Rect(a.x,a.y,Vector2.Distance(a,b),thickness),color); GUI.matrix = matrix;
        }
        public void DrawLanguageControls(GameApp app)
        {
            if(app==null||app.State==null||!app.State.PendingPetition)return;
            Prepare();
            string[] languages={"ru","tr"};
            var label=new GUIStyle(small){fontSize=13,alignment=TextAnchor.MiddleCenter};
            for(int i=0;i<languages.Length;i++)
            {
                string language=languages[i];var rect=new Rect(1015+i*65,116,55,28);
                bool active=L.Language==language,hover=GUI.enabled&&rect.Contains(Event.current.mousePosition);
                if(active||hover)Fill(rect,active?C("#E6DBB4"):C("#ECE3C3"));
                if(active)Fill(new Rect(rect.x+6,rect.yMax-2,rect.width-12,2),Ink);
                if(GUI.Button(rect,language.ToUpperInvariant(),label))app.SetLanguage(language);
            }
        }
        public void Draw(GameApp app)
        {
            if(app==null||app.State==null||!app.State.PendingPetition)return;
            Prepare();
            Fill(new Rect(0, 0, 1440, 900), new Color(.14f, .23f, .216f, .70f));
            Fill(new Rect(282, 107, 888, 704), new Color(.345f, .275f, .302f, .35f));
            Fill(new Rect(270, 95, 888, 704), Paper);
            Fill(new Rect(295, 119, 3, 652), C("#C98270"));
            var ink = C("#7C8B65");
            // A wheat bundle drawn as ink strokes, not a borrowed emblem or historical portrait.
            for (int i=0; i<7; i++)
            {
                float x=340+i*13, tip=175+Mathf.Abs(i-3)*13;
                Line(new Vector2(382, 396),new Vector2(x,tip),ink,2);
                for(int j=0;j<6;j++)
                {
                    float y=tip+j*11;
                    Line(new Vector2(x,y+16),new Vector2(x-10,y),ink,2);
                    Line(new Vector2(x,y+18),new Vector2(x+10,y+2),ink,2);
                }
            }
            Line(new Vector2(350,346),new Vector2(421,361),ink,3);
            Line(new Vector2(355,363),new Vector2(422,350),ink,2);
            var margin=new GUIStyle(small);margin.fontSize=12;margin.normal.textColor=Muted;
            GUI.Label(new Rect(325,411,130,42),L.Text("petition.margin"),margin);
            GUI.Label(new Rect(480, 128, 510, 24),L.Text("petition.heading"),small);
            GUI.Label(new Rect(480, 169, 615, 83),L.Text("petition.title"),title);
            GUI.Label(new Rect(482, 273, 600, 124),L.Text("petition.body"),body);
            GUI.Label(new Rect(482, 410, 605, 36),L.Text("petition.sender",L.Text("character.lefevre.name")),small);
            Fill(new Rect(324, 460, 778, 1),ink);
            GUI.Label(new Rect(324, 467, 778, 22),L.Text("petition.stock",app.State.Food.ToString("N0",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"))),margin);
            string[] ids = { "relief", "negotiate", "refuse" };
            for(int i=0;i<ids.Length;i++)
            {
                string id=ids[i]; var rect=new Rect(324,493+i*96,778,90);
                bool enabled = id != "relief" || app.State.Food >= 60;
                bool hover=enabled&&GUI.enabled&&rect.Contains(Event.current.mousePosition);
                bool pressed=hover&&Input.GetMouseButton(0);float shift=pressed?1f:0f;
                if(hover){Fill(rect,pressed?C("#D4D5AA"):C("#E2DFC0"));Fill(new Rect(rect.x,rect.y,3,rect.height),Brass);}
                var label = new GUIStyle(choice); label.normal.textColor=enabled?Ink:Muted;
                GUI.Label(new Rect(rect.x+14,rect.y+3+shift,740,28),L.Text("petition.choice."+id),label);
                GUI.Label(new Rect(rect.x+14,rect.y+34+shift,740,36),L.Text("petition.effects."+id),small);
                if(enabled){var arrow=new GUIStyle(choice){alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(rect.xMax-34,rect.y+1+shift,26,28),"→",arrow);}
                if(!enabled)
                {
                    var warning=new GUIStyle(margin);warning.normal.textColor=Wine;
                    GUI.Label(new Rect(rect.x+14,rect.y+71,740,18),L.Text("petition.insufficient",(60-app.State.Food).ToString("N0",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"))),warning);
                }
                bool previous=GUI.enabled;GUI.enabled=previous&&enabled;
                if(GUI.Button(rect,GUIContent.none,GUIStyle.none))app.ChoosePetition(id);
                GUI.enabled=previous;
                Fill(new Rect(rect.x,rect.yMax,rect.width,1),C("#CDC7A3"));
            }
        }
        private void OnDestroy() { if (serif != null) Destroy(serif); if (sans != null) Destroy(sans); }
    }
}
