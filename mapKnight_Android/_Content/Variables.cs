using System;
using System.Collections.Generic;
using System.ComponentModel;

using Android.Graphics;
using Android.Content;

using mapKnight.Android;
using mapKnight.Android.Net;
using mapKnight.Android.CGL;
using mapKnight.Android.CGL.Entity;
using mapKnight.Values;
using mapKnight.Entity;

namespace mapKnight
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
		public static TextureCoordinateManager<ushort> TileTexCoordManager { get; private set; }

		public static TextureCoordinateManager<ushort> OverlayTexCoordManager { get; private set; }

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
		public static int CoordsPerVertex2D = 2;
		public static int CoordsPerVertex3D = 3;
		public static int VertexStride2D = sizeof(float) * CoordsPerVertex2D;
		public static int VertexStride3D = sizeof(float) * CoordsPerVertex3D;

		// current touch manager
		public static ButtonManager TouchManager{ get; private set; }

		//character
		public static Character Character{ get; private set; }

		//		public static List<CharacterInfo> LoadedCharacterInfos{ get; private set; }

		//data
		public static SaveManager Data{ get; private set; }

		public static TerminalManager Terminal{ get; private set; }

		public static Context Context{ get; private set; }

		//map
		public static CGLMap Map{ get; private set; }

		//viewing
		public static CGLCamera Camera{ get; private set; }
	}
}