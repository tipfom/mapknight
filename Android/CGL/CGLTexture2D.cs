using System;
using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public class CGLTexture2D : IDisposable {
        public Size Bounds { get; private set; }
        public int Width { get { return Bounds.Width; } }
        public int Height { get { return Bounds.Height; } }
        public int Texture { get; private set; }
        public string Name { get; private set; }

        public CGLTexture2D (int texture, string name, int width, int height) {
            Bounds = new Size (width, height);
            Texture = texture;
            Name = name;
        }

        public void Dispose () {
            CGLTools.DeleteTexture (Texture);
            Bounds = new Size (0, 0);
            Name = null;
        }

        public override string ToString () {
            return $"Texture2D Width:{ Width } Height:{ Height }";
        }
    }
}