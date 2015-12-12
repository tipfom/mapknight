using System;

namespace mapKnight.Android
{
	public struct DefinitionPoint
	{
		public readonly int ID;
		public readonly int Texture;
		public readonly string Name;

		public DefinitionPoint (int id, string name, int texture) : this ()
		{
			ID = id;
			Name = name;
			Texture = texture;
		}
	}
}

