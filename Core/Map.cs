using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using mapKnight.Core.Exceptions;
using mapKnight.Core.World;
using Newtonsoft.Json;
using mapKnight.Core.World.Serialization;

namespace mapKnight.Core {
    public class Map {
        public const int TIME_BETWEEN_TICKS = 1000 / 2;
        public const int TILE_PXL_SIZE = 18;
        public static byte[ ] HEADER { get; } = { 133, 007, 042, 077, 064, 080 };
        public static short VERSION { get; } = 4;

        #region default deserialization values
        private static Vector2 DEFAULT_GRAVITY { get { return new Vector2(0, -60); } }
        #endregion

        // x,y,layer
        public int[ , , ] Data;

        public Size Size { get; private set; }
        public int Height { get { return Size.Height; } }
        public int Width { get { return Size.Width; } }

        public Vector2 SpawnPoint;
        public Tile[ ] Tiles;
        public string Texture { get; set; }
        public string Creator { get; set; }
        public string Name { get; set; }
        public Vector2 Gravity { get; set; }

        public List<Entity> Entities { get; set; }
        public event Action<Entity> EntityAdded;
        private int nextTickTime;
        private Queue<Entity> removedEntities = new Queue<Entity>( );

        public Map(Size size, string creator, string name) {
            Size = size;
            Data = new int[size.Width, size.Height, 3];
            Creator = creator;
            Name = name;
            SpawnPoint = new Vector2( );
            Entities = new List<Entity>( );
        }

        public Map(Size size) : this(size, null, null) {

        }

        public Map(Stream input, IEntitySerializer serializer) {
            using (BinaryReader reader = new BinaryReader(input, Encoding.UTF8, false)) {
                // check for header
                if (!reader.ReadBytes(HEADER.Length).SequenceEqual(HEADER))
                    throw new MissingMapIdentifierException( );
                // load specific mapversion
                short version = reader.ReadInt16( );
                switch (version) {
                    case 1:
                        Deserialize00001(reader);
                        break;
                    case 2:
                        Deserialize00002(reader);
                        break;
                    case 3:
                        Deserialize00003(reader);
                        break;
                    case 4:
                        Deserialize00004(reader, serializer);
                        break;
                    default:
                        throw new FileLoadException($"map version {version} is not known.");
                }
            }
        }

