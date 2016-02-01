using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

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

		public static string Zip (this string stringToZip)
		{
			byte[] buffer = Encoding.UTF8.GetBytes (stringToZip);
			MemoryStream memoryStream = new MemoryStream ();
			using (GZipStream gZipStream = new GZipStream (memoryStream, CompressionMode.Compress, true)) {
				gZipStream.Write (buffer, 0, buffer.Length);
			}

			memoryStream.Position = 0;

			byte[] compressedData = new byte[memoryStream.Length];
			memoryStream.Read (compressedData, 0, compressedData.Length);

			byte[] gZipBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy (compressedData, 0, gZipBuffer, 4, compressedData.Length);
			Buffer.BlockCopy (BitConverter.GetBytes (buffer.Length), 0, gZipBuffer, 0, 4);
			return Convert.ToBase64String (gZipBuffer);
		}

		public static string UnZip (this string stringToUnZip)
		{
			byte[] gZipBuffer = Convert.FromBase64String (stringToUnZip);
			using (var memoryStream = new MemoryStream ()) {
				int dataLength = BitConverter.ToInt32 (gZipBuffer, 0);
				memoryStream.Write (gZipBuffer, 4, gZipBuffer.Length - 4);

				byte[] buffer = new byte[dataLength];

				memoryStream.Position = 0;
				using (GZipStream gZipStream = new GZipStream (memoryStream, CompressionMode.Decompress)) {
					gZipStream.Read (buffer, 0, buffer.Length);
				}

				return Encoding.UTF8.GetString (buffer);
			}
		}

		public static T[] Cut<T> (this T[] array, int index, int length)
		{
			T[] result = new T[length];
			Array.Copy (array, index, result, 0, length);
			return result;
		}
	}
}

