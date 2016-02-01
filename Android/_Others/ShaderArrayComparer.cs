using System;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight.Android
{
	internal class ShaderArrayComparer : IEqualityComparer<Shader[]>
	{
		public bool Equals (Shader[] x, Shader[] y)
		{
			if (x == null || y == null) {
				return x == y;
			}
			return x.SequenceEqual (y);
		}

		public int GetHashCode (Shader[] obj)
		{
			return obj.ToString ().GetHashCode ();
		}
	}
}

