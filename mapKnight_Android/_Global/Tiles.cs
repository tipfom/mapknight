using System;

namespace mapKnight_Android
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
		GlowingGrass = 10001,
		GlowingTendill = 10002,
		GlowingRoot = 10003
	}

	public enum Overlay : ushort
	{
		None = 0,
		EmeraldOre_1 = 1,
		EmeraldOre_2 = 2,
		Sand = 11,
		SandEdge = 12,
		SandSpot = 13,
		SlimeDots = 21,
		Slime = 22,
		SlimeEdge = 23,
		SlimeSpot = 24,
		Grass = 31,
		GrassRoot = 32,
		GrassEdge = 33,
		GrassSpot = 34
	}
}