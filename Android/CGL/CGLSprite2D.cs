using System.Collections.Generic;

namespace mapKnight.Android.CGL {
    class CGLSprite2D : CGLTexture2D {
        private Dictionary<string, float[ ]> sprites;

        public CGLSprite2D (Dictionary<string, int[ ]> content, int texture, int width, int height) : base (texture, width, height) {
            sprites = new Dictionary<string, float[ ]> ( );
            // parse sprite pixel coords to opengl coords
            foreach (KeyValuePair<string, int[ ]> sprite in content) {
                // parse pixel values to opengl values
                // y axis needs to be flipped since opengl 0,0 is bottom  and png 0 is top
                float top = (float)sprite.Value[1] / height;
                float bottom = (float)(sprite.Value[1] + sprite.Value[3]) / height;
                float left = (float)sprite.Value[0] / width;
                float right = (float)(sprite.Value[0] + sprite.Value[2]) / width;

                sprites.Add (sprite.Key, new float[ ] { left, top, left, bottom, right, bottom, right, top });
            }
        }

        public float[ ] Get (string name) {
            return sprites[name];
        }

        public override string ToString () {
            return $"CGLSprite2D containing {sprites.Count} sprites";
        }
    }
}