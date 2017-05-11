using System;
using mapKnight.Core;
using OpenTK.Graphics.ES20;
using mapKnight.Core.Graphics;

namespace mapKnight.Extended.Graphics {
    public class Framebuffer : IDisposable {
        public readonly Texture2D Texture;
        public readonly Size Size;
        public readonly Vector2 PixelSize;

        private int framebuffer;
        private readonly bool disposeTexture;

        public Framebuffer (int width, int height, bool disposetexture, int interpolationMode = (int)All.Nearest) {
            Size = new Size(width, height);
            PixelSize = new Vector2(1f / width, 1f / height);

            disposeTexture = disposetexture;

            GL.GenFramebuffers(1, out framebuffer);
            Texture = new Texture2D(GL.GenTexture( ), Size);

            GL.BindTexture(TextureTarget.Texture2D, Texture.ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, interpolationMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, interpolationMode);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.Texture2D, Texture.ID, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        ~Framebuffer ( ) {
            Dispose( );
        }

        public void Bind ( ) {
            GL.Viewport(0, 0, Size.Width, Size.Height);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
        }

        public void Unbind ( ) {
            GL.Viewport(0, 0, Window.Size.Width, Window.Size.Height);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose ( ) {
            GL.DeleteFramebuffers(1, ref framebuffer);
            if (disposeTexture) GL.DeleteTexture(Texture.ID);
        }
    }
}
