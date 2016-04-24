using System;
using System.Collections.Generic;

using mapKnight.Basic;

namespace mapKnight.Android
{
	public class Set
	{
		public readonly string Name;
		public readonly string Description;

		public List<Part> Parts;

		public Set (XMLElemental setConfig)
		{
			Name = setConfig.Attributes ["name"];
			Parts = new List<Part> ();

			foreach (XMLElemental child in setConfig.GetAll()) {
				switch (child.Name) {
				case "description":
					Description = child.Value;
					break;
				case "part":
					Parts.Add (new Part (child));
					break;
				}
			}
		}

		public class Part
		{
			public readonly string Name;
			public readonly Slot Slot;
			public readonly Dictionary<Attribute,int> Attributes;

			public Part (XMLElemental partConfig)
			{
				Name = partConfig.Attributes ["name"];
				Slot = (Slot)Enum.Parse (typeof(Slot), partConfig.Attributes ["slot"], true);
				Attributes = new Dictionary<Attribute, int> ();

				foreach (XMLElemental attribute in partConfig.GetAll()) {
					Attributes.Add ((Attribute)Enum.Parse (typeof(Attribute), attribute.Attributes ["name"], true), Convert.ToInt32 (attribute.Value));
				}
			}
		}
	}
}