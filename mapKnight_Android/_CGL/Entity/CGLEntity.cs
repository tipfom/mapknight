using System;
using System.Collections.Generic;

using Java.Nio;

using GL = Android.Opengl.GLES20;

using mapKnight.Values;
using mapKnight.Utils;
using mapKnight.Entity;

namespace mapKnight.Android.CGL.Entity
{
	public class CGLEntity : Entity
	{
		private static List<Entity> activeEntitys = new List<CGLEntity> ();

		private FloatBuffer vertexBuffer;
		private ShortBuffer indexBuffer;
		private FloatBuffer textureBuffer;

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


				// texture buffer
				textureBuffer [i * 8 + 0] = boundedPoints [i].TextureRectangle.Top;
				textureBuffer [i * 8 + 1] = boundedPoints [i].TextureRectangle.Left;
				textureBuffer [i * 8 + 2] = boundedPoints [i].TextureRectangle.Bottom;
				textureBuffer [i * 8 + 3] = boundedPoints [i].TextureRectangle.Left;
				textureBuffer [i * 8 + 4] = boundedPoints [i].TextureRectangle.Bottom;
				textureBuffer [i * 8 + 5] = boundedPoints [i].TextureRectangle.Right;
				textureBuffer [i * 8 + 6] = boundedPoints [i].TextureRectangle.Top;
				textureBuffer [i * 8 + 7] = boundedPoints [i].TextureRectangle.Right;

				// index buffer
				indexBufferArray [i * 6 + 0] = i * 3 + 0;
				indexBufferArray [i * 6 + 1] = i * 3 + 1;
				indexBufferArray [i * 6 + 2] = i * 3 + 2;
				indexBufferArray [i * 6 + 3] = i * 3 + 0;
				indexBufferArray [i * 6 + 4] = i * 3 + 2;
				indexBufferArray [i * 6 + 5] = i * 3 + 3;
			}
		}

		public static void Draw ()
		{
			foreach (CGLEntity entity in activeEntitys) {
				entity.Draw (); // das wird bald verbessert du fauler hund!!!!
			}
		}

		private void Draw ()
		{
			
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