using System;

namespace mapKnight_Android
{
	public static class AO
	{
		public static T[] Cut<T>(T[] array, int index, int length)
		{
			T[] result = new T[length];
			Array.Copy (array, index, result, 0, length);
			return result;
		}
	}
}

