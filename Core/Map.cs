using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using mapKnight.Core.Exceptions;
using Newtonsoft.Json;

namespace mapKnight.Core {
    public class Map {
        public const int TILE_PXL_SIZE = 18;
        public static byte[ ] HEADER { get; } = { 133, 007, 042, 077, 064, 080 };
        public static short VERSION { get; } = 2;

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

        public Map (Size size, string creator, string name) {
            Size = size;
            Data = new int[size.Width, size.Height, 3];
            Creator = creator;
            Name = name;
            SpawnPoint = new Vector2( );
        }

        public Map (Size size) : this(size, null, null) {

        }

        public Map (Stream input) {
            using (BinaryReader reader = new BinaryReader(input)) {
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
                    default:
                        throw new FileLoadException($"map version {version} is not known.");
                }
            }
        }

        public void Serialize (Stream stream) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
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
                writer.Write((short)Texture.Length);
                writer.Write(Texture.Encode( ));

                // write info
                writer.Write((short)Creator.Length);
                writer.Write(Creator.Encode( ));
                writer.Write((short)Name.Length);
                writer.Write(Name.Encode( ));
                writer.Write(Gravity.X);
                writer.Write(Gravity.Y);
            }
        }

        public Tile GetTile (int x, int y, int layer) {
            return Tiles[Data[x, y, layer]];
        }

        public Tile GetTileBackground (int x, int y) {
            return Tiles[Data[x, y, 0]];
        }

        public Tile GetTile (int x, int y) {
            return Tiles[Data[x, y, 1]];
        }

        public Tile GetTileForeground (int x, int y) {
            return Tiles[Data[x, y, 2]];
        }

        public override string ToString ( ) {
            return $"{Name} ({Size.Width}x{Size.Height})";
        }

        #region deserialization
        private void Deserialize00001 (BinaryReader reader) {
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
            Gravity = new Vector2(0, -10);
        }

        private void Deserialize00002 (BinaryReader reader) {
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
            Gravity = new Vector2(0, -10);
        }
        #endregion
    }
}