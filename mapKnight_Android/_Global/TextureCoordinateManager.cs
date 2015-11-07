using System;
using System.Collections.Generic;

namespace mapKnight_Android
{
	public static partial class GlobalContent
	{
		public class TextureCoordinateManager<T>
		{
			Dictionary<T,float[]> Container;

			public TextureCoordinateManager ()
			{
				Container = new Dictionary<T, float[]> ();
			}

			public float[] this [T ID] {
				get {
					return Container [ID];
				}
				set {
					if (Container.ContainsKey (ID)) {
						Container [ID] = value;
					} else {
						Container.Add (ID, value);
					}
				}
			}

			public float[] Get (T ID)
			{
				return this [ID];	
			}

			public void Add (T ID, float[] value)
			{
				if (!Container.ContainsKey (ID))
					Container.Add (ID, value);
			}

			public void Set (T ID, float[] value)
			{
				if (Container.ContainsKey (ID))
					Container [ID] = value;
			}
		}
	}
}