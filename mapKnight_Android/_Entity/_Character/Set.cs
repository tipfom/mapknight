using System;
using System.Collections.Generic;

using mapKnight.Utils;

namespace mapKnight.Android
{
	public class Set
	{
		public string Name{ get; private set; }

		public Dictionary<ArmorSlot,ArmorPiece> Armor;

		public Set (XMLElemental config)
		{
			Name = config.Attributes ["name"];

			Armor = new Dictionary<ArmorSlot, ArmorPiece> ();

			foreach (XMLElemental element in config.GetAll()) {
				if (element.Name == "part") {
					// its a armorpiece
					ArmorSlot armor = (ArmorSlot)Enum.Parse (typeof(ArmorSlot), element.Attributes ["slot"], true);
					if (Enum.IsDefined (typeof(ArmorSlot), armor)) {
						Armor.Add (armor, new ArmorPiece (element.Attributes ["name"], element.GetAll ()));
					} else {
						Log.All (this, "tried to load ArmorSlot named " + element.Attributes ["name"] + ", which is not defined", MessageType.Warn);
					}
				}	
			}
		}
	}
}