using System;
using System.Collections.Generic;

using mapKnight.Basic;

namespace mapKnight.Android
{
	public class EntityPreset
	{
		protected string name;
		protected Dictionary<Attribute, int> defaultAttributes;
		protected Dictionary<Attribute,float> attributeIncrease;

		public EntityPreset (XMLElemental entityConfig)
		{
			name = entityConfig.Attributes ["name"];

			// default attributes
			defaultAttributes = new Dictionary<Attribute, int> ();
			foreach (XMLElemental attribute in entityConfig["level"]["default"].GetAll()) {
				defaultAttributes.Add ((Attribute)Enum.Parse (typeof(Attribute), attribute.Name, true), Convert.ToInt32 (attribute.Attributes ["value"]));
			}

			// increase of each attribute per level
			attributeIncrease = new Dictionary<Attribute, float> ();
			foreach (XMLElemental attribute in entityConfig["level"]["increase"].GetAll()) {
				attributeIncrease.Add ((Attribute)Enum.Parse (typeof(Attribute), attribute.Name, true), float.Parse (attribute.Attributes ["value"]));
			}
		}

		public virtual Entity Instantiate (uint level)
		{
			return Instantiate (level, new fPoint (0, 0));			
		}

		public virtual Entity Instantiate (uint level, fPoint position)
		{
			return new Entity (defaultAttributes [Attribute.Health] + (int)((level - 1) * attributeIncrease [Attribute.Health]), position, name);
		}
	}
}

