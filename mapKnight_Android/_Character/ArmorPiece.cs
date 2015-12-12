using System;
using System.Collections.Generic;

using mapKnight.Utils;
using mapKnight.Values;

namespace mapKnight.Android
{
	public class ArmorPiece
	{
		public Dictionary<Attribute,int> Attributes{ get; private set; }

		public string Name{ get; private set; }

		public Rectangle TextureRectangle{ get; private set; }

		public ArmorPiece (string name, List<XMLElemental> config)
		{
			Name = name;
			Attributes = new Dictionary<Attribute, int> ();

			foreach (XMLElemental element in config) {
				if (element.Name == "texture") {
					TextureRectangle = new Rectangle (Convert.ToInt32 (element.Attributes ["x"]), Convert.ToInt32 (element.Attributes ["y"]), Convert.ToInt32 (element.Attributes ["width"]), Convert.ToInt32 (element.Attributes ["height"]));
				} else if (element.Name == "attribute") {
					Attributes.Add ((Attribute)Enum.Parse (typeof(Attribute), element.Attributes ["name"], true), Convert.ToInt32 (element.Value));
				}
			}
		}
	}
}
