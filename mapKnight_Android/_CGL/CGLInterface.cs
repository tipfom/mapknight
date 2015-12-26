using System;

using GL = Android.Opengl.GLES20;
	
using Java.Nio;

using mapKnight.Values;

namespace mapKnight.Android.CGL
{
	public class CGLInterface
	{
		private static Size jumpButtonSize = new Size ((int)(Content.ScreenSize.Height * 0.45f));
		private static Size moveButtonSize = new Size ((int)(Content.ScreenSize.Height * 0.325f));

		delegate void Test ();

		delegate int TestIntVoid (int test222);

		FloatBuffer VertexBuffer;
		ShortBuffer IndexBuffer;
		FloatBuffer TextureBuffer;
		float[] TextureCoords = new float[48];

		int RenderProgram;

		public Button JumpButton;
		public Button LeftButton;
		public Button RightButton;

		public CGLInterface ()
		{
			RenderProgram = CGLTools.GetProgram (Content.FragmentShaderN, Content.VertexShaderM);

			short[] Indicies = new short[] {
				0, 1, 2,
				0, 2, 3, 
				4, 5, 6,
				4, 6, 7,
				8, 9, 10,
				8, 10, 11,
				12, 13, 14,
				12, 14, 15,
				16, 17, 18,
				16, 18, 19,
				20, 21, 22,
				20, 22, 23
			};

			ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (Indicies.Length * sizeof(short));
			bytebuffer.Order (ByteOrder.NativeOrder ());
			IndexBuffer = bytebuffer.AsShortBuffer ();
			IndexBuffer.Put (Indicies);
			IndexBuffer.Position (0);

			updateVertexBuffer ();
			initTextureBuffer ();
			initButtons ();

			Content.OnUpdate += OnUpdate;
		}

		private void updateVertexBuffer ()
		{
			fSize movebuttonsize = new fSize (.65f, .65f);
			fSize jumpbuttonsize = new fSize (.9f, .9f);

			float[] verticies = new float[] {
				//button 1
				Content.ScreenRatio - movebuttonsize.Width, -1f + movebuttonsize.Height, 0f,
				Content.ScreenRatio, -1f + movebuttonsize.Height, 0f,
				Content.ScreenRatio, -1f, 0f,
				Content.ScreenRatio - movebuttonsize.Width, -1f, 0f,
				//button 2
				Content.ScreenRatio - movebuttonsize.Width, -1f, 0f,
				Content.ScreenRatio - movebuttonsize.Width - movebuttonsize.Width, -1f, 0f,
				Content.ScreenRatio - movebuttonsize.Width - movebuttonsize.Width, -1f + movebuttonsize.Height, 0f,
				Content.ScreenRatio - movebuttonsize.Width, -1f + movebuttonsize.Height, 0f,
				//jump button
				-Content.ScreenRatio, -1f, 0f,
				-Content.ScreenRatio, -1f + jumpbuttonsize.Height, 0f,
				-Content.ScreenRatio + jumpbuttonsize.Width, -1f + jumpbuttonsize.Height, 0f,
				-Content.ScreenRatio + jumpbuttonsize.Width, -1f, 0f,
				//health bar
				-Content.ScreenRatio, 1f, 0f,
				-Content.ScreenRatio + Content.ScreenRatio * Content.Character.Health.Current / Content.Character.Health.Max, 1f, 0f,
				-Content.ScreenRatio + Content.ScreenRatio * Content.Character.Health.Current / Content.Character.Health.Max, 0.97f, 0f,
				-Content.ScreenRatio, 0.97f, 0f,
				//energie bar
				// hat zur zeit die weerte der healtbar
				-Content.ScreenRatio, 0.97f, 0f,
				-Content.ScreenRatio + Content.ScreenRatio * Content.Character.Health.Current / Content.Character.Health.Max, 0.97f, 0f,
				-Content.ScreenRatio + Content.ScreenRatio * Content.Character.Health.Current / Content.Character.Health.Max, 0.94f, 0f,
				-Content.ScreenRatio, 0.94f, 0f,
				// back button
				Content.ScreenRatio, 0.8f, 0f,
				Content.ScreenRatio, 1f, 0f,
				Content.ScreenRatio - 0.2f, 1f, 0f,
				Content.ScreenRatio - 0.2f, 0.8f, 0f
			};
			ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (verticies.Length * sizeof(float));
			bytebuffer.Order (ByteOrder.NativeOrder ());
			VertexBuffer = bytebuffer.AsFloatBuffer ();
			VertexBuffer.Put (verticies);
			VertexBuffer.Position (0);
		}

