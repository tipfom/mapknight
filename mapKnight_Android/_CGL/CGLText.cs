using System;
using System.Collections.Generic;

using Android.Graphics;
using GL = Android.Opengl.GLES20;
using Android.Opengl;

using Java.Nio;

namespace mapKnight_Android
{
	namespace CGL
	{
		public class CGLText : IDisposable
		{
			private Paint TextPaint;

			public event EventHandler OnRefresh;

			private string iText;

			public string Text {
				get { return iText; }
				set {
					iText = value;
					Refresh ();
				}
			}

			private int iFontsize;

			public int Fontsize {
				get{ return iFontsize; }
				set {
					iFontsize = value;
					Refresh ();
				}
			}

			private Font iFont;

			public Font Font {
				get { return iFont; }
				set {
					iFont = value;
					Refresh ();
				}
			}

			private Point iPosition;

			public Point Position {
				get { return iPosition; }
				set {
					iPosition = value;
					Refresh ();
				}
			}

			private Color iColor;

			public Color Color {
				get { return iColor; }
				set {
					iColor = value;
					Refresh ();
				}
			}

			public int Width{ get; private set; }

			public int Height{ get; private set; }

			public CGLText (string text, int fontsize, Font font)
			{
				this.iText = text;
				this.iFontsize = fontsize;
				this.iFont = font;
				this.iColor = Color.Black;
				this.Position = new Point (GlobalContent.ScreenSize);

				Refresh ();

				CGLTextContainer.Subscribe (this);
			}

			public CGLText (string text, int fontsize, Font font, Point position, Color color)
			{
				this.iText = text;
				this.iFontsize = fontsize;
				this.iFont = font;
				this.iPosition = position;
				this.iColor = color;

				Refresh ();

				CGLTextContainer.Subscribe (this);
			}

			private void Refresh ()
			{
				TextPaint = new Paint ();
				TextPaint.TextSize = this.Fontsize;
				TextPaint.TextAlign = Paint.Align.Left;
				TextPaint.Color = new Android.Graphics.Color (this.Color.AlphaByte, this.Color.RedByte, this.Color.GreenByte, this.Color.BlueByte);
				TextPaint.SetTypeface (GlobalContent.Fonts [this.Font]);
				TextPaint.TextLocale = new Java.Util.Locale ("de", "de");

				Rect textbounds = new Rect ();
				TextPaint.GetTextBounds (this.Text, 0, this.Text.Length, textbounds);
				this.Height = textbounds.Height ();
				this.Width = textbounds.Width ();

				if (OnRefresh != null)
					OnRefresh (this, EventArgs.Empty);
			}

			public void Dispose ()
			{
				CGLTextContainer.Unsubcribe (this);
			}

			public static class CGLTextContainer
			{
				private static List<CGLText> subscribedItems;

				private static FloatBuffer fVertexBuffer;
				private static FloatBuffer fTextureBuffer;
				private static ShortBuffer fIndexBuffer;

				private static int texture;
				private static int renderprogram;

				private static int CoordsPerVertex = 2;
				private static int VertexStride = CGLTextContainer.CoordsPerVertex * sizeof(float);

				public static void Init ()
				{
					subscribedItems = new List<CGLText> ();

					renderprogram = CGLTools.LoadProgram (GlobalContent.FragmentShaderN, GlobalContent.VertexShaderN);
					initTexture ();
					updateTexture ();
				}

				public static void Draw ()
				{
					GL.GlUseProgram (renderprogram);

					// Set the active texture unit to texture unit 0.

					int PositionHandle = GL.GlGetAttribLocation (renderprogram, "vPosition");
					GL.GlEnableVertexAttribArray (PositionHandle);
					GL.GlVertexAttribPointer (PositionHandle, 3, GL.GlFloat, false, 3 * sizeof(float), fVertexBuffer);

					GL.GlEnable (GL.GlBlend);
					GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

					int mTextureUniformHandle = GL.GlGetUniformLocation (renderprogram, "u_Texture");
					int mTextureCoordinateHandle = GL.GlGetAttribLocation (renderprogram, "a_TexCoordinate");
					GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, fTextureBuffer);
					GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

					GL.GlActiveTexture (GL.GlTexture0);
					GL.GlBindTexture (GL.GlTexture2d, texture);
					GL.GlUniform1i (mTextureUniformHandle, 0);

					GL.GlDrawElements (GL.GlTriangles, 6, GL.GlUnsignedShort, fIndexBuffer);
					GL.GlDisableVertexAttribArray (PositionHandle);
					GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
				}

