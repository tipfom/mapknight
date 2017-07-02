using System;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Programs {
    public class ColorProgram : TextureProgram {
        public static ColorProgram Program;

        public static void Init ( ) {
            Program = new ColorProgram( );
        }

        public static void Destroy ( ) {
            Program.Dispose( );
        }

        private UniformMatrixHandle mvpMatrixHandle;
        private AttributeHandle colorHandle;

        public ColorProgram ( ) : base("color.vert", "color.frag") {
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
            Apply(texture.ID, indexbuffer, vertexbuffer, texturebuffer, alphablending);
            colorbuffer.Bind(colorHandle);
            mvpMatrixHandle.Set(matrix.MVP);

            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, new IntPtr(offset));
        }

        public class BufferBatch : IDisposable {
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


            public void Dispose ( ) {
                IndexBuffer.Dispose( );
                VertexBuffer.Dispose( );
                TextureBuffer.Dispose( );
                ColorBuffer.Dispose( );
            }
        }
    }
}
