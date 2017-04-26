using mapKnight.Core;
using OpenTK.Graphics.ES20;
using OpenTK.Platform;

namespace mapKnight.Extended.Graphics {
    public static class Window {
        public delegate void HandleScreenChanged ( );
        public static event HandleScreenChanged Changed;

        private static Color _Background;
        public static Color Background { get { return _Background; } set { _Background = value; UpdateBackgroundColor( ); } }
        public static Vector2 ProjectionSize { get { return new Vector2(Ratio, 1); } }
        public static Size Size;
        public static float Ratio;
        public static IWindowInfo Info;

        public static void Change (int width, int height) {
            GL.Viewport(0, 0, width, height);
            Size = new Size(width, height);
            Ratio = width / (float)height;

            Matrix.Default.UpdateProjection(ProjectionSize);
            Matrix.Default.CalculateMVP( );

            Changed?.Invoke( );
        }

        public static void UpdateBackgroundColor ( ) {
            GL.ClearColor(_Background.R / 255f, _Background.G / 255f, _Background.B / 255f, _Background.A / 255f);
        }
    }
}
