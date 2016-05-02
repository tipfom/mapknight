using Java.Nio;

namespace mapKnight.Android.CGL.Buffer {
    public class CGLColorBufferBatch : CGLBufferBatch {
        public FloatBuffer ColorBuffer { get; }
        public float[ ] ColorData { get; set; }

        public CGLColorBufferBatch (int vertexCount) : base (vertexCount) {
            ColorData = new float[vertexCount * 16];
            ColorBuffer = CGLTools.CreateBuffer (ColorData);
        }

        public override void UpdateBuffer () {
            ColorBuffer.Put (ColorData);
            base.UpdateBuffer ( );
        }
    }
}