        public void Serialize(Stream stream, IEntitySerializer serializer) {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true)) {
                bool is32bit = (Size.Area >= Math.Pow(2, 16));

                // write header
                writer.Write(HEADER);
                writer.Write(VERSION);

                // write data 
                writer.Write(is32bit);
                writer.Write((short)Width);
                writer.Write((short)Height);
                writer.Write((short)SpawnPoint.X);
                writer.Write((short)SpawnPoint.Y);

                /////////////////////////////////////////////////////////////////////////////////////////
                // compress mapdata
                int[ ] currenttile = { -1, -1, -1 }; // current tile of each layer
                Dictionary<int, List<int>[ ]> startpoints = new Dictionary<int, List<int>[ ]>( );

                for (int y = 0; y < Height; y++) {
                    for (uint x = 0; x < Width; x++) {
                        for (int layer = 0; layer < 3; layer++) {
                            // go through each layer
                            if (currenttile[layer] != Data[x, y, layer]) {
                                // if tile changed
                                if (!startpoints.ContainsKey(Data[x, y, layer])) {
                                    startpoints.Add(Data[x, y, layer], new List<int>[3]);

                                    startpoints[Data[x, y, layer]][0] = new List<int>( );
                                    startpoints[Data[x, y, layer]][1] = new List<int>( );
                                    startpoints[Data[x, y, layer]][2] = new List<int>( );
                                }
                                // remember startingpoint
                                // invert the y axis
                                startpoints[Data[x, y, layer]][layer].Add((int)(y * Width + x));
                                currenttile[layer] = Data[x, y, layer];
                            }
                        }
                    }
                }

                Action<int> WriteInt;
                if (is32bit)
                    WriteInt = (value) => { writer.Write((int)value); };
                else
                    WriteInt = (value) => { writer.Write((short)value); };

                /////////////////////////////////////////////////////////////////////////////////////////
                // write mapdata
                foreach (var tileentry in startpoints) {
                    WriteInt(tileentry.Key + 1); // tileid
                    for (int layer = 0; layer < 3; layer++) {
                        tileentry.Value[layer].Sort( );
                        for (int i = 0; i < tileentry.Value[layer].Count; i++) {
                            WriteInt(tileentry.Value[layer][i] + 1);
                        }
                        WriteInt(0);
                    }
                }
                WriteInt(0);

                //////////////////////////////////////////////////////////////////////////////////////////
                // write tiles
                byte[ ] tilesraw = JsonConvert.SerializeObject(Tiles).Compress( ).Encode( );
                writer.Write((short)tilesraw.Length);
                writer.Write(tilesraw);

                // write texturename
                writer.Write(Texture);

                // write info
                writer.Write(Creator);
                writer.Write(Name);
                writer.Write(Gravity.X);
                writer.Write(Gravity.Y);

                // estimated entity count
                writer.Write((short)Entities.Count);
                // write entities
                for (int i = 0; i < Entities.Count; i++) {
                    // id
                    writer.Write((int)serializer.GetID(Entities[i]));
                    // pos
                    writer.Write(Entities[i].Transform.Center.X);
                    writer.Write(Entities[i].Transform.Center.Y);
                    // data
                    foreach (Tuple<DataID, DataType, object> data in serializer.GetData(Entities[i])) {
                        // data id
                        writer.Write((int)data.Item1);
                        // data type
                        writer.Write((byte)data.Item2);
                        // the data itself
                        switch (data.Item2) {
                            case DataType.Boolean:
                                writer.Write((bool)data.Item3);
                                break;
                            case DataType.BooleanArray:
                                writer.WriteArray((bool[ ])data.Item3);
                                break;
                            case DataType.Byte:
                                writer.Write((byte)data.Item3);
                                break;
                            case DataType.ByteArray:
                                writer.WriteArray((byte[ ])data.Item3);
                                break;
                            case DataType.SByte:
                                writer.Write((sbyte)data.Item3);
                                break;
                            case DataType.SByteArray:
                                writer.WriteArray((sbyte[ ])data.Item3);
                                break;
                            case DataType.Short:
                                writer.Write((short)data.Item3);
                                break;
                            case DataType.ShortArray:
                                writer.WriteArray((short[ ])data.Item3);
                                break;
                            case DataType.UShort:
                                writer.Write((ushort)data.Item3);
                                break;
                            case DataType.UShortArray:
                                writer.WriteArray((ushort[ ])data.Item3);
                                break;
                            case DataType.Int:
                                writer.Write((int)data.Item3);
                                break;
                            case DataType.IntArray:
                                writer.WriteArray((int[ ])data.Item3);
                                break;
                            case DataType.UInt:
                                writer.Write((uint)data.Item3);
                                break;
                            case DataType.UIntArray:
                                writer.WriteArray((uint[ ])data.Item3);
                                break;
                            case DataType.Long:
                                writer.Write((long)data.Item3);
                                break;
                            case DataType.LongArray:
                                writer.WriteArray((long[ ])data.Item3);
                                break;
                            case DataType.ULong:
                                writer.Write((ulong)data.Item3);
                                break;
                            case DataType.ULongArray:
                                writer.WriteArray((ulong[ ])data.Item3);
                                break;
                            case DataType.Float:
                                writer.Write((float)data.Item3);
                                break;
                            case DataType.FloatArray:
                                writer.WriteArray((float[ ])data.Item3);
                                break;
                            case DataType.Double:
                                writer.Write((double)data.Item3);
                                break;
                            case DataType.DoubleArray:
                                writer.WriteArray((double[ ])data.Item3);
                                break;
                            case DataType.Decimal:
                                writer.Write((decimal)data.Item3);
                                break;
                            case DataType.DecimalArray:
                                writer.WriteArray((decimal[ ])data.Item3);
                                break;
                            case DataType.Vector2:
                                writer.Write((Vector2)data.Item3);
                                break;
                            case DataType.Vector2Array:
                                writer.WriteArray((Vector2[ ])data.Item3);
                                break;
                            case DataType.String:
                                writer.Write((string)data.Item3);
                                break;
                            case DataType.StringArray:
                                writer.WriteArray((string[ ])data.Item3);
                                break;
                        }
                    }
                    // end
                    writer.Write((int)DataID.End);
                }
                writer.Write((int)EntityID.End);
            }
        }

        public Tile GetTile(int x, int y, int layer) {
            return Tiles[Data[x, y, layer]];
        }

        public Tile GetTileBackground(int x, int y) {
            return Tiles[Data[x, y, 0]];
        }

        public Tile GetTile(int x, int y) {
            return Tiles[Data[x, y, 1]];
        }

        public Tile GetTileForeground(int x, int y) {
            return Tiles[Data[x, y, 2]];
        }

        public void Add(Entity entity) {
            Entities.Add(entity);
            EntityAdded?.Invoke(entity);
        }

        public void Destroy(Entity entity) {
            removedEntities.Enqueue(entity);
        }

