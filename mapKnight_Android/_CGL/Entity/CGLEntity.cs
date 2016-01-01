using System;
using System.Collections.Generic;

using Java.Nio;

using GL = Android.Opengl.GLES20;

using mapKnight.Values;
using mapKnight.Utils;
using mapKnight.Entity;

namespace mapKnight.Android.CGL.Entity
{
	public class CGLEntity : mapKnight.Entity.Entity
	{
		private static List<CGLEntity> activeEntitys = new List<CGLEntity> ();

		private FloatBuffer vertexBuffer;
		private ShortBuffer indexBuffer;
		private FloatBuffer textureBuffer;
		private int renderProgram;

		protected List<CGLBoundedPoint> boundedPoints;

		protected CGLSet set;
		protected List<CGLAnimation> animations;
		private int currentAnimation;

		public CGLEntity (int health, Point position, string name, List<CGLBoundedPoint>  boundedpoints, List<CGLAnimation> animations, CGLSet set) : base (health, position, name) //XMLElemental entityConfig
		{
			activeEntitys.Add (this); // register entity

			this.set = set;
			this.boundedPoints = boundedpoints;
			this.currentAnimation = animations.FindIndex (((CGLAnimation obj) => obj.Action == "default"));

			float[] vertexBufferArray = new float[boundedPoints.Count * 8];
			float[] textureBufferArray = new float[boundedPoints.Count * 8];
			short[] indexBufferArray = new short[boundedPoints.Count * 6];

			for (int i = 0; i < boundedPoints.Count; i++) {
				// set buffer default values

				// vertex buffer
				vertexBufferArray = animations [currentAnimation].Default;

				// texture buffer
				textureBufferArray [i * 8 + 0] = boundedPoints [i].TextureRectangle.Top;
				textureBufferArray [i * 8 + 1] = boundedPoints [i].TextureRectangle.Left;
				textureBufferArray [i * 8 + 2] = boundedPoints [i].TextureRectangle.Bottom;
				textureBufferArray [i * 8 + 3] = boundedPoints [i].TextureRectangle.Left;
				textureBufferArray [i * 8 + 4] = boundedPoints [i].TextureRectangle.Bottom;
				textureBufferArray [i * 8 + 5] = boundedPoints [i].TextureRectangle.Right;
				textureBufferArray [i * 8 + 6] = boundedPoints [i].TextureRectangle.Top;
				textureBufferArray [i * 8 + 7] = boundedPoints [i].TextureRectangle.Right;

				// index buffer
				indexBufferArray [i * 6 + 0] = (short)(i * 3 + 0);
				indexBufferArray [i * 6 + 1] = (short)(i * 3 + 1);
				indexBufferArray [i * 6 + 2] = (short)(i * 3 + 2);
				indexBufferArray [i * 6 + 3] = (short)(i * 3 + 0);
				indexBufferArray [i * 6 + 4] = (short)(i * 3 + 2);
				indexBufferArray [i * 6 + 5] = (short)(i * 3 + 3);
			}

			vertexBuffer = CGLTools.CreateBuffer (vertexBufferArray);
			textureBuffer = CGLTools.CreateBuffer (textureBufferArray);
			indexBuffer = CGLTools.CreateBuffer (indexBufferArray);
			renderProgram = CGLTools.GetProgram (Content.FragmentShaderN, Content.VertexShaderM);
		}

		public static void Draw ()
		{
			foreach (CGLEntity entity in activeEntitys) {
				entity.Draw (0); // das wird bald verbessert du fauler hund!!!!
			}
		}

		private void Draw (int attribute)
		{
			GL.GlUseProgram (renderProgram);

			// Set the active texture unit to texture unit 0.

			int PositionHandle = GL.GlGetAttribLocation (renderProgram, "vPosition");
			GL.GlEnableVertexAttribArray (PositionHandle);
			GL.GlVertexAttribPointer (PositionHandle, 3, GL.GlFloat, false, 3 * sizeof(float), vertexBuffer);

			int MVPMatrixHandle = GL.GlGetUniformLocation (renderProgram, "uMVPMatrix");
			GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, Content.MVPMatrix, 0);

			GL.GlEnable (GL.GlBlend);
			GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

			int mTextureUniformHandle = GL.GlGetUniformLocation (renderProgram, "u_Texture");
			int mTextureCoordinateHandle = GL.GlGetAttribLocation (renderProgram, "a_TexCoordinate");
			GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, textureBuffer);
			GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

			GL.GlActiveTexture (GL.GlTexture2);
			GL.GlBindTexture (GL.GlTexture2d, set.Texture);
			GL.GlUniform1i (mTextureUniformHandle, 2);

			GL.GlDrawElements (GL.GlTriangles, 6, GL.GlUnsignedShort, indexBuffer);
			GL.GlDisableVertexAttribArray (PositionHandle);
			GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
		}

		private void Update ()
		{
			
		}

		~CGLEntity ()
		{
			// destruktor
			activeEntitys.Remove (this); // unregister entity
		}
	}
}