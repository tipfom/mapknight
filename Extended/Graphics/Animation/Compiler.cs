using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.Animation {
    public static class Compiler {
        /*
         steps : 
            boneverticies generieren
            texture erstellen
         */

        public static void Compile (VertexAnimation[ ] animations, float[ ] scales, Vector2[ ] offsets, IEnumerable<string> sprites, Entity entity, out float[ ][ ] boneverticies, out SpriteBatch sprite) {
            sprite = SpriteBatch.Combine(false, new List<SpriteBatch>(sprites.Select(item => Assets.Load<SpriteBatch>(item))));
            Debug.CheckGL(typeof(Compiler));
            boneverticies = new float[animations[0].Frames[0].State.Length][ ];
            float entityratio = entity.Transform.Width / entity.Transform.Height;
            for (int i = 0; i < boneverticies.Length; i++) {
                float[ ] texture = sprite.Get(animations[0].Frames[0].State[i].Texture);
                Vector2 textureSize = new Vector2(Math.Abs(texture[4] - texture[0]) * sprite.Width, Math.Abs(texture[1] - texture[3]) * sprite.Height);
                Vector2 vertexSize = new Vector2(textureSize.X * scales[i] / 2f, textureSize.Y * scales[i] / 2f * entityratio);
                Vector2 offset = (offsets[i] - textureSize / 2f) / (textureSize);
                offset.Y *= entityratio;

                boneverticies[i] = new[ ] {
                    -vertexSize.X - offset.X,  vertexSize.Y + offset.Y,
                    -vertexSize.X - offset.X, -vertexSize.Y + offset.Y,
                     vertexSize.X - offset.X, -vertexSize.Y + offset.Y,
                     vertexSize.X - offset.X,  vertexSize.Y + offset.Y
                };
            }
        }
    }
}
