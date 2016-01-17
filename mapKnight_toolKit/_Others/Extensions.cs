using System;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight
{
	public static class Extensions
	{
		public static List<T> Clone<T> (this List<T> listToClone) where T : ICloneable
		{
			return listToClone.Select (item => (T)item.Clone ()).ToList ();
		}


		public static Dictionary<K,V> Clone<K,V> (this Dictionary<K,V> dictionaryToClone) where V : ICloneable
		{
			Dictionary<K,V> result = new Dictionary<K, V> ();
			foreach (KeyValuePair<K,V> keyvaluepair in dictionaryToClone) {
				result.Add ((K)keyvaluepair.Key, (V)keyvaluepair.Value.Clone ());
			}
			return result;
		}
	}
}

