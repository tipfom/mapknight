using System;

using mapKnight.Basic;
using Android.Opengl;

namespace mapKnight.Android.CGL
{
	public class CGLCamera
	{
		public Point CurrentMapTile;

		public float[] DefaultProjectionMatrix{ get; private set; }

		public float[] DefaultViewMatrix{ get; private set; }

		public float[] DefaultMVPMatrix { get; private set; }


		public float[] MapViewMatrix{ get; private set; }

		public float[] MapMVPMatrix{ get; private set; }


		public float[] CharacterViewMatrix{ get; private set; }

		public float[] CharacterMVPMatrix{ get; private set; }


		private float characterOffset;

		public CGLCamera (float characteroffset)
		{
			CurrentMapTile = new Point ();
			characterOffset = characteroffset;

			DefaultProjectionMatrix = new float[16];
			DefaultViewMatrix = new float[16];
			DefaultMVPMatrix = new float[16];
			UpdateDefaultMatrix ();

			MapViewMatrix = new float[16];
			MapMVPMatrix = new float[16];

			CharacterViewMatrix = new float[16];
			CharacterMVPMatrix = new float[16];

			Content.OnUpdate += () => {
				UpdateDefaultMatrix ();	
			};
		}

		private void UpdateDefaultMatrix ()
		{
			Matrix.FrustumM (DefaultProjectionMatrix, 0, -Content.ScreenRatio, Content.ScreenRatio, -1, 1, 3, 7);
			Matrix.SetLookAtM (DefaultViewMatrix, 0, 0, 0, -3, 0f, 0f, 0f, 0f, 1f, 0f);
			Matrix.MultiplyMM (DefaultMVPMatrix, 0, DefaultProjectionMatrix, 0, DefaultViewMatrix, 0);
		}

		public void Update ()
		{
			fPoint CharacterCenter = new fPoint ((float)(Content.Character.Position.X + Content.Character.Bounds.Width / 2) / PhysX.PhysXMap.TILE_BOX_SIZE, 
				                         (float)(Content.Character.Position.Y + Content.Character.Bounds.Height / 2) / PhysX.PhysXMap.TILE_BOX_SIZE);

			// berechnet die werte für die map abhängig von der Character position
			CurrentMapTile.Y = Math.Min (Math.Max ((int)(CharacterCenter.Y) - (int)(Content.Map.DrawSize.Height / 2), 0), Content.Map.Height - Content.Map.DrawSize.Height);
			CurrentMapTile.X = Math.Min (Math.Max ((int)(CharacterCenter.X) - (int)(Content.Map.DrawSize.Width / 2), 0), Content.Map.Width - Content.Map.DrawSize.Width);

			Matrix.SetLookAtM (CharacterViewMatrix, 0, 0, 0, -3f, 0, 0f, 0, 0f, 1f, 0f);
			Matrix.SetLookAtM (MapViewMatrix, 0, 0, 0, -3f, 0, 0, 0, 0, 1f, 0);

			float mapOffsetY = 0f;
			float mapOffsetX = 0f;
			float charOffsetY = 0f;
			float charOffsetX = 0f;

			if (CharacterCenter.X > Content.Map.DrawSize.Width / 2 && CharacterCenter.X < (Content.Map.Width - Content.Map.DrawSize.Width / 2)) {
				mapOffsetX = CharacterCenter.X;
				mapOffsetX -= (int)mapOffsetX;
				mapOffsetX = 2f * mapOffsetX * Content.ScreenRatio / (Content.Map.DrawSize.Width);
			} else if (CharacterCenter.X > Content.Map.DrawSize.Width / 2) {
				// on the right side
				charOffsetX = -Content.ScreenRatio + 2f * ((Content.Map.Width - CharacterCenter.X) / Content.Map.DrawSize.Width) * Content.ScreenRatio;
			} else {
				// on the left side
				charOffsetX = 2f * ((Content.Map.DrawSize.Width / 2 - CharacterCenter.X) / Content.Map.DrawSize.Width) * Content.ScreenRatio;
			}

			if (CharacterCenter.Y > Content.Map.DrawSize.Height / 2 && CharacterCenter.Y < (Content.Map.Height - Content.Map.DrawSize.Height / 2)) {
				mapOffsetY = CharacterCenter.Y;
				mapOffsetY -= (int)mapOffsetY;
				mapOffsetY = -2f * mapOffsetY * 1f / (Content.Map.DrawSize.Height);

				charOffsetY = characterOffset;
			} else if (CharacterCenter.Y > Content.Map.DrawSize.Height / 2) {
				
			} else {
				charOffsetY = -2f * ((Content.Map.DrawSize.Height / 2 - CharacterCenter.Y) / Content.Map.DrawSize.Height);
			}

			Matrix.TranslateM (CharacterViewMatrix, 0, charOffsetX, charOffsetY, 0);
			Matrix.TranslateM (MapViewMatrix, 0, mapOffsetX, mapOffsetY, 0);

			Matrix.MultiplyMM (CharacterMVPMatrix, 0, DefaultProjectionMatrix, 0, CharacterViewMatrix, 0);
			Matrix.MultiplyMM (MapMVPMatrix, 0, DefaultProjectionMatrix, 0, MapViewMatrix, 0);

			Content.Map.updateTextureBuffer (CGLMap.UpdateType.Complete);
		}
	}
}

