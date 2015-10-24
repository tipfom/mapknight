﻿using System;

using GL = Android.Opengl.GLES20;

using Java.Nio;

namespace mapKnight_Android
{
	namespace CGL
	{
		public class CGLInterface
		{
			FloatBuffer VertexBuffer;
			ShortBuffer IndexBuffer;
			FloatBuffer TextureBuffer;

			int fRenderProgram;

			public bool LeftButtonPressed{ get; private set; }

			public bool RightButtonPressed{ get; private set; }

			public bool JumpButtonPressed{ get; private set; }

			public CGLInterface ()
			{
				fRenderProgram = CGLTools.LoadProgram (GlobalContent.FragmentShaderN, GlobalContent.VertexShaderM);

				GlobalContent.OnUpdate += () => {
					updateVertexBuffer ();
				};

				short[] Indicies = new short[]{ 0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11 };
				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (Indicies.Length * sizeof(short));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				IndexBuffer = bytebuffer.AsShortBuffer ();
				IndexBuffer.Put (Indicies);
				IndexBuffer.Position (0);

				updateVertexBuffer ();
				updateTextureBuffer ();
			}

			private void updateVertexBuffer ()
			{
				fSize movebuttonsize = new fSize (.65f, .65f);
				fSize jumpbuttonsize = new fSize (.9f, .9f);

				float[] verticies = new float[] {
					//button 1
					-GlobalContent.ScreenRatio, -1f + movebuttonsize.Height, 0f,
					-GlobalContent.ScreenRatio, -1f, 0f,
					-GlobalContent.ScreenRatio + movebuttonsize.Width, -1f, 0f,
					-GlobalContent.ScreenRatio + movebuttonsize.Width, -1f + movebuttonsize.Height, 0f,
					//button 2
					-GlobalContent.ScreenRatio + movebuttonsize.Width, -1f + movebuttonsize.Height, 0f,
					-GlobalContent.ScreenRatio + movebuttonsize.Width, -1f, 0f,
					-GlobalContent.ScreenRatio + movebuttonsize.Width + movebuttonsize.Width, -1f, 0f,
					-GlobalContent.ScreenRatio + movebuttonsize.Width + movebuttonsize.Width, -1f + movebuttonsize.Height, 0f,
					//jump button
					GlobalContent.ScreenRatio - jumpbuttonsize.Width, -1f + jumpbuttonsize.Height, 0f,
					GlobalContent.ScreenRatio - jumpbuttonsize.Width, -1f, 0f,
					GlobalContent.ScreenRatio, -1f, 0f,
					GlobalContent.ScreenRatio, -1f + jumpbuttonsize.Height, 0f
				};
				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (verticies.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				VertexBuffer = bytebuffer.AsFloatBuffer ();
				VertexBuffer.Put (verticies);
				VertexBuffer.Position (0);
			}

			private void updateTextureBuffer ()
			{
				float[] TextureCoords = new float[24];

				if (LeftButtonPressed) {
					Array.Copy (GlobalContent.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 0, 8);
				} else {
					Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 0, 8);
				}
				if (RightButtonPressed) {
					Array.Copy (GlobalContent.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 8, 8);
				} else {
					Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 8, 8);
				}
				if (JumpButtonPressed) {
					Array.Copy (GlobalContent.InterfaceSprite.Sprites [1].Verticies, 0, TextureCoords, 16, 8);
				} else {
					Array.Copy (GlobalContent.InterfaceSprite.Sprites [0].Verticies, 0, TextureCoords, 16, 8);
				}

				ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (TextureCoords.Length * sizeof(float));
				bytebuffer.Order (ByteOrder.NativeOrder ());
				TextureBuffer = bytebuffer.AsFloatBuffer ();
				TextureBuffer.Put (TextureCoords);
				TextureBuffer.Position (0);
			}

			public void Draw (float[] mvpMatrix)
			{
				GL.GlUseProgram (fRenderProgram);

				// Set the active texture unit to texture unit 0.

				int PositionHandle = GL.GlGetAttribLocation (fRenderProgram, "vPosition");
				GL.GlEnableVertexAttribArray (PositionHandle);
				GL.GlVertexAttribPointer (PositionHandle, 3, GL.GlFloat, false, 3 * sizeof(float), VertexBuffer);

				int MVPMatrixHandle = GL.GlGetUniformLocation (fRenderProgram, "uMVPMatrix");
				GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, mvpMatrix, 0);

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
			}
		}
	}
}

