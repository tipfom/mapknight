using System;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Programs {
    public class MatrixProgram : TextureProgram {
        private UniformMatrixHandle mvpMatrixHandle;

        public MatrixProgram ( ) : base(Assets.GetVertexShader("matrix"), Assets.GetFragmentShader("normal")) {
            mvpMatrixHandle = new UniformMatrixHandle(glProgram, "u_mvpmatrix");
        }

        public void Draw (BufferBatch buffer, Texture2D texture, Matrix matrix, bool alphaBlending = true) {
            Draw(buffer, texture, matrix, buffer.IndexBuffer.Length, alphaBlending);
        }

        public void Draw (BufferBatch buffer, Texture2D texture, Matrix matrix, int count, bool alphaBlending = true) {
            Draw(buffer.IndexBuffer, buffer.VertexBuffer, buffer.TextureBuffer, texture, matrix, count, alphaBlending);
        }

        public void Draw (IndexBuffer indexbuffer, IAttributeBuffer vertexbuffer, IAttributeBuffer texturebuffer, Texture2D texture, Matrix matrix, int count, bool alphablending = true) {
            Apply(texture.ID,indexbuffer, vertexbuffer, texturebuffer, alphablending);
            mvpMatrixHandle.Set(matrix.MVP);

            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }

        public static MatrixProgram Program;

        public class BufferBatch : IDisposable {
            public IndexBuffer IndexBuffer;
            public IAttributeBuffer VertexBuffer;
            public IAttributeBuffer TextureBuffer;

            public BufferBatch (IndexBuffer indexbuffer, IAttributeBuffer vertexbuffer, IAttributeBuffer texturebuffer) {
                IndexBuffer = indexbuffer;
                VertexBuffer = vertexbuffer;
                TextureBuffer = texturebuffer;
            }

            public void Dispose ( ) {
                IndexBuffer.Dispose( );
                VertexBuffer.Dispose( );
                TextureBuffer.Dispose( );
            }
        }
    }
}
