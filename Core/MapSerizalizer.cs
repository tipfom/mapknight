using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using mapKnight.Core.Exceptions;

namespace mapKnight.Core {
    public static class MapSerizalizer {
        public static byte[ ] HEADER { get; } = { 133, 007, 042, 077, 064, 080 };
        public static short VERSION { get; } = 1;

        public static Map Deserialize (Stream input) {
            using (BinaryReader reader = new BinaryReader(input)) {
                // check for header
                if (!reader.ReadBytes(HEADER.Length).SequenceEqual(HEADER))
                    throw new MissingMapIdentifierException( );
                // load specific mapversion
                short version = reader.ReadInt16( );
                switch (version) {
                    case 1:
                        return Deserialize00001(reader);
                    default:
                        throw new FileLoadException($"map version {version} is not known.");
                }
            }
        }

        public static void Serialize (Stream stream, Map map) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                bool is32bit = (map.Size.Area >= Math.Pow(2, 16));

                // write header
                writer.Write(HEADER);
                writer.Write(VERSION);

                // write data 
                writer.Write(is32bit);
                writer.Write((Int16)map.Width);
                writer.Write((Int16)map.Height);
                writer.Write((Int16)map.SpawnPoint.X);
                writer.Write((Int16)map.SpawnPoint.Y);

                /////////////////////////////////////////////////////////////////////////////////////////
                // compress mapdata
                int[ ] currenttile = { -1, -1, -1 }; // current tile of each layer
                Dictionary<int, List<int>[ ]> startpoints = new Dictionary<int, List<int>[ ]>( );

                for (int y = 0; y < map.Height; y++) {
                    for (uint x = 0; x < map.Width; x++) {
                        for (int layer = 0; layer < 3; layer++) {
                            // go through each layer
                            if (currenttile[layer] != map.Data[x, y, layer]) {
                                // if tile changed
                                if (!startpoints.ContainsKey(map.Data[x, y, layer])) {
                                    startpoints.Add(map.Data[x, y, layer], new List<int>[3]);

                                    startpoints[map.Data[x, y, layer]][0] = new List<int>( );
                                    startpoints[map.Data[x, y, layer]][1] = new List<int>( );
                                    startpoints[map.Data[x, y, layer]][2] = new List<int>( );
                                }
                                // remember startingpoint
                                // invert the y axis
                                startpoints[map.Data[x, y, layer]][layer].Add((int)(y * (map.Size.Width + x)));
                                currenttile[layer] = map.Data[x, y, layer];
                            }
                        }
                    }
                }

                /////////////////////////////////////////////////////////////////////////////////////////
                // write mapdata
                foreach (var tileentry in startpoints) {
                    WriteInt(writer, tileentry.Key + 1, is32bit); // tileid
                    for (int layer = 0; layer < 3; layer++) {
                        tileentry.Value[layer].Sort( );
                        for (int i = 0; i < tileentry.Value[layer].Count; i++) {
                            WriteInt(writer, tileentry.Value[layer][i] + 1, is32bit);
                        }
                        WriteInt(writer, 0, is32bit);
                    }
                }
                WriteInt(writer, 0, is32bit);

                //////////////////////////////////////////////////////////////////////////////////////////
                // write tiles
                byte[ ] tilesraw = JsonConvert.SerializeObject(map.Tiles).Compress( ).Encode( );
                writer.Write(tilesraw.Length);
                writer.Write(tilesraw);

                // write texturename
                writer.Write((short)map.Texture.Length);
                writer.Write(map.Texture.Encode( ));

                // write info
                writer.Write((short)map.Creator.Length);
                writer.Write(map.Creator.Encode( ));
                writer.Write((short)map.Name.Length);
                writer.Write(map.Name.Encode( ));
            }
        }

        private static void WriteInt (BinaryWriter writer, int value, bool is32bit) {
            if (is32bit)
                writer.Write(value);
            else
                writer.Write((short)value);
        }

        #region version specific deserialization
        private static Map Deserialize00001 (BinaryReader reader) {
            Map map = new Map(new Size(reader.ReadInt16( ), reader.ReadInt16( )));
            map.SpawnPoint = new Vector2(reader.ReadInt16( ), reader.ReadInt16( ));

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            // load data
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            map.Data = new int[map.Width, map.Height, 3];

            bool is32bit = reader.ReadBoolean( );

            int currenttile;
            int currentlayer = 0;
            while ((currenttile = is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) != 0) {
                while (currentlayer < 3) {
                    int data = (is32bit ? reader.ReadInt32( ) : reader.ReadInt16( )) - 1;
                    if (data == -1) {
                        currentlayer++;
                    } else {
                        int y = (int)(data / map.Width);
                        int x = data - (int)(y * map.Width);
                        map.Data[x, y, currentlayer] = currenttile;
                    }
                }
                currentlayer = 0;
            }

            // uncompress mapdata
            int[ ] currentTile = new int[ ] { -1, -1, -1 };
            for (int y = 0; y < map.Height; y++) {
                for (int x = 0; x < map.Width; x++) {
                    for (int layer = 0; layer < 3; layer++) {
                        if (map.Data[x, y, layer] != currentTile[layer] && map.Data[x, y, layer] != 0) {
                            currentTile[layer] = map.Data[x, y, layer];
                        } else {
                            map.Data[x, y, layer] = currentTile[layer];
                        }
                        map.Data[x, y, layer] -= 1;
                    }
                }
            }

            map.Tiles = JsonConvert.DeserializeObject<Tile[ ]>(reader.ReadBytes(reader.ReadInt16( )).Decode( ).Decompress( ));
            map.Texture = reader.ReadBytes(reader.ReadInt16( )).Decode( );

            map.Creator = reader.ReadBytes(reader.ReadInt16( )).Decode( );
            map.Name = reader.ReadBytes(reader.ReadInt16( )).Decode( );

            return map;
        }
        #endregion
    }
}
