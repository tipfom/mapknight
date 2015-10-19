using System;
using System.IO;

using Android.Graphics;
using Android.Content;
using Android.Opengl;
using GL = Android.Opengl.GLES20;

using mapKnight_Android.Utils;

namespace mapKnight_Android
{
	public static partial class GlobalContent
	{
		public delegate void HandleInitCompleted(Context GameContext);
		public static event HandleInitCompleted OnInitCompleted;

		public static void Initialize(XMLElemental configfile, Context GameContext)
		{
			TileSize = Convert.ToInt32 (configfile ["image"].Attributes ["tilesize"]);

			LoadImage (GameContext.Assets.Open (configfile ["image"].Attributes ["source"]));

			TextureVertexWidth = TileSize / (float)ImageWidth;
			TextureVertexHeight = TileSize / (float)ImageHeight;

			TileTexCoordManager = LoadTileManager (configfile ["tiles"].GetAll ());
			OverlayTexCoordManager = LoadOverlayManager (configfile ["overlay"].GetAll ());

			if (OnInitCompleted != null)
				OnInitCompleted (GameContext);
		}

		private static void LoadImage(Stream ImageStream)
		{
			int[] loadedtexture = new int[1];
			GL.GlGenTextures (1, loadedtexture, 0);

			BitmapFactory.Options bfoptions = new BitmapFactory.Options ();
			bfoptions.InScaled = false;
			Bitmap bitmap = BitmapFactory.DecodeStream (ImageStream, null, bfoptions);

			GL.GlBindTexture (GL.GlTexture2d, loadedtexture [0]);

			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
			GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
			GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
			GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

			GLUtils.TexImage2D (GL.GlTexture2d, 0, bitmap, 0);

			ImageHeight = bitmap.Height;
			ImageWidth = bitmap.Width;

			bitmap.Recycle ();
			GL.GlBindTexture (GL.GlTexture2d, 0);

			// Error Check
			int error = GL.GlGetError ();
			if (error != 0) {
				Log.All (typeof(GlobalContent), "error while loading mainimage (errorcode => " + error.ToString () + ")", MessageType.Debug);
				throw new FileLoadException ("error while loading mainimage (errorcode => " + error.ToString () + ")");
			}
			if (loadedtexture[0] == 0) {
				Log.All (typeof(GlobalContent), "loaded mainimage is zero", MessageType.Debug);
				throw new FileLoadException ("loaded mainimage is zero");
			}

			// set MainTexture to the loaded texture
			MainTexture = loadedtexture[0];
		}
	}
}