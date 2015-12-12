using System.Collections.Generic;
using System.Drawing;

namespace mapKnight.ToolKit
{
	class Set
	{
		public string Name;
		public string Description;

		public Dictionary<Slot, Part> Parts = new Dictionary<Slot, Part> ();

		public Set (string name)
		{
			this.Name = name;
		}

		public Set (string name, Dictionary<Slot,Part> parts)
		{
			this.Name = name;
			Parts = parts;
		}

		public override string ToString ()
		{
			return this.Name;
		}

		public class Part
		{
			public Dictionary<Attribute, string> Attributes;
			public string Name;
			public Bitmap Bitmap;

			public Part (string name, Dictionary<Attribute, string> attributes, Bitmap bitmap)
			{
				Name = name;
				Attributes = attributes;
				Bitmap = bitmap;
			}

			public Part (string name)
			{
				Name = name;

				Attributes = new Dictionary<Attribute, string> ();
				Bitmap = Properties.Resources.icon_unselected;
			}
		}
	}
}
