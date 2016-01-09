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
			this.set = set;
			this.boundedPoints = boundedpoints;
			this.animations = animations;
			this.currentAnimation = animations.FindIndex (((CGLAnimation obj) => obj.Action == "default"));
			this.animations [currentAnimation].Start ();

			float[] textureBufferArray = new float[boundedPoints.Count * 8];
			short[] indexBufferArray = new short[boundedPoints.Count * 6];
			float[] vertexBufferArray = new float[boundedPoints.Count * 8];

			for (int i = 0; i < boundedPoints.Count; i++) {
				// set buffer default values

				float[] data = animations [currentAnimation].Default [boundedPoints [i].Name];
				float[] vert = CGLTools.GetVerticies (boundedPoints [i].Size);
				float[] trans = CGLTools.TranslateRotateMirror (CGLTools.GetVerticies (boundedPoints [i].Size), 0, 0, data [0], data [1], data [2], (data [3] == 1f) ? true : false);
				// if data == 1f give mirrored as true, else false
				Array.Copy (trans, 0, vertexBufferArray, i * 8, 8);

				// texture buffer
				textureBufferArray [i * 8 + 0] = boundedPoints [i].TextureRectangle.Right;
				textureBufferArray [i * 8 + 1] = boundedPoints [i].TextureRectangle.Bottom;
				textureBufferArray [i * 8 + 2] = boundedPoints [i].TextureRectangle.Right;
				textureBufferArray [i * 8 + 3] = boundedPoints [i].TextureRectangle.Top;
				textureBufferArray [i * 8 + 4] = boundedPoints [i].TextureRectangle.Left;
				textureBufferArray [i * 8 + 5] = boundedPoints [i].TextureRectangle.Top;
				textureBufferArray [i * 8 + 6] = boundedPoints [i].TextureRectangle.Left;
				textureBufferArray [i * 8 + 7] = boundedPoints [i].TextureRectangle.Bottom;

				// index buffer
				indexBufferArray [i * 6 + 0] = (short)(i * 4 + 0);
				indexBufferArray [i * 6 + 1] = (short)(i * 4 + 1);
				indexBufferArray [i * 6 + 2] = (short)(i * 4 + 2);
				indexBufferArray [i * 6 + 3] = (short)(i * 4 + 0);
				indexBufferArray [i * 6 + 4] = (short)(i * 4 + 2);
				indexBufferArray [i * 6 + 5] = (short)(i * 4 + 3);
			}

			textureBuffer = CGLTools.CreateBuffer (textureBufferArray);
			indexBuffer = CGLTools.CreateBuffer (indexBufferArray);
			vertexBuffer = CGLTools.CreateBuffer (vertexBufferArray);
			renderProgram = CGLTools.GetProgram (Content.FragmentShaderN, Content.VertexShaderM);

			activeEntitys.Add (this); // register entity
		}

		public static void Draw (int deltatime)
		{
			foreach (CGLEntity entity in activeEntitys) {
				entity.Update (deltatime);
				entity.Draw (); // das wird bald verbessert du fauler hund!!!!
			}
		}

		private void Draw ()
		{
			GL.GlUseProgram (renderProgram);

			// Set the active texture unit to texture unit 0.

			int PositionHandle = GL.GlGetAttribLocation (renderProgram, "vPosition");
			GL.GlEnableVertexAttribArray (PositionHandle);
			GL.GlVertexAttribPointer (PositionHandle, Content.CoordsPerVertex2D, GL.GlFloat, false, Content.VertexStride2D, vertexBuffer);

			int MVPMatrixHandle = GL.GlGetUniformLocation (renderProgram, "uMVPMatrix");
			GL.GlUniformMatrix4fv (MVPMatrixHandle, 1, false, Content.MVPMatrix, 0);

			GL.GlEnable (GL.GlBlend);
			GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);

			int mTextureUniformHandle = GL.GlGetUniformLocation (renderProgram, "u_Texture");
			int mTextureCoordinateHandle = GL.GlGetAttribLocation (renderProgram, "a_TexCoordinate");
			GL.GlVertexAttribPointer (mTextureCoordinateHandle, 2, GL.GlFloat, false, 0, textureBuffer);
			GL.GlEnableVertexAttribArray (mTextureCoordinateHandle);

			GL.GlActiveTexture (GL.GlTexture0);
			GL.GlBindTexture (GL.GlTexture2d, set.Texture);
			GL.GlUniform1i (mTextureUniformHandle, 0);

			GL.GlDrawElements (GL.GlTriangles, boundedPoints.Count * 6, GL.GlUnsignedShort, indexBuffer);
			GL.GlDisableVertexAttribArray (PositionHandle);
			GL.GlDisableVertexAttribArray (mTextureCoordinateHandle);
		}

		public bool Animate (string animation)
		{
			if (animations [currentAnimation].Abortable == true || animations [currentAnimation].Finished == true) {
				currentAnimation = animations.IndexOf (animations.Find (((CGLAnimation obj) => obj.Action == animation)));
				return true;
			} else
				return false;
		}

		private void Update (int deltatime)
		{
			if (animations [currentAnimation].Finished) {
				if (animations [currentAnimation].Loopable)
					animations [currentAnimation].Start ();
			} else {
				animations [currentAnimation].Step (deltatime);
			}

			// begin translating the current animationdata
			for (int i = 0; i < boundedPoints.Count; i++) {
				float[] data = animations [currentAnimation].Current [boundedPoints [i].Name];
				vertexBuffer.Put (CGLTools.TranslateRotateMirror (CGLTools.GetVerticies (boundedPoints [i].Size), 0, 0, data [0], data [1], data [2], (data [3] == 1f) ? true : false));
			}
			vertexBuffer.Position (0);
		}

		~CGLEntity ()
		{
			// destruktor
			// wenns hier fehler, auf erik zukommen (keine destruktor)
			activeEntitys.Remove (this); // unregister entity
		}
	}
}