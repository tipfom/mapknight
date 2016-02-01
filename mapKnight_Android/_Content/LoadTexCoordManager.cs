using System;
using System.Collections.Generic;

using mapKnight.Utils;

namespace mapKnight
{
	public static partial class Content
	{
		private static TextureCoordinateManager<ushort> LoadManager (List<XMLElemental> Tiles)
		{
			TextureCoordinateManager<ushort> LoadedInstance = new TextureCoordinateManager<ushort> ();

			foreach (XMLElemental entry in Tiles) {
				ushort id = (ushort)Convert.ToInt32 (entry.Attributes ["id"]);
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