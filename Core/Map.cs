using mapKnight.Core.Exceptions;
using mapKnight.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace mapKnight.Core {
    public class Map {
        const byte INFO_SPERATOR = 255;
        const CompressionLevel COMPRESSION_LEVEL = CompressionLevel.Optimal;
        public static byte[ ] IDENTIFIER = { 133, 7, 42, 84, 77, 83, 76, 52 };

        // x,y,layer
        private int[ , , ] Data;

        public Size Size { get; private set; }
        public Vector2 SpawnPoint;
        private Tile[ ] tiles = new Tile[1];
        private Texture2D texture;
        public string Creator;
        public string Name;

        public float VertexSize { get; protected set; }

        public Map (Stream stream) {
            using (BinaryReader reader = new BinaryReader(stream.Decompress( ))) {
                if (!reader.ReadBytes(IDENTIFIER.Length).SequenceEqual(IDENTIFIER))
                    throw new MissingMapIdentifierException( );

                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                // load header
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                Size = new Size(reader.ReadInt16( ), reader.ReadInt16( ));
                SpawnPoint = new Vector2(reader.ReadInt16( ), reader.ReadInt16( ));

                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                // load data
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                Data = new int[Size.Width, Size.Height, 3];

                bool is32bit = reader.ReadBoolean( );

                int currenttile;
                int currentlayer = 0;
                while ((currenttile = is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) != 0) {
                    while (currentlayer < 3) {
                        int data = (is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) - 1;
                        if (data == -1) {
                            currentlayer++;
                        } else {
                            int y = (int)(data / Size.Width);
                            int x = data - (int)(y * Size.Width);
                            Data[x, y, currentlayer] = currenttile;
                        }
                    }
                    currentlayer = 0;
                }

                // uncompress mapdata
                int[ ] currentTile = new int[ ] { -1, -1, -1 };
                for (int y = 0; y < Size.Height; y++) {
                    for (int x = 0; x < Size.Width; x++) {
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

                byte[ ] mapinforaw = reader.ReadBytes(reader.ReadInt32( ));
                string[ ] mapinfo = mapinforaw.Decode( ).Split(Convert.ToChar(INFO_SPERATOR));
                Creator = mapinfo[0];
                Name = mapinfo[1];
                if (!reader.ReadBytes(reader.ReadInt32( )).SequenceEqual(mapinforaw.ComputeHash( )))
                    throw new ChangedHashException(new MapLoadException( ));

                byte[ ] tilesraw = reader.ReadBytes(reader.ReadInt32( ));
                string tiles = tilesraw.Decode( ).Decompress( );
                JsonConvert.PopulateObject(tiles, this.tiles);
                if (!reader.ReadBytes(reader.ReadInt32( )).SequenceEqual(tilesraw.ComputeHash( )))
                    throw new ChangedHashException(new MapLoadException( ));

                string texturename = reader.ReadBytes(reader.ReadInt32( )).Decode( );
                texture = Assets.Load<Texture2D>(texturename);
            }
        }

        public void Flush (Stream targetstream) {
            Flush(targetstream, texture.Name);
        }

        public void Flush (Stream targetstream, string textureName) {
            using (GZipStream zipstream = new GZipStream(targetstream, COMPRESSION_LEVEL))
            using (BinaryWriter writer = new BinaryWriter(zipstream)) {
                // write identifies
                writer.Write(IDENTIFIER);

                // write header
                writer.Write((short)Size.Width);
                writer.Write((short)Size.Height);
                writer.Write((short)SpawnPoint.X);
                writer.Write((short)SpawnPoint.Y);

                bool is32bit = (this.Size.Area < Math.Pow(2, 16));
                writer.Write(is32bit);

                /////////////////////////////////////////////////////////////////////////////////////////
                // compress mapdata
                int[ ] currenttile = { -1, -1, -1 }; // current tile of each layer
                Dictionary<int, List<int>[ ]> startpoints = new Dictionary<int, List<int>[ ]>( );

                for (int y = this.Size.Height - 1; y >= 0; y--) {
                    for (uint x = 0; x < this.Size.Width; x++) {
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
                                startpoints[Data[x, y, layer]][layer].Add((int)((this.Size.Height - y - 1) * (this.Size.Width + x)));
                                currenttile[layer] = Data[x, y, layer];
                            }
                        }
                    }
                }

                /////////////////////////////////////////////////////////////////////////////////////////
                // write data
                foreach (var tileentry in startpoints) {
                    WriteInt(writer, tileentry.Key, is32bit); // tileid
                    for (int layer = 0; layer < 3; layer++) {
                        tileentry.Value[layer].Sort( );
                        for (int i = 0; i < tileentry.Value[layer].Count; i++) {
                            WriteInt(writer, tileentry.Value[layer][i], is32bit);
                        }
                        WriteInt(writer, 0, is32bit);
                    }
                }
                WriteInt(writer, 0, is32bit);

                //////////////////////////////////////////////////////////////////////////////////////////
                // write info
                byte[ ] mapinfo = String.Join(Convert.ToChar(INFO_SPERATOR).ToString( ), Creator, Name).Encode( );
                writer.Write(mapinfo.Length);
                writer.Write(mapinfo);
                byte[ ] mapinfohash = mapinfo.ComputeHash( );
                writer.Write(mapinfohash.Length);
                writer.Write(mapinfohash);

                //////////////////////////////////////////////////////////////////////////////////////////
                // write tiles
                byte[ ] tilesraw = JsonConvert.SerializeObject(tiles).Encode( );
                writer.Write(tilesraw.Length);
                writer.Write(tilesraw);
                byte[ ] tilesrawhash = tilesraw.ComputeHash( );
                writer.Write(tilesrawhash.Length);
                writer.Write(tilesrawhash);

                // write texturename
                writer.Write(textureName.Length);
                writer.Write(textureName.Encode( ));
            }
        }

        private void WriteInt (BinaryWriter writer, int number, bool is32bit) {
            if (is32bit)
                writer.Write(number);
            else
                writer.Write((short)number);
        }

        public Tile GetTile (int x, int y, int layer) {
            return tiles[Data[x, y, layer]];
        }

        public Tile GetTileBackground (int x, int y) {
            return tiles[Data[x, y, 0]];
        }

        public Tile GetTile (int x, int y) {
            return tiles[Data[x, y, 1]];
        }

        public Tile GetTileForeground (int x, int y) {
            return tiles[Data[x, y, 2]];
        }
    }
}