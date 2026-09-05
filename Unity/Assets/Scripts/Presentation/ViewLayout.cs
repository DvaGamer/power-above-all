using UnityEngine;

namespace PowerAboveAll
{
    // One physical canvas for UI, camera framing and pointer coordinates.
    public static class ViewLayout
    {
        public const float Width = 1440, Height = 900;
        public static readonly Rect BattleViewport = new Rect(0, .19f, 1, .77f);
        public static float Scale => Mathf.Min(Screen.width / Width, Screen.height / Height);
        public static Vector2 Offset => new Vector2((Screen.width - Width * Scale) * .5f, (Screen.height - Height * Scale) * .5f);
        public static Matrix4x4 GuiMatrix => Matrix4x4.TRS(Offset, Quaternion.identity, new Vector3(Scale, Scale, 1));
        public static Vector2 ToCanvas(Vector3 screen)
        { return (new Vector2(screen.x, Screen.height - screen.y) - Offset) / Scale; }
        public static Rect CameraRect(Rect normalizedCanvasRect)
        {
            var offset = Offset;
            return new Rect((offset.x + normalizedCanvasRect.x * Width * Scale) / Screen.width,
                (offset.y + normalizedCanvasRect.y * Height * Scale) / Screen.height,
                normalizedCanvasRect.width * Width * Scale / Screen.width,
                normalizedCanvasRect.height * Height * Scale / Screen.height);
        }
        public static void DrawFrame()
        {
            var offset = Offset;
            if (offset == Vector2.zero) return;
            Color old = GUI.color; GUI.color = new Color(.075f, .12f, .095f);
            if (offset.x > 0)
            {
                GUI.DrawTexture(new Rect(0, 0, offset.x, Screen.height), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(Screen.width - offset.x, 0, offset.x, Screen.height), Texture2D.whiteTexture);
            }
            if (offset.y > 0)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, offset.y), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, Screen.height - offset.y, Screen.width, offset.y), Texture2D.whiteTexture);
            }
            GUI.color = old;
        }
    }
}
