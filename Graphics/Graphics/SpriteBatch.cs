using mapKnight.Core;
using System.Collections.Generic;

namespace mapKnight.Graphics {
    public class SpriteBatch : Texture2D {
        public Dictionary<string, float[]> Sprites { get; private set; } = new Dictionary<string, float[]> ();

        public SpriteBatch (Texture2D texture) : base (texture.ID, texture.Size, texture.Name) { }

        public SpriteBatch (Dictionary<string, int[]> content, Texture2D texture) :
            this (content, texture.ID, texture.Name, texture.Size) {

        }

        public SpriteBatch (Dictionary<string, int[]> content, int id, string name, Size size) : base (id, size, name) {
            foreach (var sprite in content) {
                Add (sprite.Key, sprite.Value);
            }
        }

        public float[] Get (string name) {
            return Sprites[name];
        }

        public void Add (string name, int[] data) {
            float top = (float)(data[1]) / Height;
            float bottom = (float)(data[1] + data[3]) / Height;
            float left = (float)data[0] / Width;
            float right = (float)(data[0] + data[2]) / Width;

            Sprites.Add (name, new float[] { left, top, left, bottom, right, bottom, right, top });
        }
    }
}
