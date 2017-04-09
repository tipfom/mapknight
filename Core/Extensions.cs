using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace mapKnight.Core {

    public static class Extensions {
        private static readonly CompressionLevel defaultCompression;
        private static readonly Encoding defaultEncoding;
        private static readonly HashAlgorithm hashProvider;

        static Extensions ( ) {
            defaultCompression = CompressionLevel.Optimal;
            defaultEncoding = Encoding.UTF8;
            hashProvider = SHA1.Create( );
        }

        #region Byte

        public static byte[ ] ComputeHash (this byte[ ] data) {
            return hashProvider.ComputeHash(data);
        }

        public static byte[ ] ComputeHash (this string data) {
            return data.Encode( ).ComputeHash( );
        }

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

        #endregion Byte

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

        #endregion Stream

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

        #endregion String

        #region Dictionary

        public static Dictionary<K, V> DeepClone<K, V> (this Dictionary<K, V> original) where V : ICloneable {
            Dictionary<K, V> result = new Dictionary<K, V>( );
            foreach (KeyValuePair<K, V> kvpair in original) {
                result.Add(kvpair.Key, (V)kvpair.Value.Clone( ));
            }
            return result;
        }

        #endregion Dictionary

        #region Array

        public static T[ ] SubArray<T> (this T[ ] array, int index, int length) {
            if (index + length >= array.Length)
                throw new IndexOutOfRangeException( );

            T[ ] subArray = new T[length];
            Array.Copy(array, index, subArray, 0, length);
            return subArray;
        }

        #endregion Array
        
        #region Queue

        public static void Enqueue<T> (this Queue<T> queue, IEnumerable<T> collection) {
            foreach (T element in collection)
                queue.Enqueue(element);
        }

        #endregion Queue

        #region Float

        public static bool IsInteger (this float value) {
            return value == (int)value;
        }

        #endregion Float

        #region BinaryWriter
        public static void Write(this BinaryWriter writer, Vector2 vector) {
            writer.Write(vector.X);
            writer.Write(vector.Y);
        }

        public static void WriteArray(this BinaryWriter writer, bool[] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, byte[] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, sbyte[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, short[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, ushort[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, int[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, uint[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, long[] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, ulong[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, float[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, double[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, decimal[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, Vector2[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }

        public static void WriteArray(this BinaryWriter writer, string[ ] array) {
            writer.Write((short)array.Length);
            for (int i = 0; i < array.Length; i++)
                writer.Write(array[i]);
        }
        #endregion

        #region BinaryReader
        public static Vector2 ReadVector2(this BinaryReader reader) {
            return new Vector2(reader.ReadSingle( ), reader.ReadSingle( ));
        }

        public static bool[ ] ReadBooleanArray(this BinaryReader reader) {
            bool[ ] result = new bool[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadBoolean( );
            return result;
        }

        public static byte[] ReadByteArray(this BinaryReader reader) {
            byte[ ] result = new byte[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadByte( );
            return result;
        }

        public static sbyte[ ] ReadSByteArray(this BinaryReader reader) {
            sbyte[ ] result = new sbyte[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadSByte( );
            return result;
        }

        public static short[] ReadInt16Array(this BinaryReader reader) {
            short[ ] result = new short[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadInt16( );
            return result;
        }

        public static ushort[ ] ReadUInt16Array(this BinaryReader reader) {
            ushort[ ] result = new ushort[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadUInt16( );
            return result;
        }

        public static int[ ] ReadInt32Array(this BinaryReader reader) {
            int[ ] result = new int[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadInt32( );
            return result;
        }

        public static uint[ ] ReadUInt32Array(this BinaryReader reader) {
            uint[ ] result = new uint[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadUInt32( );
            return result;
        }

        public static long[ ] ReadInt64Array(this BinaryReader reader) {
            long[ ] result = new long[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadInt64( );
            return result;
        }

        public static ulong[ ] ReadUInt64Array(this BinaryReader reader) {
            ulong[ ] result = new ulong[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadUInt64( );
            return result;
        }

        public static float[ ] ReadSingleArray(this BinaryReader reader) {
            float[ ] result = new float[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadSingle( );
            return result;
        }

        public static double[ ] ReadDoubleArray(this BinaryReader reader) {
            double[ ] result = new double[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadDouble( );
            return result;
        }

        public static decimal[ ] ReadDecimalArray(this BinaryReader reader) {
            decimal[ ] result = new decimal[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadDecimal( );
            return result;
        }

        public static Vector2[ ] ReadVector2Array(this BinaryReader reader) {
            Vector2[ ] result = new Vector2[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadVector2( );
            return result;
        }

        public static string[ ] ReadStringArray(this BinaryReader reader) {
            string[ ] result = new string[reader.ReadInt16( )];
            for (int i = 0; i < result.Length; i++)
                result[i] = reader.ReadString( );
            return result;
        }
        #endregion
    }
}