		private void initButtons ()
		{
			JumpButton = Content.TouchManager.Create (new Point (Content.ScreenSize.Width - jumpButtonSize.Width, 0), jumpButtonSize);
			RightButton = Content.TouchManager.Create (new Point (moveButtonSize.Width, 0), moveButtonSize);
			LeftButton = Content.TouchManager.Create (new Point (0, 0), moveButtonSize);

			JumpButton.OnClick += handleJumpButtonClick;
			RightButton.OnClick += handleRightButtonClick;
			LeftButton.OnClick += handleLeftButtonClick;

			JumpButton.OnLeave += handleJumpButtonLeave;
			RightButton.OnLeave += handleRightButtonLeave;
			LeftButton.OnLeave += handleLeftButtonLeave;
		}

		private void handleJumpButtonClick ()
		{
			Array.Copy (Content.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 16, 8);
			updateTextureBuffer ();
		}

		private void handleJumpButtonLeave ()
		{
			Array.Copy (Content.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 16, 8);
			updateTextureBuffer ();
		}

		private void handleLeftButtonClick ()
		{
			Array.Copy (Content.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 0, 8);
			updateTextureBuffer ();
		}

		private void handleLeftButtonLeave ()
		{
			Array.Copy (Content.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 0, 8);
			updateTextureBuffer ();
		}

		private void handleRightButtonClick ()
		{
			Array.Copy (Content.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 8, 8);
			updateTextureBuffer ();
		}

		private void handleRightButtonLeave ()
		{
			Array.Copy (Content.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 8, 8);
			updateTextureBuffer ();
		}

		private void initTextureBuffer ()
		{
			// set textures of buttons first time
			Array.Copy (Content.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 0, 8);
			Array.Copy (Content.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 8, 8);
			Array.Copy (Content.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 16, 8);
			Array.Copy (Content.InterfaceSprite.Sprites [2].Verticies, 0, TextureCoords, 24, 8);
			Array.Copy (Content.InterfaceSprite.Sprites [3].Verticies, 0, TextureCoords, 32, 8);
			Array.Copy (Content.InterfaceSprite.Sprites [4].Verticies, 0, TextureCoords, 40, 8);

			ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (TextureCoords.Length * sizeof(float));
			bytebuffer.Order (ByteOrder.NativeOrder ());
			TextureBuffer = bytebuffer.AsFloatBuffer ();
			TextureBuffer.Put (TextureCoords);
			TextureBuffer.Position (0);
		}

		private void updateTextureBuffer ()
		{
			TextureBuffer.Put (TextureCoords);
			TextureBuffer.Position (0);
		}

		private void updateStatBars ()
		{
			
		}

		public void Draw (float[] mvpMatrix)
		{
			GL.GlUseProgram (RenderProgram);

			// Set the active texture unit to texture unit 0.

			int PositionHandle = GL.GlGetAttribLocation (RenderProgram, "vPosition");
			GL.GlEnableVertexAttribArray (PositionHandle);
			GL.GlVertexAttribPointer (PositionHandle, 3, GL.GlFloat, false, 3 * sizeof(float), VertexBuffer);

			int MVPMatrixHandle = GL.GlGetUniformLocation (RenderProgram, "uMVPMatrix");
			GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, Content.MVPMatrix, 0);

			GL.GlEnable (GL.GlBlend);
			GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

			int mTextureUniformHandle = GL.GlGetUniformLocation (RenderProgram, "u_Texture");
			int mTextureCoordinateHandle = GL.GlGetAttribLocation (RenderProgram, "a_TexCoordinate");
			GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, TextureBuffer);
			GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

			GL.GlActiveTexture (GL.GlTexture0);
			GL.GlBindTexture (GL.GlTexture2d, Content.InterfaceSprite.Texture);
			GL.GlUniform1i (mTextureUniformHandle, 0);

			GL.GlDrawElements (GL.GlTriangles, IndexBuffer.Limit (), GL.GlUnsignedShort, IndexBuffer);
			GL.GlDisableVertexAttribArray (PositionHandle);
			GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
		}

		private void OnUpdate ()
		{
			JumpButton.Dispose ();
			RightButton.Dispose ();
			LeftButton.Dispose ();

			jumpButtonSize = new Size ((int)(Content.ScreenSize.Height * 0.45f));
			moveButtonSize = new Size ((int)(Content.ScreenSize.Height * 0.325f));

			JumpButton = Content.TouchManager.Create (new Point (Content.ScreenSize.Width - jumpButtonSize.Width, 0), jumpButtonSize);
			RightButton = Content.TouchManager.Create (new Point (moveButtonSize.Width, 0), moveButtonSize);
			LeftButton = Content.TouchManager.Create (new Point (0, 0), moveButtonSize);

			JumpButton.OnClick += handleJumpButtonClick;
			RightButton.OnClick += handleRightButtonClick;
			LeftButton.OnClick += handleLeftButtonClick;

			JumpButton.OnLeave += handleJumpButtonLeave;
			RightButton.OnLeave += handleRightButtonLeave;
			LeftButton.OnLeave += handleLeftButtonLeave;

			updateVertexBuffer ();
		}
	}
}