#if __ANDROID__
        public void Update(DeltaTime dt) {
            // cleanup
            while (removedEntities.Count > 0) {
                Entities.Remove(removedEntities.Dequeue( ));
            }

            // ticks
            if (Environment.TickCount > nextTickTime) {
                for (int i = 0; i < Entities.Count; i++)
                    Entities[i].Tick( );
                nextTickTime = Environment.TickCount + TIME_BETWEEN_TICKS;
            }

            // updates
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Update(dt);

            // collisions
            int outerLoopsBounds = Entities.Count - 1;
            for (int i = 0; i < outerLoopsBounds; i++) {
                for (int l = i + 1; l < Entities.Count; l++) {
                    if (Entities[i].Transform.Intersects(Entities[l].Transform)) {
                        Entities[i].Collision(Entities[l]);
                        Entities[l].Collision(Entities[i]);
                    }
                }
            }
        }

        public void Draw( ) {
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Draw( );
        }
#endif

        public override string ToString( ) {
            return $"{Name} ({Size.Width}x{Size.Height})";
        }

        #region deserialization
        private void Deserialize00001(BinaryReader reader) {
            bool is32bit = reader.ReadBoolean( );

            Size = new Size(reader.ReadInt16( ), reader.ReadInt16( ));
            SpawnPoint = new Vector2(reader.ReadInt16( ), reader.ReadInt16( ));

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            // load data
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            Data = new int[Width, Height, 3];

            int currenttile;
            int currentlayer = 0;
            while ((currenttile = is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) != 0) {
                while (currentlayer < 3) {
                    int data = (is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) - 1;
                    if (data == -1) {
                        currentlayer++;
                    } else {
                        int y = (int)(data / Width);
                        int x = data - (int)(y * Width);
                        Data[x, y, currentlayer] = currenttile;
                    }
                }
                currentlayer = 0;
            }

            // uncompress mapdata
            int[ ] currentTile = new int[ ] { -1, -1, -1 };
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    for (int layer = 0; layer < 3; layer++) {
                        if (Data[x, y, layer] != currentTile[layer] && Data[x, y, layer] != 0) {
                            currentTile[layer] = Data[x, y, layer];
                        } else {
                            Data[x, y, layer] = currentTile[layer];
                        }
                        Data[x, y, layer] -= 1;
                    }
                }
            }

            Tiles = JsonConvert.DeserializeObject<Tile[ ]>(reader.ReadBytes(reader.ReadInt16( )).Decode( ).Decompress( ));
            Texture = reader.ReadBytes(reader.ReadInt16( )).Decode( );

            Creator = reader.ReadBytes(reader.ReadInt16( )).Decode( );
            Name = reader.ReadBytes(reader.ReadInt16( )).Decode( );
            Gravity = DEFAULT_GRAVITY;
            Entities = new List<Entity>( );
        }

        private void Deserialize00002(BinaryReader reader) {
            bool is32bit = reader.ReadBoolean( );

            Size = new Size(reader.ReadInt16( ), reader.ReadInt16( ));
            SpawnPoint = new Vector2(reader.ReadInt16( ), reader.ReadInt16( ));

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            // load data
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            Data = new int[Width, Height, 3];

            int currenttile;
            int currentlayer = 0;
            while ((currenttile = is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) != 0) {
                while (currentlayer < 3) {
                    int data = (is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) - 1;
                    if (data == -1) {
                        currentlayer++;
                    } else {
                        int y = (int)(data / Width);
                        int x = data - (int)(y * Width);
                        Data[x, y, currentlayer] = currenttile;
                    }
                }
                currentlayer = 0;
            }

            // uncompress mapdata
            int[ ] currentTile = new int[ ] { -1, -1, -1 };
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    for (int layer = 0; layer < 3; layer++) {
                        if (Data[x, y, layer] != currentTile[layer] && Data[x, y, layer] != 0) {
                            currentTile[layer] = Data[x, y, layer];
                        } else {
                            Data[x, y, layer] = currentTile[layer];
                        }
                        Data[x, y, layer] -= 1;
                    }
                }
            }

            Tiles = JsonConvert.DeserializeObject<Tile[ ]>(reader.ReadBytes(reader.ReadInt16( )).Decode( ).Decompress( ));
            Texture = reader.ReadBytes(reader.ReadInt16( )).Decode( );

            Creator = reader.ReadBytes(reader.ReadInt16( )).Decode( );
            Name = reader.ReadBytes(reader.ReadInt16( )).Decode( );
            Gravity = new Vector2(reader.ReadSingle( ), reader.ReadSingle( ));
            Entities = new List<Entity>( );
        }

        private void Deserialize00003(BinaryReader reader) {
            bool is32bit = reader.ReadBoolean( );

            Size = new Size(reader.ReadInt16( ), reader.ReadInt16( ));
            SpawnPoint = new Vector2(reader.ReadInt16( ), reader.ReadInt16( ));

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            // load data
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            Data = new int[Width, Height, 3];

            int currenttile;
            int currentlayer = 0;
            while ((currenttile = is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) != 0) {
                while (currentlayer < 3) {
                    int data = (is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) - 1;
                    if (data == -1) {
                        currentlayer++;
                    } else {
                        int y = (int)(data / Width);
                        int x = data - (int)(y * Width);
                        Data[x, y, currentlayer] = currenttile;
                    }
                }
                currentlayer = 0;
            }

            // uncompress mapdata
            int[ ] currentTile = new int[ ] { -1, -1, -1 };
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    for (int layer = 0; layer < 3; layer++) {
                        if (Data[x, y, layer] != currentTile[layer] && Data[x, y, layer] != 0) {
                            currentTile[layer] = Data[x, y, layer];
                        } else {
                            Data[x, y, layer] = currentTile[layer];
                        }
                        Data[x, y, layer] -= 1;
                    }
                }
            }

            Tiles = JsonConvert.DeserializeObject<Tile[ ]>(reader.ReadBytes(reader.ReadInt16( )).Decode( ).Decompress( ));
            Texture = reader.ReadString( );

            Creator = reader.ReadString( );
            Name = reader.ReadString( );
            Gravity = new Vector2(reader.ReadSingle( ), reader.ReadSingle( ));
            Entities = new List<Entity>( );
        }

        private void Deserialize00004(BinaryReader reader, IEntitySerializer serializer) {
            bool is32bit = reader.ReadBoolean( );

            Size = new Size(reader.ReadInt16( ), reader.ReadInt16( ));
            SpawnPoint = new Vector2(reader.ReadInt16( ), reader.ReadInt16( ));

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            // load data
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            Data = new int[Width, Height, 3];

            int currenttile;
            int currentlayer = 0;
            while ((currenttile = is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) != 0) {
                while (currentlayer < 3) {
                    int data = (is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) - 1;
                    if (data == -1) {
                        currentlayer++;
                    } else {
                        int y = (int)(data / Width);
                        int x = data - (int)(y * Width);
                        Data[x, y, currentlayer] = currenttile;
                    }
                }
                currentlayer = 0;
            }

            // uncompress mapdata
            int[ ] currentTile = new int[ ] { -1, -1, -1 };
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    for (int layer = 0; layer < 3; layer++) {
                        if (Data[x, y, layer] != currentTile[layer] && Data[x, y, layer] != 0) {
                            currentTile[layer] = Data[x, y, layer];
                        } else {
                            Data[x, y, layer] = currentTile[layer];
                        }
                        Data[x, y, layer] -= 1;
                    }
                }
            }

            Tiles = JsonConvert.DeserializeObject<Tile[ ]>(reader.ReadBytes(reader.ReadInt16( )).Decode( ).Decompress( ));
            Texture = reader.ReadString( );

            Creator = reader.ReadString( );
            Name = reader.ReadString( );
            Gravity = new Vector2(reader.ReadSingle( ), reader.ReadSingle( ));

            Entities = new List<Entity>(reader.ReadInt16( ));
            EntityID entityID;
            while ((entityID = (EntityID)reader.ReadInt32( )) != EntityID.End) {
                Vector2 position = reader.ReadVector2( );
                Dictionary<DataID, object> data = new Dictionary<DataID, object>(2);
                DataID dataID;
                while ((dataID = (DataID)reader.ReadInt32( )) != DataID.End) {
                    switch ((DataType)reader.ReadByte()) {
                        case DataType.Boolean:
                            data.Add(dataID, reader.ReadBoolean( ));
                            break;
                        case DataType.BooleanArray:
                            data.Add(dataID, reader.ReadBooleanArray( ));
                            break;
                        case DataType.Byte:
                            data.Add(dataID, reader.ReadByte( ));
                            break;
                        case DataType.ByteArray:
                            data.Add(dataID, reader.ReadByteArray( ));
                            break;
                        case DataType.SByte:
                            data.Add(dataID, reader.ReadSByte( ));
                            break;
                        case DataType.SByteArray:
                            data.Add(dataID, reader.ReadSByteArray( ));
                            break;

                        case DataType.Short:
                            data.Add(dataID, reader.ReadInt16( ));
                            break;
                        case DataType.ShortArray:
                            data.Add(dataID, reader.ReadInt16Array( ));
                            break;
                        case DataType.UShort:
                            data.Add(dataID, reader.ReadUInt16( ));
                            break;
                        case DataType.UShortArray:
                            data.Add(dataID, reader.ReadUInt16Array( ));
                            break;
                        case DataType.Int:
                            data.Add(dataID, reader.ReadInt32( ));
                            break;
                        case DataType.IntArray:
                            data.Add(dataID, reader.ReadInt32Array( ));
                            break;
                        case DataType.UInt:
                            data.Add(dataID, reader.ReadUInt32( ));
                            break;
                        case DataType.UIntArray:
                            data.Add(dataID, reader.ReadUInt32Array( ));
                            break;
                        case DataType.Long:
                            data.Add(dataID, reader.ReadInt64( ));
                            break;
                        case DataType.LongArray:
                            data.Add(dataID, reader.ReadInt64Array( ));
                            break;
                        case DataType.ULong:
                            data.Add(dataID, reader.ReadUInt64( ));
                            break;
                        case DataType.ULongArray:
                            data.Add(dataID, reader.ReadUInt64Array( ));
                            break;
                        case DataType.Float:
                            data.Add(dataID, reader.ReadSingle( ));
                            break;
                        case DataType.FloatArray:
                            data.Add(dataID, reader.ReadSingleArray( ));
                            break;
                        case DataType.Double:
                            data.Add(dataID, reader.ReadDouble( ));
                            break;
                        case DataType.DoubleArray:
                            data.Add(dataID, reader.ReadDoubleArray( ));
                            break;
                        case DataType.Decimal:
                            data.Add(dataID, reader.ReadDecimal( ));
                            break;
                        case DataType.DecimalArray:
                            data.Add(dataID, reader.ReadDecimalArray( ));
                            break;
                        case DataType.Vector2:
                            data.Add(dataID, reader.ReadVector2( ));
                            break;
                        case DataType.Vector2Array:
                            data.Add(dataID, reader.ReadVector2Array( ));
                            break;
                        case DataType.String:
                            data.Add(dataID, reader.ReadString( ));
                            break;
                        case DataType.StringArray:
                            data.Add(dataID, reader.ReadStringArray( ));
                            break;
                    }
                }
                serializer.Instantiate(entityID, data, position, this as IEntityWorld);
            }
        }

        #endregion
    }
}