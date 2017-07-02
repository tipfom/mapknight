using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;
using System;

namespace mapKnight.Extended.Graphics.Programs {
    public class GaussianBlurProgram : TextureProgram {
        public static GaussianBlurProgram Program;

        public static GPUBuffer VERTEX_BUFFER;
        public static GPUBuffer TEXTURE_BUFFER;
        public static IndexBuffer INDEX_BUFFER;

        public static void Init( ) {
            VERTEX_BUFFER = new GPUBuffer(2, 4, PrimitiveType.Quad, new float[ ] { -1f, 1f, -1f, -1f, 1f, -1f, 1f, 1f }, BufferUsage.StaticDraw);
            TEXTURE_BUFFER = new GPUBuffer(2, 4, PrimitiveType.Quad, new float[ ] { 0, 1, 0, 0, 1, 0, 1, 1 }, BufferUsage.StaticDraw);
            INDEX_BUFFER = new IndexBuffer(1);
            Program = new GaussianBlurProgram( );
        }

        public static void Destroy ( ) {
            Program.Dispose( );
            VERTEX_BUFFER.Dispose( );
            TEXTURE_BUFFER.Dispose( );
            INDEX_BUFFER.Dispose( );
        }

        private UniformVec2Handle pixelOffsetHandle;

        public GaussianBlurProgram ( ) : base("normal.vert", "gauss.frag") {
            pixelOffsetHandle = new UniformVec2Handle(glProgram, "u_pixel_offset");
        }

        public void Draw (Framebuffer original, Framebuffer cache, bool alphaBlending) {
            GL.ClearColor(0f, 0f, 0f, 0f);

            cache.Bind( );
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Apply(original.Texture.ID, INDEX_BUFFER, VERTEX_BUFFER, TEXTURE_BUFFER, alphaBlending);
            pixelOffsetHandle.Set(new float[ ] { original.PixelSize.X, 0f });
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, IntPtr.Zero);

            original.Bind( );
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Apply(cache.Texture.ID, INDEX_BUFFER, VERTEX_BUFFER, TEXTURE_BUFFER, alphaBlending);
            pixelOffsetHandle.Set(new float[ ] { 0f, original.PixelSize.Y });
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, IntPtr.Zero);
            original.Unbind( );

            Window.UpdateBackgroundColor( );
        }
    }
}
