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
        private void Prepare()
        {
            if (title != null) return;
            serif = Font.CreateDynamicFontFromOSFont(new[] { "Georgia", "Times New Roman" }, 30);
            sans = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI" }, 17);
            title = Style(34, serif); body = Style(18, sans); small = Style(14, sans); choice = Style(20, serif);
        }
        private GUIStyle Style(int size, Font font)
        {
            var style = new GUIStyle(GUI.skin.label) { fontSize = size, font = font, wordWrap = true, padding = new RectOffset(0, 0, 0, 0) };
            style.normal.textColor = new Color(.16f, .22f, .17f); return style;
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
        public void Draw(GameApp app)
        {
            Prepare();
            Fill(new Rect(0, 0, 1440, 900), new Color(.07f, .13f, .1f, .67f));
            Fill(new Rect(282, 107, 888, 704), new Color(.04f, .08f, .05f, .3f));
            Fill(new Rect(270, 95, 888, 704), new Color(.94f, .916f, .849f));
            Fill(new Rect(295, 119, 3, 652), new Color(.64f, .27f, .20f, .65f));
            var ink = new Color(.37f, .40f, .29f, .65f);
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
            GUI.Label(new Rect(480, 128, 620, 24),L.Text("petition.heading"),small);
            GUI.Label(new Rect(480, 169, 615, 83),L.Text("petition.title"),title);
            GUI.Label(new Rect(482, 273, 600, 124),L.Text("petition.body"),body);
            GUI.Label(new Rect(482, 410, 605, 32),L.Text("character.lefevre.name")+" · "+L.Text("character.lefevre.position"),small);
            Fill(new Rect(324, 460, 778, 1),ink);
            string[] ids = { "relief", "negotiate", "refuse" };
            for(int i=0;i<ids.Length;i++)
            {
                string id=ids[i]; var rect=new Rect(324,480+i*92,778,82);
                bool enabled = id != "relief" || app.State.Food >= 60;
                bool hover=rect.Contains(Event.current.mousePosition);
                if(hover)Fill(rect,new Color(.76f,.77f,.64f,.34f));
                var label = new GUIStyle(choice); label.normal.textColor=enabled?new Color(.15f,.24f,.18f):new Color(.51f,.47f,.40f);
                GUI.Label(new Rect(rect.x+14,rect.y+4,740,30),L.Text("petition.choice."+id),label);
                GUI.Label(new Rect(rect.x+14,rect.y+39,740,40),L.Text(!enabled?"petition.insufficient":"petition.effects."+id),small);
                bool previous=GUI.enabled;GUI.enabled=previous&&enabled;
                if(GUI.Button(rect,GUIContent.none,GUIStyle.none))app.ChoosePetition(id);
                GUI.enabled=previous;
                Fill(new Rect(rect.x,rect.yMax,rect.width,1),new Color(.54f,.57f,.45f,.25f));
            }
        }
        private void OnDestroy() { if (serif != null) Destroy(serif); if (sans != null) Destroy(sans); }
    }
}
