using System;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Programs {
    public class FBOProgram : TextureProgram {
        public static FBOProgram Program;

        public static void Init ( ) {
            Program = new FBOProgram( );
        }

        public static void Destroy ( ) {
            Program.Dispose( );
        }

        public FBOProgram ( ) : base(Assets.GetVertexShader("normal"), Assets.GetFragmentShader("normal")) {
        }

        public void Draw (MatrixProgram.BufferBatch batch, Texture2D texture, bool alphablending = true) {
            Draw(batch.IndexBuffer, batch.VertexBuffer, batch.TextureBuffer, texture, batch.IndexBuffer.Length, alphablending);
        }

        public void Draw (MatrixProgram.BufferBatch batch, Texture2D texture, int count, bool alphablending = true) {
            Draw(batch.IndexBuffer, batch.VertexBuffer, batch.TextureBuffer, texture, count, alphablending);
        }

        public void Draw(IndexBuffer indexbuffer, IAttributeBuffer vertexbuffer, IAttributeBuffer texturebuffer, Texture2D texture, int count, bool alphablending = true) {
            Apply(texture.ID, indexbuffer, vertexbuffer, texturebuffer, alphablending);
            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }
    }
}
