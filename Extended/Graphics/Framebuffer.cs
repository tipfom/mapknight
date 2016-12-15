using System;
using mapKnight.Core;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics {
    class Framebuffer : IDisposable {
        public readonly int Texture;
        public readonly Size Size;

        private int framebuffer;
        private readonly bool disposeTexture;

        public Framebuffer (int width, int height, bool disposetexture) {
            Size = new Size(width, height);
            disposeTexture = disposetexture;

            GL.GenFramebuffers(1, out framebuffer);
            Texture = GL.GenTexture( );

            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.Texture2D, Texture, 0);

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
            if (disposeTexture) GL.DeleteTexture(Texture);
        }
    }
}
