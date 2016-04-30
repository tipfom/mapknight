using System.IO;

using Android.Content;

using mapKnight.Basic;

namespace mapKnight.Android.CGL.Entity {
    public class CGLSet : Set {
        public readonly CGLTexture2D Texture;

        public CGLSet (XMLElemental setConfig, Context context) : base (setConfig) {
            // load texture
            Texture = Assets.LoadTexture (Path.Combine ("sets", Name + ".png"));
        }
    }
}