using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Handle;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Programs {
    public class ColorProgram : Program {
        private UniformMatrixHandle mvpMatrixHandle;
        private AttributeHandle colorHandle;

        public ColorProgram ( ) : base(Assets.GetVertexShader("color"), Assets.GetFragmentShader("color")) {
            mvpMatrixHandle = new UniformMatrixHandle(glProgram, "u_mvpmatrix");
            colorHandle = new AttributeHandle(glProgram, "a_color");
        }

        public override void Begin ( ) {
            colorHandle.Enable( );
            base.Begin( );
        }

        public override void End ( ) {
            colorHandle.Disable( );
            base.End( );
        }

        public void Draw (BufferBatch buffer, Texture2D texture, Matrix matrix, bool alphaBlending = true) {
            Draw(buffer, texture, matrix, buffer.IndexBuffer.Length, 0, alphaBlending);
        }

        public void Draw (BufferBatch buffer, Texture2D texture, Matrix matrix, int count, int offset, bool alphaBlending = true) {
            Draw(buffer.IndexBuffer, buffer.VertexBuffer, buffer.TextureBuffer, buffer.ColorBuffer, texture, matrix, count, offset, alphaBlending);
        }

        public void Draw (IndexBuffer indexbuffer, IAttributeBuffer vertexbuffer, IAttributeBuffer texturebuffer, IAttributeBuffer colorbuffer, Texture2D texture, Matrix matrix, int count, int offset, bool alphablending = true) {
            Apply(texture.ID, vertexbuffer, texturebuffer, alphablending);
            colorbuffer.Bind(colorHandle);
            mvpMatrixHandle.Set(matrix.MVP);
            indexbuffer.Bind( );
            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, new IntPtr(offset));
        }

        public static ColorProgram Program;

        public class BufferBatch {
            public IndexBuffer IndexBuffer;
            public IAttributeBuffer VertexBuffer;
            public IAttributeBuffer ColorBuffer;
            public IAttributeBuffer TextureBuffer; 

            public BufferBatch (IndexBuffer indexbuffer, IAttributeBuffer vertexbuffer, IAttributeBuffer colorbuffer, IAttributeBuffer texturebuffer) {
                IndexBuffer = indexbuffer;
                VertexBuffer = vertexbuffer;
                ColorBuffer = colorbuffer;
                TextureBuffer = texturebuffer;
            }
        }
    }
}
