using System;

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

		// current map
		public static CGL.CGLMap Map { get; private set; }

		// TextureCoordinateManager for tiles and overlays
		public static TextureCoordinateManager<short> TileTexCoordManager { get; private set; }
		public static TextureCoordinateManager<short> OverlayTexCoordManager { get; private set; }
	}
}