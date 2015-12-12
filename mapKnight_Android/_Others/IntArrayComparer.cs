using System;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight.Android
{
	internal class IntArrayComparer : IEqualityComparer<int[]>
	{
		public bool Equals (int[] x, int[] y)
		{
			if (x == null || y == null) {
				return x == y;
			}
			return x.SequenceEqual (y);
		}

		public int GetHashCode (int[] obj)
		{
			return obj.ToString ().GetHashCode ();
		}
	}
}

