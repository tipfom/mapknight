namespace mapKnight.Graphics {
    public class BufferBatch {
        public float[ ] Verticies;
        public float[ ] Texture;
        public short[ ] Indicies;
        public readonly int Dimesions;
        public readonly int QuadCount;

        public BufferBatch ( int quadCount, int dimensions) {
            Dimesions = dimensions;
            QuadCount = quadCount;
            Verticies = new float[quadCount * 4 * dimensions];
            Texture = new float[quadCount * 8];
            Indicies = GenerateIndexBuffer(quadCount);
        }

        public void ResetVerticies ( ) {
            Verticies = new float[QuadCount * 4 * Dimesions];
        }

        public static short[ ] GenerateIndexBuffer (int quadCount) {
            short[ ] buffer = new short[quadCount * 6];

            for (int i = 0; i < quadCount; i++) {
                buffer[i * 6 + 0] = (short)(i * 4 + 0);
                buffer[i * 6 + 1] = (short)(i * 4 + 1);
                buffer[i * 6 + 2] = (short)(i * 4 + 2);
                buffer[i * 6 + 3] = (short)(i * 4+ 0);
                buffer[i * 6 + 4] = (short)(i * 4 + 2);
                buffer[i * 6 + 5] = (short)(i * 4 + 3);
            }

            return buffer;
        }
    }
}
