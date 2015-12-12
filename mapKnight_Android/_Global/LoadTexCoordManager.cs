using System;
using System.Collections.Generic;

using mapKnight.Utils;

namespace mapKnight.Android
{
	public static partial class Content
	{
		private static TextureCoordinateManager<short> LoadManager (List<XMLElemental> Tiles)
		{
			TextureCoordinateManager<short> LoadedInstance = new TextureCoordinateManager<short> ();

			foreach (XMLElemental entry in Tiles) {
				short id = (short)Convert.ToInt32 (entry.Attributes ["id"]);
				float x = (float)Convert.ToInt32 (entry.Attributes ["x"]) / ImageWidth;
				float y = 1f - (float)(Content.ImageHeight - (short)Convert.ToInt32 (entry.Attributes ["y"])) / ImageHeight;

				float[] parsedCoordinates = new float[] { 
					x + TextureVertexWidth, y,
					x + TextureVertexWidth, y + TextureVertexHeight,
					x, y + TextureVertexHeight,
					x, y
				};

				LoadedInstance [id] = parsedCoordinates;
			}

			return LoadedInstance;
		}
	}
}