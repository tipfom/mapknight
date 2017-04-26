using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Programs;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.UI {
    public class UIWindow : Screen {
        GPUBuffer VERTEX_BUFFER = new GPUBuffer(2, 4, PrimitiveType.Quad, new float[ ] { -1f, 1f, -1f, -1f, 1f, -1f, 1f, 1f }, BufferUsage.StaticDraw);
        GPUBuffer TEXTURE_BUFFER = new GPUBuffer(2, 4, PrimitiveType.Quad, new float[ ] { 0, 1, 0, 0, 1, 0, 1, 1 }, BufferUsage.StaticDraw);
        IndexBuffer INDEX_BUFFER = new IndexBuffer(1);

        private Framebuffer uiBuffer;

        public UIWindow ( ) {
            uiBuffer = new Framebuffer(Window.Size.Width, Window.Size.Height, true);
        }

        public void FillUIBuffer ( ) {
            Framebuffer cache = new Framebuffer(Window.Size.Width, Window.Size.Height, true);

            uiBuffer.Bind( );
            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UIRenderer.Update(default(DeltaTime));
            UIRenderer.Draw( );

            GaussianBlurProgram.Program.Begin( );
            GaussianBlurProgram.Program.Draw(uiBuffer, cache, true);
            GaussianBlurProgram.Program.End( );
            uiBuffer.Unbind( );

            cache.Dispose( );
        }

        public override void Draw ( ) {
            FBOProgram.Program.Begin( );
            FBOProgram.Program.Draw(INDEX_BUFFER, VERTEX_BUFFER, TEXTURE_BUFFER, uiBuffer.Texture, 6);
            FBOProgram.Program.End( );
            base.Draw( );
        }
    }
}
