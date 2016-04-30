using System;
using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public class CGLTexture2D : IDisposable {
        public Vector2D Bounds { get; private set; }
        public int Width { get { return Bounds.X; } }
        public int Height { get { return Bounds.Y; } }
        public int Texture { get; private set; }

        public CGLTexture2D (int texture, int width, int height) {
            Bounds = new Vector2D (width, height);
            Texture = texture;
        }

        public void Dispose () {
            CGLTools.DeleteTexture (Texture);
            Bounds = new Vector2D (0, 0);
        }

        public override string ToString () {
            return $"Texture2D Width:{ Width } Height:{ Height }";
        }
    }
}