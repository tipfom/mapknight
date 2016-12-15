using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Extended.Graphics.Buffer;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Programs {
    public class FBOProgram : TextureProgram {
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

        public static FBOProgram Program;
    }
}
