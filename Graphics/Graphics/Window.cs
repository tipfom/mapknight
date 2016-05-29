using mapKnight.Core;
using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Graphics {
    public static class Window {
        public delegate void HandleScreenChanged ( );
        public static event HandleScreenChanged Changed;

        public static Size Size;
        public static float Ratio;

        public static void Change(int width, int height) {
            GL.Viewport(0, 0, width, height);
            Size = new Size(width, height);
            Ratio = width / (float)height;
            Changed?.Invoke( );
        }
    }
}
