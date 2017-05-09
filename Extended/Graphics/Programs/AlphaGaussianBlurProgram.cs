using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;
using System;
using static mapKnight.Extended.Graphics.Programs.GaussianBlurProgram;

namespace mapKnight.Extended.Graphics.Programs {
    class AlphaGaussianBlurProgram : TextureProgram {
        public static AlphaGaussianBlurProgram Program;

        public static void Init ( ) {
            Program = new AlphaGaussianBlurProgram( );
        }

        public static void Destroy ( ) {
            Program.Dispose( );
        }

        private UniformVec2Handle pixelOffsetHandle;
        public AlphaGaussianBlurProgram ( ) : base(Assets.GetVertexShader("normal"), Assets.GetFragmentShader("alpha_gauss")) {
            pixelOffsetHandle = new UniformVec2Handle(glProgram, "u_pixel_offset");
        }

        public void Draw (Framebuffer original, Framebuffer cache, bool alphaBlending) {

            cache.Bind( );
            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Apply(original.Texture.ID, INDEX_BUFFER, VERTEX_BUFFER, TEXTURE_BUFFER, alphaBlending);
            pixelOffsetHandle.Set(new float[ ] { original.PixelSize.X, 0f });
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, IntPtr.Zero);

            original.Bind( );
            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Apply(cache.Texture.ID, INDEX_BUFFER, VERTEX_BUFFER, TEXTURE_BUFFER, alphaBlending);
            pixelOffsetHandle.Set(new float[ ] { 0f, original.PixelSize.Y });
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedShort, IntPtr.Zero);
            original.Unbind( );

            Window.UpdateBackgroundColor( );
        }
    }
}
