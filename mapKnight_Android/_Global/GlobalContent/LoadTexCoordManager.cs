using System;
using System.Collections.Generic;

using mapKnight_Android.Utils;

namespace mapKnight_Android
{
	public static partial class GlobalContent
	{
		private static TextureCoordinateManager<short> LoadTileManager (List<XMLElemental> Tiles)
		{
			TextureCoordinateManager<short> LoadedInstance = new TextureCoordinateManager<short> ();

			foreach(XMLElemental entry in Tiles)
			{
				short id = (short)Convert.ToInt32 (entry.Attributes ["id"]);
				float x = (float)Convert.ToInt32 (entry.Attributes ["x"]) / ImageWidth;
				float y = 1f - (float)(GlobalContent.ImageHeight - (short)Convert.ToInt32 (entry.Attributes ["y"])) / ImageHeight;

				float[] parsedCoordinates = new float[] { 
					x, y + TextureVertexHeight,
					x, y,
					x + TextureVertexWidth, y,
					x + TextureVertexWidth, y + TextureVertexHeight
				};

				LoadedInstance [id] = parsedCoordinates;
			}

			return LoadedInstance;
		}

		private static TextureCoordinateManager<short> LoadOverlayManager (List<XMLElemental> Overlays)
		{
			TextureCoordinateManager<short> LoadedInstance = new TextureCoordinateManager<short> ();

			foreach(XMLElemental entry in Overlays)
			{
				short id = (short)Convert.ToInt32 (entry.Attributes ["id"]);
				float x = (float)Convert.ToInt32 (entry.Attributes ["x"]) / ImageWidth;
				float y = (float)(GlobalContent.ImageHeight - (short)Convert.ToInt32 (entry.Attributes ["y"])) / ImageHeight;

				float[] parsedCoordinates = new float[] { 
					x, y + TextureVertexHeight,
					x, y,
					x + TextureVertexWidth, y,
					x + TextureVertexWidth, y + TextureVertexHeight
				};

				LoadedInstance [id] = parsedCoordinates;
			}

			return LoadedInstance;
		}
	}
}