using System;
using System.Collections.Generic;
using System.ComponentModel;

using Android.Graphics;

using mapKnight.Android.CGL;
using mapKnight.Values;

namespace mapKnight.Android
{
	public static partial class Content
	{
		public static int MainTexture { get; private set; }

		public static CGLSprite<int> InterfaceSprite{ get; private set; }

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

		// text variables
		private static bool iAntialiasText = true;

		public static bool AntialiasText{ get { return iAntialiasText; } set { iAntialiasText = value; } }

		// version string
		public static Values.Version Version;

		// draw variables
		public static float[] ViewMatrix{ get; private set; }

		public static float[] ProjectionMatrix{ get; private set; }

		public static float[] MVPMatrix{ get; private set; }

		// current touch manager
		public static ButtonManager TouchManager{ get; private set; }

		//character
		public static Character Character{ get; private set; }

		public static List<CharacterInfo> LoadedCharacterInfos{ get; private set; }

		//data
		public static SaveManager Data{ get; private set; }
	}
}