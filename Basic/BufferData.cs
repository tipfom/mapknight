using System;

namespace mapKnight.Basic
{
	public struct BufferData
	{
		public int FrameBuffer;
		public int RenderBuffer;
		public int Texture;

		public int Width;
		public int Height;

		public BufferData (int framebuffer, int renderbuffer, int framebuffertexture, int width, int height) : this ()
		{
			this.FrameBuffer = framebuffer;
			this.RenderBuffer = renderbuffer;
			this.Texture = framebuffertexture;
			this.Width = width;
			this.Height = height;
		}
	}
}

