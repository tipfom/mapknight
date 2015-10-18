using System;

namespace mapKnight_Android
{
	public static partial class GlobalContent
	{
		public static int MainTexture { get; protected set; }
	
		// texture variables
		public static int TileSize{ get; protected set; }
		protected static int ImageWidth{ get ; protected set; }
		protected static int ImageHeight{ get ; protected set; }
		protected static float TextureVertexWidth{ get; protected set; }
		protected static float TextureVertexHeight{ get; protected set; }

		// current map
		public static CGL.CGLMap Map { get; protected set; }

		// TextureCoordinateManager for tiles and overlays
		public static TextureCoordinateManager<short> TileTexCoordManager { get; protected set; }
		public static TextureCoordinateManager<short> OverlayTexCoordManager { get; protected set; }
	}
}