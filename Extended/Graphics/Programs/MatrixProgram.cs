using System;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Handle;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Programs {
    public class MatrixProgram : Program {
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
            Apply(texture.ID, vertexbuffer, texturebuffer, alphablending);
            mvpMatrixHandle.Set(matrix.MVP);
            indexbuffer.Bind( );

            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }

        public static MatrixProgram Program;

        public class BufferBatch {
            public IndexBuffer IndexBuffer;
            public IAttributeBuffer VertexBuffer;
            public IAttributeBuffer TextureBuffer;

            public BufferBatch (IndexBuffer indexbuffer, IAttributeBuffer vertexbuffer, IAttributeBuffer texturebuffer) {
                IndexBuffer = indexbuffer;
                VertexBuffer = vertexbuffer;
                TextureBuffer = texturebuffer;
            }
        }
    }
}
