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
            sprite = Assets.Load<SpriteBatch>(sprites.ToArray( )[0]);
            Debug.CheckGL(typeof(Compiler));
            boneverticies = new float[animations[0].Frames[0].State.Length][ ];
            float entityratio = entity.Transform.Width / entity.Transform.Height;
            for (int i = 0; i < boneverticies.Length; i++) {
                float[ ] texture = sprite.Get(animations[0].Frames[0].State[i].Texture);
                float textureWidth = Math.Abs(texture[4] - texture[0]) * sprite.Width;
                float textureHeight = Math.Abs(texture[1] - texture[3]) * sprite.Height;
                float vertexWidth = textureWidth * scales[i] / 2f;
                float vertexHeight = textureHeight * scales[i] / 2f * entityratio;
                Vector2 offset = offsets[i] * scales[i];
                offset.Y *= entityratio;

                boneverticies[i] = new[ ] {
                    -vertexWidth + offset.X, vertexHeight + offset.Y,
                    -vertexWidth + offset.X, -vertexHeight + offset.Y,
                    vertexWidth + offset.X, -vertexHeight + offset.Y,
                    vertexWidth + offset.X, vertexHeight + offset.Y
                };
            }
        }
    }
}
