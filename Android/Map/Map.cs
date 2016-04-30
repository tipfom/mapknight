using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using mapKnight.Basic;

namespace mapKnight.Android.Map {
    public class Map {
        public static byte[ ] IDENTIFIER = { 84, 77, 83, 76, 4, 42, 133, 7 };

        // x,y,layer
        private int[ , , ] Data;

        public Size Size;

        public string Creator;
        public string Name;
        public fPoint SpawnPoint;
        public TileManager TileManager;

        public Map (string filename) {
            if (Path.GetExtension (filename) != ".map")
                Log.Warn (this, "selected map may isnt certified");
            Load (Content.Context.Assets.Open (Path.Combine ("maps", filename)));
        }

        public Map (byte[ ] data) {
            Load (data);
        }

        public Map (Stream stream) {
            Load (stream);
        }

        private void Load (Stream stream) {
            using (BinaryReader reader = new BinaryReader (stream)) {
                // check if file has the map prebytes
                if (reader.ReadBytes (8).SequenceEqual (IDENTIFIER)) {
                    // read header and load tileset
                    Size = new Size ((int)reader.ReadInt16 ( ), (int)reader.ReadInt16 ( ));
                    SpawnPoint = new fPoint ((float)reader.ReadInt16 ( ), (float)reader.ReadInt16 ( ));
                    TileManager = new TileManager (Encoding.UTF8.GetString (reader.ReadBytes (reader.ReadInt16 ( ))));
                    Data = new int[Size.Width, Size.Height, 3];

                    // read in mapdata
                    switch (reader.ReadByte ( )) {
                    case 1:
                        // map is 16-bit decoded
                        int currenttile = (int)reader.ReadInt16 ( );
                        int currentlayer = 0;
                        while (currenttile != 0) {
                            while (currentlayer < 3) {
                                int data = (int)reader.ReadInt16 ( ) - 1;
                                if (data == -1) {
                                    currentlayer++;
                                } else {
                                    int y = (int)(data / Size.Width);
                                    int x = data - (int)(y * Size.Width);
                                    Data[x, y, currentlayer] = currenttile;
                                }
                            }
                            currenttile = (int)reader.ReadInt16 ( );
                            currentlayer = 0;
                        }
                        break;
                    case 2:
                        // map is 32-bit decoded
                        currenttile = (int)reader.ReadInt32 ( );
                        currentlayer = 0;
                        while (currenttile != 0) {
                            while (currentlayer < 3) {
                                int data = (int)reader.ReadInt32 ( ) - 1;
                                if (data == -1) {
                                    currentlayer++;
                                } else {
                                    int y = this.Size.Height - (int)(data / Size.Width);
                                    int x = data - (int)(y * Size.Width);
                                    Data[x, y, currentlayer] = currenttile;
                                }
                            }
                            currenttile = (int)reader.ReadInt32 ( );
                            currentlayer = 0;
                        }
                        break;
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

                    // load and verify infodata
                    int infostringlength = reader.ReadInt32 ( );
                    byte[ ] infostringbytes = reader.ReadBytes (infostringlength);
                    int hashlength = reader.ReadInt32 ( );
                    byte[ ] infohash = reader.ReadBytes (hashlength);
                    string infostring = Encoding.UTF8.GetString (infostringbytes);
                    string[ ] info = infostring.Split (new char[ ] { Encoding.UTF8.GetChars (new byte[ ] { 255 })[0] });
                    Creator = info[0];
                    Name = info[1];
                    byte[ ] realhash = SHA1.Create ( ).ComputeHash (infostringbytes); // verify
                    if (!realhash.SequenceEqual (infohash)) {
                        throw new InvalidDataException ("map has been modified!");
                    }
                } else {
                    Log.Warn (this, "selected file isnt a tmsl4-map");
                }
            }
        }

        private void Load (byte[ ] data) {
            using (MemoryStream stream = new MemoryStream (data)) {
                Load (stream);
            }
        }

        public Tile GetTile (int x, int y, int layer) {
            return TileManager.GetTile (Data[x, y, layer]);
        }

        public Tile GetTileL1 (int x, int y) {
            return TileManager.GetTile (Data[x, y, 0]);
        }

        public Tile GetTileL2 (int x, int y) {
            return TileManager.GetTile (Data[x, y, 1]);
        }

        public Tile GetTileL3 (int x, int y) {
            return TileManager.GetTile (Data[x, y, 2]);
        }
    }
}