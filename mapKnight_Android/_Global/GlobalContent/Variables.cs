using System;
using System.Collections.Generic;

using Android.Graphics;

namespace mapKnight_Android
{
	public static partial class GlobalContent
	{
		public static int MainTexture { get; private set; }
	
		// texture variables
		public static int TileSize{ get; private set; }
		private static int ImageWidth{ get ; set; }
		private static int ImageHeight{ get ; set; }
		private static float TextureVertexWidth{ get; set; }
		private static float TextureVertexHeight{ get; set; }

		// TextureCoordinateManager for tiles and overlays
		public static TextureCoordinateManager<short> TileTexCoordManager { get; private set; }
		public static TextureCoordinateManager<short> OverlayTexCoordManager { get; private set; }

		// screen bounds
		public static Size ScreenSize{ get; private set; }
		public static float ScreenRatio{ get; private set; }

		// fonts
		public static Dictionary<Font, Typeface> Fonts{ get; private set; }
	}
}