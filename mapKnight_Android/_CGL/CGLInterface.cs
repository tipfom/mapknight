using System;

using GL = Android.Opengl.GLES20;
using Button = mapKnight_Android.ButtonManager.Button;
	
using Java.Nio;

namespace mapKnight_Android
{
	namespace CGL
	{
		public class CGLInterface
		{
			FloatBuffer fVertexBuffer;
			ShortBuffer fIndexBuffer;
			FloatBuffer fTextureBuffer;

			FloatBuffer VertexBuffer;
			ShortBuffer IndexBuffer;
			FloatBuffer TextureBuffer;
			ByteBuffer TextureByteBuffer;
			float[] TextureCoords = new float[24];

			int fRenderProgram;

			BufferData framebuffer;

			public Button JumpButton;
			public Button LeftButton;
			public Button RightButton;

			private bool FrameBufferUpdateRequired = false;

			public CGLInterface ()
			{
				fRenderProgram = CGLTools.GetProgram (GlobalContent.FragmentShaderN, GlobalContent.VertexShaderM);

				short[] Indicies = new short[]{ 0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11 };
				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (Indicies.Length * sizeof(short));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				IndexBuffer = bytebuffer.AsShortBuffer ();
				IndexBuffer.Put (Indicies);
				IndexBuffer.Position (0);

				Indicies = new short[]{ 0, 1, 2, 0, 2, 3 };
				bytebuffer = ByteBuffer.AllocateDirect (Indicies.Length * sizeof(short));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				fIndexBuffer = bytebuffer.AsShortBuffer ();
				fIndexBuffer.Put (Indicies);
				fIndexBuffer.Position (0);

				updateVertexBuffer ();
				initTextureBuffer ();
				initButtons ();
				updateFramebuffer ();
				drawFramebuffer ();

				GlobalContent.OnUpdate += () => {
					updateVertexBuffer ();
					updateFramebuffer ();
					drawFramebuffer ();
				};
			}

			private void updateVertexBuffer ()
			{
				fSize movebuttonsize = new fSize (.65f, .65f);
				fSize jumpbuttonsize = new fSize (.9f, .9f);

				float[] verticies = new float[] {
					//button 1
					-GlobalContent.ScreenRatio + movebuttonsize.Width, -1f + movebuttonsize.Height, 0f,
					-GlobalContent.ScreenRatio, -1f + movebuttonsize.Height, 0f,
					-GlobalContent.ScreenRatio, -1f, 0f,
					-GlobalContent.ScreenRatio + movebuttonsize.Width, -1f, 0f,
					//button 2
					-GlobalContent.ScreenRatio + movebuttonsize.Width, -1f, 0f,
					-GlobalContent.ScreenRatio + movebuttonsize.Width + movebuttonsize.Width, -1f, 0f,
					-GlobalContent.ScreenRatio + movebuttonsize.Width + movebuttonsize.Width, -1f + movebuttonsize.Height, 0f,
					-GlobalContent.ScreenRatio + movebuttonsize.Width, -1f + movebuttonsize.Height, 0f,
					//jump button
					GlobalContent.ScreenRatio, -1f, 0f,
					GlobalContent.ScreenRatio, -1f + jumpbuttonsize.Height, 0f,
					GlobalContent.ScreenRatio - jumpbuttonsize.Width, -1f + jumpbuttonsize.Height, 0f,
					GlobalContent.ScreenRatio - jumpbuttonsize.Width, -1f, 0f
				};
				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (verticies.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				VertexBuffer = bytebuffer.AsFloatBuffer ();
				VertexBuffer.Put (verticies);
				VertexBuffer.Position (0);
			}

			private void initButtons ()
			{
				Size jumpButtonSize = new Size ((int)(GlobalContent.ScreenSize.Height * 0.45f));
				Size moveButtonSize = new Size ((int)(GlobalContent.ScreenSize.Height * 0.325f));

				JumpButton = GlobalContent.TouchManager.Create (new Point (GlobalContent.ScreenSize.Width - jumpButtonSize.Width, 0), jumpButtonSize);
				RightButton = GlobalContent.TouchManager.Create (new Point (moveButtonSize.Width, 0), moveButtonSize);
				LeftButton = GlobalContent.TouchManager.Create (new Point (0, 0), moveButtonSize);

				JumpButton.OnClick += handleJumpButtonClick;
				RightButton.OnClick += handleRightButtonClick;
				LeftButton.OnClick += handleLeftButtonClick;

				JumpButton.OnLeave += handleJumpButtonLeave;
				RightButton.OnLeave += handleRightButtonLeave;
				LeftButton.OnLeave += handleLeftButtonLeave;

				GlobalContent.OnUpdate += () => {
					JumpButton.Dispose ();
					RightButton.Dispose ();
					LeftButton.Dispose ();

					jumpButtonSize = new Size ((int)(GlobalContent.ScreenSize.Height * 0.45f));
					moveButtonSize = new Size ((int)(GlobalContent.ScreenSize.Height * 0.325f));

					JumpButton = GlobalContent.TouchManager.Create (new Point (GlobalContent.ScreenSize.Width - jumpButtonSize.Width, 0), jumpButtonSize);
					RightButton = GlobalContent.TouchManager.Create (new Point (moveButtonSize.Width, 0), moveButtonSize);
					LeftButton = GlobalContent.TouchManager.Create (new Point (0, 0), moveButtonSize);

					JumpButton.OnClick += handleJumpButtonClick;
					RightButton.OnClick += handleRightButtonClick;
					LeftButton.OnClick += handleLeftButtonClick;

					JumpButton.OnLeave += handleJumpButtonLeave;
					RightButton.OnLeave += handleRightButtonLeave;
					LeftButton.OnLeave += handleLeftButtonLeave;
				};
			}

			private void handleJumpButtonClick ()
			{
				Array.Copy (GlobalContent.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 16, 8);
				updateTextureBuffer ();
			}

			private void handleJumpButtonLeave ()
			{
				Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 16, 8);
				updateTextureBuffer ();
			}

