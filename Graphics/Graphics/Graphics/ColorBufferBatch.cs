namespace mapKnight.Extended.Graphics {
    public class ColorBufferBatch : BufferBatch {
        public float[ ] Color;

        public ColorBufferBatch (int quadCount, int dimensions = 2) : base(quadCount, dimensions) {
            Color = new float[quadCount * 16];
        }
    }
}
