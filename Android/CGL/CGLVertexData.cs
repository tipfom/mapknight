using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public struct CGLVertexData {
        public float[ ] Verticies;
        public string Texture;
        public Color Color;

        public CGLVertexData (float[ ] verticies, string texture, Color color) : this ( ) {
            Verticies = verticies;
            Texture = texture;
            Color = color;
        }
    }
}