				public static void Subscribe (CGLText Item)
				{
					if (!subscribedItems.Contains (Item)) {
						Item.OnRefresh += (object sender, EventArgs e) => {
							updateTexture ();
						};
						subscribedItems.Add (Item);
						updateTexture ();
					}
				}

				public static void Unsubcribe (CGLText Item)
				{
					if (subscribedItems.Contains (Item)) {
						subscribedItems.Remove (Item);
						updateTexture ();
					}
				}

				private static void initTexture ()
				{
					int[] temp = new int[1];
					GL.GlGenTextures (1, temp, 0);
					texture = temp [0];

					GL.GlBindTexture (GL.GlTexture2d, texture);
					GL.GlTexImage2D (GL.GlTexture2d, 0, GL.GlRgba, GlobalContent.ScreenSize.Width, GlobalContent.ScreenSize.Height, 0, GL.GlRgba, GL.GlUnsignedByte, null);

					GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
					GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
					GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
					GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

					GL.GlBindTexture (GL.GlTexture2d, 0);

					// init render buffer
					float[] Verticies = new float[] {
						-GlobalContent.ScreenRatio,
						1f,
						0f,
						-GlobalContent.ScreenRatio,
						-1f,
						0f,
						GlobalContent.ScreenRatio,
						-1f,
						0f,
						GlobalContent.ScreenRatio,
						1f,
						0f
					};

					ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (Verticies.Length * sizeof(float));
					bytebuffer.Order (ByteOrder.NativeOrder ());
					fVertexBuffer = bytebuffer.AsFloatBuffer ();
					fVertexBuffer.Put (Verticies);
					fVertexBuffer.Position (0);

					short[] Indicies = new short[]{ 0, 1, 2, 0, 2, 3 };
					bytebuffer = ByteBuffer.AllocateDirect (Indicies.Length * sizeof(short));
					bytebuffer.Order (ByteOrder.NativeOrder ());
					fIndexBuffer = bytebuffer.AsShortBuffer ();
					fIndexBuffer.Put (Indicies);
					fIndexBuffer.Position (0);

					float[] TextureCoords = new float[]{ 0, 1, 0, 0, 1, 0, 1, 1 };
					bytebuffer = ByteBuffer.AllocateDirect (TextureCoords.Length * sizeof(float));
					bytebuffer.Order (ByteOrder.NativeOrder ());
					fTextureBuffer = bytebuffer.AsFloatBuffer ();
					fTextureBuffer.Put (TextureCoords);
					fTextureBuffer.Position (0);

					GlobalContent.OnUpdate += () => {	
						float[] UpdatedVerticies = new float[] {
							-GlobalContent.ScreenRatio,
							1f,
							0f,
							-GlobalContent.ScreenRatio,
							-1f,
							0f,
							GlobalContent.ScreenRatio,
							-1f,
							0f,
							GlobalContent.ScreenRatio,
							1f,
							0f
						};

						bytebuffer = ByteBuffer.AllocateDirect (UpdatedVerticies.Length * sizeof(float));
						bytebuffer.Order (ByteOrder.NativeOrder ());
						fVertexBuffer = bytebuffer.AsFloatBuffer ();
						fVertexBuffer.Put (UpdatedVerticies);
						fVertexBuffer.Position (0);

						updateTexture ();
					};
				}

				private static void updateTexture ()
				{
					Bitmap bitmap = Bitmap.CreateBitmap (GlobalContent.ScreenSize.Width, GlobalContent.ScreenSize.Height, Bitmap.Config.Argb8888);
					bitmap.EraseColor (0);
					Canvas canvas = new Canvas (bitmap);
									
					foreach (CGLText item in subscribedItems) {
						canvas.DrawText (item.Text, item.Position.X, item.Position.Y, item.TextPaint);
					}

					int[] generatedtexture = new int[1];
					GL.GlGenTextures (1, generatedtexture, 0);
					
					GL.GlBindTexture (GL.GlTexture2d, generatedtexture [0]);
					
					GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
					GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
					GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
					GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);
					
					GLUtils.TexImage2D (GL.GlTexture2d, 0, bitmap, 0);
					
					bitmap.Recycle ();
					GL.GlBindTexture (GL.GlTexture2d, 0);

					GL.GlDeleteTextures (1, new int[]{ texture }, 0);
					texture = generatedtexture [0];
				}
			}
		}
	}
}