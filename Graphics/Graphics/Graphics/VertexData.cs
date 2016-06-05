using mapKnight.Core;

namespace mapKnight.Extended.Graphics {
    public struct VertexData {
        public Color Color;
        public float[ ] Verticies;
        public string Texture;
        public int Depth;

        public VertexData (float[ ] verticies, string texture) : this(verticies, texture, 0, Color.White) {

        }

        public VertexData (float[ ] verticies, string texture, int depth, Color color) {
            Verticies = verticies;
            Texture = texture;
            Color = color;
            Depth = depth;
        }
    }
}
