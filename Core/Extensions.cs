using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO.Compression;
using System.Security.Cryptography;

namespace mapKnight.Core {
    public static class Extensions {
        static readonly CompressionLevel defaultCompression;
        static readonly Encoding defaultEncoding;
        static readonly HashAlgorithm hashProvider;

        static Extensions ( ) {
            defaultCompression = CompressionLevel.Optimal;
            defaultEncoding = Encoding.UTF8;
            hashProvider = SHA1.Create( );
        }

        #region Byte
        public static string Decode (this byte[ ] utf8data) {
            return defaultEncoding.GetString(utf8data, 0, utf8data.Length);
        }

        public static byte[ ] Encode (this string data) {
            return defaultEncoding.GetBytes(data);
        }

        public static byte[ ] Encode (this int data) {
            return BitConverter.GetBytes(data);
        }

        public static byte[ ] Encode (this short data) {
            return BitConverter.GetBytes(data);
        }

        public static byte[ ] Encode (this float data) {
            return BitConverter.GetBytes(data);
        }

        public static byte[ ] ComputeHash (this byte[ ] data) {
            return hashProvider.ComputeHash(data);
        }

        public static byte[ ] ComputeHash (this string data) {
            return data.Encode( ).ComputeHash( );
        }
        #endregion

        #region Stream
        public static Stream Compress (this Stream stream) {
            return new GZipStream(stream, defaultCompression);
        }

        public static Stream Compress (this Stream stream, CompressionLevel level) {
            return new GZipStream(stream, level);
        }


        public static Stream Decompress (this Stream stream) {
            return new GZipStream(stream, CompressionMode.Decompress);
        }
        #endregion

        #region String
        public static string Compress (this string data) {
            return data.Compress(defaultCompression);
        }

        public static string Compress (this string data, CompressionLevel level) {
            using (MemoryStream memstream = new MemoryStream( )) {
                // compress data
                using (GZipStream zipstream = new GZipStream(memstream, level, true))
                using (BinaryWriter writer = new BinaryWriter(zipstream))
                    writer.Write(data.Encode( ));

                // reset readposition
                memstream.Seek(0, SeekOrigin.Begin);
                // read result
                using (BinaryReader reader = new BinaryReader(memstream))
                    return Convert.ToBase64String(reader.ReadBytes((int)memstream.Length));
            }
        }
        
        public static string Decompress (this string data) {
            using (MemoryStream memstream = new MemoryStream(Convert.FromBase64String(data)))
            using (GZipStream zipstream = new GZipStream(memstream, CompressionMode.Decompress))
            using (StreamReader reader = new StreamReader(zipstream))
                return reader.ReadToEnd( );
        }
        #endregion

        #region Dictionary
        public static Dictionary<K, V> DeepClone<K, V> (this Dictionary<K, V> original) {
            return new Dictionary<K, V>(original);
        }
        #endregion

        #region Array
        public static T[ ] SubArray<T> (this T[ ] array, int index, int length) {
            if (index + length >= array.Length)
                throw new IndexOutOfRangeException( );

            T[ ] subArray = new T[length];
            Array.Copy(array, index, subArray, 0, length);
            return subArray;
        }
        #endregion

        #region Random
        public static double NextDouble (this Random random, double maxValue) {
            return random.NextDouble( ) * maxValue;
        }

        public static double NextDouble (this Random random, double minValue, double maxValue) {
            return random.NextDouble(maxValue - minValue) + minValue;
        }

        public static float NextFloat (this Random random) {
            return (float)random.NextDouble(float.MinValue, float.MaxValue);
        }

        public static float NextFloat (this Random random, float maxValue) {
            return (float)random.NextDouble(maxValue);
        }

        public static float NextFloat (this Random random, float minValue, float maxValue) {
            return (float)random.NextDouble(minValue, maxValue);
        }
        #endregion

        #region Queue
        public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> collection) {
            foreach (T element in collection)
                queue.Enqueue(element);
        }
        #endregion
    }
}
