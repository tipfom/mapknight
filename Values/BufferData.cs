using System;

namespace mapKnight.Values
{
	public struct BufferData
	{
		public int FrameBuffer;
		public int RenderBuffer;
		public int FrameBufferTexture;

		public int Width;
		public int Height;

		public BufferData (int framebuffer, int renderbuffer, int framebuffertexture, int width, int height) : this ()
		{
			this.FrameBuffer = framebuffer;
			this.RenderBuffer = renderbuffer;
			this.FrameBufferTexture = framebuffertexture;
			this.Width = width;
			this.Height = height;
		}
	}
}