			private void handleLeftButtonClick ()
			{
				Array.Copy (GlobalContent.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 0, 8);
				updateTextureBuffer ();
			}

			private void handleLeftButtonLeave ()
			{
				Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 0, 8);
				updateTextureBuffer ();
			}

			private void handleRightButtonClick ()
			{
				Array.Copy (GlobalContent.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 8, 8);
				updateTextureBuffer ();
			}

			private void handleRightButtonLeave ()
			{
				Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 8, 8);
				updateTextureBuffer ();
			}

			private void initTextureBuffer ()
			{
				Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 0, 8);
				Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 8, 8);
				Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 16, 8);

				TextureByteBuffer = ByteBuffer.AllocateDirect (24 * sizeof(float));
				TextureByteBuffer.Order (ByteOrder.NativeOrder ());
				TextureBuffer = TextureByteBuffer.AsFloatBuffer ();
				TextureBuffer.Put (TextureCoords);
				TextureBuffer.Position (0);
			}

			private void updateTextureBuffer ()
			{
				TextureBuffer = TextureByteBuffer.AsFloatBuffer ();
				TextureBuffer.Put (TextureCoords);
				TextureBuffer.Position (0);
				FrameBufferUpdateRequired = true;
			}

			//			private void updateTextureBuffer ()
			//			{
			//				float[] TextureCoords = new float[24];
			//
			//				if (LeftButton.Clicked) {
			//					Array.Copy (GlobalContent.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 0, 8);
			//				} else {
			//					Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 0, 8);
			//				}
			//				if (RightButton.Clicked) {
			//					Array.Copy (GlobalContent.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 8, 8);
			//				} else {
			//					Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 8, 8);
			//				}
			//				if (JumpButton.Clicked) {
			//					Array.Copy (GlobalContent.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 16, 8);
			//				} else {
			//					Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 16, 8);
			//				}
			//
			//				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (TextureCoords.Length * sizeof(float));
			//				bytebuffer.Order (ByteOrder.NativeOrder ());
			//				TextureBuffer = bytebuffer.AsFloatBuffer ();
			//				TextureBuffer.Put (TextureCoords);
			//				TextureBuffer.Position (0);
			//			}

			private void updateFramebuffer ()
			{
				framebuffer = CGLTools.GenerateFramebuffer ();

				float[] verticies = new float[] {
					-GlobalContent.ScreenRatio, 1f, 0f,
					-GlobalContent.ScreenRatio, -1f, 0f,
					GlobalContent.ScreenRatio, -1f, 0f,
					GlobalContent.ScreenRatio, 1f, 0f
				};
				float[] textureCoords = new float[] {
					0f, 1f,
					0f, 0f,
					1f, 0f,
					1f, 1f
				};

				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (verticies.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				fVertexBuffer = bytebuffer.AsFloatBuffer ();
				fVertexBuffer.Put (verticies);
				fVertexBuffer.Position (0);

				bytebuffer = ByteBuffer.AllocateDirect (textureCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				fTextureBuffer = bytebuffer.AsFloatBuffer ();
				fTextureBuffer.Put (textureCoords);
				fTextureBuffer.Position (0);
			}

			private void drawFramebuffer ()
			{
				GL.GlBindFramebuffer (GL.GlFramebuffer, framebuffer.FrameBuffer);
				GL.GlClearColor (1.0f, 1.0f, 1.0f, 0.0f);
				GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

				GL.GlUseProgram (fRenderProgram);

				// Set the active texture unit to texture unit 0.

				int PositionHandle = GL.GlGetAttribLocation (fRenderProgram, "vPosition");
				GL.GlEnableVertexAttribArray (PositionHandle);
				GL.GlVertexAttribPointer (PositionHandle, 3, GL.GlFloat, false, 3 * sizeof(float), VertexBuffer);

				int MVPMatrixHandle = GL.GlGetUniformLocation (fRenderProgram, "uMVPMatrix");
				GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, GlobalContent.MVPMatrix, 0);

				GL.GlEnable (GL.GlBlend);
				GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

				int mTextureUniformHandle = GL.GlGetUniformLocation (fRenderProgram, "u_Texture");
				int mTextureCoordinateHandle = GL.GlGetAttribLocation (fRenderProgram, "a_TexCoordinate");
				GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, TextureBuffer);
				GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

				GL.GlActiveTexture (GL.GlTexture1);
				GL.GlBindTexture (GL.GlTexture2d, GlobalContent.InterfaceSprite.Texture);
				GL.GlUniform1i (mTextureUniformHandle, 1);

				GL.GlDrawElements (GL.GlTriangles, 18, GL.GlUnsignedShort, IndexBuffer);
				GL.GlDisableVertexAttribArray (PositionHandle);
				GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);

				GL.GlBindFramebuffer (GL.GlFramebuffer, 0);
			}

			public void Draw (float[] mvpMatrix)
			{
				if (FrameBufferUpdateRequired) {
					drawFramebuffer ();
					FrameBufferUpdateRequired = false;
				}

				GL.GlUseProgram (fRenderProgram);

				// Set the active texture unit to texture unit 0.

				int PositionHandle = GL.GlGetAttribLocation (fRenderProgram, "vPosition");
				GL.GlEnableVertexAttribArray (PositionHandle);
				GL.GlVertexAttribPointer (PositionHandle, 3, GL.GlFloat, false, 3 * sizeof(float), fVertexBuffer);

				int MVPMatrixHandle = GL.GlGetUniformLocation (fRenderProgram, "uMVPMatrix");
				GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, mvpMatrix, 0);

				GL.GlEnable (GL.GlBlend);
				GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

				int mTextureUniformHandle = GL.GlGetUniformLocation (fRenderProgram, "u_Texture");
				int mTextureCoordinateHandle = GL.GlGetAttribLocation (fRenderProgram, "a_TexCoordinate");
				GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, fTextureBuffer);
				GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

				GL.GlActiveTexture (GL.GlTexture1);
				GL.GlBindTexture (GL.GlTexture2d, framebuffer.FrameBufferTexture);
				GL.GlUniform1i (mTextureUniformHandle, 1);

				GL.GlDrawElements (GL.GlTriangles, 6, GL.GlUnsignedShort, fIndexBuffer);
				GL.GlDisableVertexAttribArray (PositionHandle);
				GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
			}
		}
	}
}