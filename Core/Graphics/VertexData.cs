namespace mapKnight.Core.Graphics {
    public class VertexData {
        public Color Color { get; set; }
        public float[ ] Verticies { get; set; }
        public string Texture { get; set; }

        public VertexData(float[ ] verticies, string texture) : this(verticies, texture, Color.White) {

        }

        public VertexData(float[ ] verticies, string texture, Color color) {
            Verticies = verticies;
            Texture = texture;
            Color = color;
        }
    }
}
