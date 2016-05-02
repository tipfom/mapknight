using Java.Nio;

namespace mapKnight.Android.CGL.Buffer {
    public class CGLBufferBatch {
        public FloatBuffer VertexBuffer { get; private set; }
        public FloatBuffer TextureBuffer { get; private set; }
        public ShortBuffer IndexBuffer { get; private set; }

        public float[ ] VertexData { get; set; }
        public float[ ] TextureData { get; set; }

        public CGLBufferBatch (int vertexCount) {
            VertexData = new float[vertexCount * 8];
            VertexBuffer = CGLTools.CreateBuffer (VertexData);
            TextureData = new float[vertexCount * 8];
            TextureBuffer = CGLTools.CreateBuffer (TextureData);

            short[ ] indexBufferData = new short[vertexCount * 6];
            for (int i = 0; i < vertexCount; i++) {
                indexBufferData[i * 6 + 0] = (short)(i * 4 + 0);
                indexBufferData[i * 6 + 1] = (short)(i * 4 + 1);
                indexBufferData[i * 6 + 2] = (short)(i * 4 + 2);
                indexBufferData[i * 6 + 3] = (short)(i * 4 + 0);
                indexBufferData[i * 6 + 4] = (short)(i * 4 + 2);
                indexBufferData[i * 6 + 5] = (short)(i * 4 + 3);
            }
            IndexBuffer = CGLTools.CreateBuffer (indexBufferData);
        }

        public virtual void UpdateBuffer () {
            TextureBuffer.Put (TextureData);
            VertexBuffer.Put (VertexData);
        }
    }
}