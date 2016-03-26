using System.IO;

using Android.Content;

using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public class CGLSet : Set {
        public readonly int Texture;
        public readonly Size TextureSize;

        public CGLSet (XMLElemental setConfig, Context context) : base (setConfig) {
            // load texture
            CGLTools.LoadedImage limage = CGLTools.LoadImage (Path.Combine ("sets", Name + ".png"));
            Texture = limage.Texture;
            TextureSize = new Size (limage.Width, limage.Height);
        }
    }
}