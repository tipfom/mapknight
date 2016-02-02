using System;

namespace mapKnight.Android
{
	public enum Tile : ushort
	{
		Error = ushort.MaxValue,
		Air = 0,
		Dirt = 1,
		CompressedDirt = 2,
		Sand = 11,
		RedSand = 12,
		Sandstone = 13,
		Ruin = 21,
		DestroyedRuin = 22,
	}
}