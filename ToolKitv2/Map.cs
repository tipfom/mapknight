using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using XMLElemental = mapKnight.Basic.XMLElemental;

namespace mapKnight.ToolKit {
    class Map {
        private static byte[] identifier = new byte[] { 84, 77, 83, 76, 4, 42, 133, 7 };

        public string Creator;
        public string Name;
        public int TileSet;
        public Point Spawn;

        public int Width;
        public int Height;

        public int[,,] Data;

        private Stack<int[,,]> Cache = new Stack<int[,,]> ();

        public Map (XMLElemental config) {
            using (BinaryReader reader = new BinaryReader (new MemoryStream (Convert.FromBase64String (config["header"].Attributes["content"])))) {
                // read header data
                TileSet = reader.ReadInt32 ();
                Width = reader.ReadInt32 ();
                Height = reader.ReadInt32 ();
                Spawn = new Point (reader.ReadInt32 (), reader.ReadInt32 ());
            }

            // read mapdata
            Data = new int[Width, Height, 3];
            using (BinaryReader reader = new BinaryReader (new MemoryStream (Convert.FromBase64String (config["data"].Attributes["content"])))) {

                int currenttile = reader.ReadInt32 ();
                int currentlayer = 0;
                while (currenttile != 0) {
                    while (currentlayer < 3) {
                        int data = reader.ReadInt32 () - 1;
                        if (data == -1) {
                            currentlayer++;
                        } else {
                            int y = (int)(data / Width);
                            int x = data - (int)(y * Width);
                            Data[x, y, currentlayer] = currenttile;
                        }
                    }
                    currenttile = reader.ReadInt32 ();
                    currentlayer = 0;
                }
            }

            int[] currentTile = new int[] { -1, -1, -1 };
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

            // read info
            using (BinaryReader reader = new BinaryReader (new MemoryStream (Convert.FromBase64String (config["info"].Attributes["content"])))) {
                int infostringlength = reader.ReadInt32 ();
                byte[] infostringbytes = reader.ReadBytes (infostringlength);
                int hashlength = reader.ReadInt32 ();
                byte[] infohash = reader.ReadBytes (hashlength);
                string infostring = Encoding.UTF8.GetString (infostringbytes);
                string[] info = infostring.Split (new char[] { Encoding.UTF8.GetChars (new byte[] { 255 })[0] });
                Creator = info[0];
                Name = info[1];
                byte[] realhash = SHA1.Create ().ComputeHash (infostringbytes);
                if (!realhash.SequenceEqual (infohash)) {
                    throw new InvalidDataException ("map has been modified!");
                }
            }

            PrepareUndo ();
        }

        public Map (int width, int height, string creator, string name, int tileset, Point spawn) {
            Width = width;
            Height = height;
            Creator = creator;
            Name = name;
            Spawn = spawn;
            TileSet = tileset;

            Data = new int[width, height, 3];

            PrepareUndo ();
        }

        public Map (byte[] file) {
            using (BinaryReader reader = new BinaryReader (new MemoryStream (file))) {
                if (reader.ReadBytes (Map.identifier.Length).SequenceEqual (Map.identifier)) {
                    // map is certified
                    // read header data
                    TileSet = reader.ReadInt32 ();
                    Width = reader.ReadInt32 ();
                    Height = reader.ReadInt32 ();
                    Spawn = new Point (reader.ReadInt32 (), reader.ReadInt32 ());

                    // read mapdata

                    Data = new int[Width, Height, 3];

                    int currenttile = reader.ReadInt32 ();
                    int currentlayer = 0;
                    while (currenttile != 0) {
                        while (currentlayer < 3) {
                            int data = reader.ReadInt32 ();
                            if (data == 0) {
                                currentlayer++;
                            } else {
                                int y = (int)(data / Width);
                                int x = data - y * Width;
                                Data[x, y, currentlayer] = currenttile;
                            }
                        }
                        currenttile = reader.ReadInt32 ();
                    }

                    // read info
                    int infohash = reader.ReadInt32 ();
                    int infostringlength = reader.ReadInt32 ();
                    string infostring = Encoding.UTF8.GetString (reader.ReadBytes (infostringlength));
                    string[] info = infostring.Split (new char[] { Encoding.UTF8.GetChars (new byte[] { 255 })[0] });
                    Creator = info[0];
                    Name = info[1];

                    if (infostring.GetHashCode () != infohash) {
                        throw new InvalidDataException ("map has been modified!");
                    }
                } else {
                    MessageBox.Show ("the map you tried to load wasn't certified by the toolkit", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void Fill (int x, int y, int[] replacement) {
            if (Data[x, y, 0] == replacement[0] & Data[x, y, 1] == replacement[1] & Data[x, y, 2] == replacement[2])
                return;

            bool[,] allreadycomputed = new bool[Width, Height];
            int[] searching = new int[3];
            searching[0] = Data[x, y, 0];
            searching[1] = Data[x, y, 1];
            searching[2] = Data[x, y, 2];

            Queue<Point> TileQueue = new Queue<Point> ();
            TileQueue.Enqueue (new Point (x, y));
            allreadycomputed[x, y] = true;

            while (TileQueue.Count > 0) {
                Point ComputingTile = TileQueue.Dequeue ();

                if (Data[ComputingTile.X, ComputingTile.Y, 0] == searching[0] && Data[ComputingTile.X, ComputingTile.Y, 1] == searching[1] && Data[ComputingTile.X, ComputingTile.Y, 2] == searching[2]) {
                    Data[ComputingTile.X, ComputingTile.Y, 0] = replacement[0];
                    Data[ComputingTile.X, ComputingTile.Y, 1] = replacement[1];
                    Data[ComputingTile.X, ComputingTile.Y, 2] = replacement[2];

                    if (ComputingTile.X > 0 && allreadycomputed[ComputingTile.X - 1, ComputingTile.Y] == false) {
                        TileQueue.Enqueue (new Point (ComputingTile.X - 1, ComputingTile.Y));
                        allreadycomputed[ComputingTile.X - 1, ComputingTile.Y] = true;
                    }
                    if (ComputingTile.X < Width - 1 && allreadycomputed[ComputingTile.X + 1, ComputingTile.Y] == false) {
                        TileQueue.Enqueue (new Point (ComputingTile.X + 1, ComputingTile.Y));
                        allreadycomputed[ComputingTile.X + 1, ComputingTile.Y] = true;
                    }
                    if (ComputingTile.Y > 0 && allreadycomputed[ComputingTile.X, ComputingTile.Y - 1] == false) {
                        TileQueue.Enqueue (new Point (ComputingTile.X, ComputingTile.Y - 1));
                        allreadycomputed[ComputingTile.X, ComputingTile.Y - 1] = true;
                    }
                    if (ComputingTile.Y < Height - 1 && allreadycomputed[ComputingTile.X, ComputingTile.Y + 1] == false) {
                        TileQueue.Enqueue (new Point (ComputingTile.X, ComputingTile.Y + 1));
                        allreadycomputed[ComputingTile.X, ComputingTile.Y + 1] = true;
                    }
                }
            }
        }

        public XMLElemental Save () {
            XMLElemental parsedelemental = XMLElemental.EmptyRootElemental ("map");

            byte[] header = new byte[20]; // 4 uint, hash of tilesetname
            Array.Copy (BitConverter.GetBytes (this.TileSet), 0, header, 0, 4);
            Array.Copy (BitConverter.GetBytes (this.Width), 0, header, 4, 4);
            Array.Copy (BitConverter.GetBytes (this.Height), 0, header, 8, 4);
            Array.Copy (BitConverter.GetBytes (this.Spawn.X), 0, header, 12, 4);
            Array.Copy (BitConverter.GetBytes (this.Spawn.Y), 0, header, 16, 4);

            parsedelemental.AddChild ("header").Attributes.Add ("content", Convert.ToBase64String (header));

            int entrycount = 0;
            int[] currenttile = new int[3]; // current tile of each layer
            Dictionary<int, List<uint>[]> startpoints = new Dictionary<int, List<uint>[]> ();

            // set start variables
            currenttile[0] = -1;
            currenttile[1] = -1;
            currenttile[2] = -1;

            for (uint y = 0; y < this.Height; y++) {
                for (uint x = 0; x < this.Width; x++) {
                    for (int layer = 0; layer < 3; layer++) {
                        // go through each layer
                        if (currenttile[layer] != Data[x, y, layer]) {
                            // if tile changed
                            if (!startpoints.ContainsKey (Data[x, y, layer])) {
                                startpoints.Add (Data[x, y, layer], new List<uint>[3]);

                                startpoints[Data[x, y, layer]][0] = new List<uint> ();
                                startpoints[Data[x, y, layer]][1] = new List<uint> ();
                                startpoints[Data[x, y, layer]][2] = new List<uint> ();
                            }
                            // remember startingpoint
                            startpoints[Data[x, y, layer]][layer].Add (y * (uint)this.Width + x);
                            entrycount++;
                            currenttile[layer] = Data[x, y, layer];
                        }
                    }
                }
            }

            byte[] data = new byte[(startpoints.Count * 4 + entrycount) * 4 + 4]; //every tile (3 layer + tile id) and entry are a int (4 byte) + last 0 int

            int currentindex = 0;
            foreach (KeyValuePair<int, List<uint>[]> tilentry in startpoints) {
                Array.Copy (BitConverter.GetBytes (tilentry.Key + 1), 0, data, currentindex, 4);
                currentindex += 4;

                for (int layer = 0; layer < 3; layer++) {
                    for (int i = 0; i < tilentry.Value[layer].Count; i++) {
                        Array.Copy (BitConverter.GetBytes (tilentry.Value[layer][i] + 1), 0, data, currentindex, 4);
                        currentindex += 4;
                    }

                    Array.Copy (BitConverter.GetBytes (0), 0, data, currentindex, 4);
                    currentindex += 4;
                }
            }
            Array.Copy (BitConverter.GetBytes (0), 0, data, currentindex, 4);

            parsedelemental.AddChild ("data").Attributes.Add ("content", Convert.ToBase64String (data));

            // save additional map info info (at the end) with hash to secure, that the map does not get changed
            // get byte of infostring (data seperated by last char of utf-8)
            byte[] infostring = Encoding.UTF8.GetBytes (String.Join (Encoding.UTF8.GetString (new byte[] { 255 }), this.Creator, this.Name).ToCharArray ());
            byte[] hash = SHA1.Create ().ComputeHash (infostring);
            byte[] info = new byte[infostring.Length + 4 + hash.Length + 4]; // infostring, infostring-length, hash, hashlength
            Array.Copy (BitConverter.GetBytes (infostring.Length), 0, info, 0, 4);
            Array.Copy (infostring, 0, info, 4, infostring.Length);
            Array.Copy (BitConverter.GetBytes (hash.Length), 0, info, infostring.Length + 4, 4);
            Array.Copy (hash, 0, info, infostring.Length + 8, hash.Length);

            parsedelemental.AddChild ("info").Attributes.Add ("content", Convert.ToBase64String (info));

            return parsedelemental;
        }

        public void Export (string filename) {
            using (FileStream filestream = new FileStream (Path.ChangeExtension (filename, "map"), FileMode.Create)) {
                filestream.Write (Map.identifier, 0, Map.identifier.Length);

                byte[] header = new byte[4 + 4 * 2]; // 4 short, hash of tilesetname
                Array.Copy (BitConverter.GetBytes (this.TileSet), 0, header, 0, 4);
                Array.Copy (BitConverter.GetBytes ((short)this.Width), 0, header, 4, 2);
                Array.Copy (BitConverter.GetBytes ((short)this.Height), 0, header, 6, 2);
                Array.Copy (BitConverter.GetBytes ((short)this.Spawn.X), 0, header, 8, 2);
                Array.Copy (BitConverter.GetBytes ((short)this.Spawn.Y), 0, header, 10, 2);

                filestream.Write (header, 0, header.Length);

                int entrycount = 0;
                int[] currenttile = new int[3]; // current tile of each layer
                Dictionary<int, List<uint>[]> startpoints = new Dictionary<int, List<uint>[]> ();

                // set start variables
                currenttile[0] = -1;
                currenttile[1] = -1;
                currenttile[2] = -1;

                for (uint y = 0; y < this.Height; y++) {
                    for (uint x = 0; x < this.Width; x++) {
                        for (int layer = 0; layer < 3; layer++) {
                            // go through each layer
                            if (currenttile[layer] != Data[x, y, layer]) {
                                // if tile changed
                                if (!startpoints.ContainsKey (Data[x, y, layer])) {
                                    startpoints.Add (Data[x, y, layer], new List<uint>[3]);

                                    startpoints[Data[x, y, layer]][0] = new List<uint> ();
                                    startpoints[Data[x, y, layer]][1] = new List<uint> ();
                                    startpoints[Data[x, y, layer]][2] = new List<uint> ();
                                }
                                // remember startingpoint
                                startpoints[Data[x, y, layer]][layer].Add (y * (uint)this.Width + x);
                                entrycount++;
                                currenttile[layer] = Data[x, y, layer];
                            }
                        }
                    }
                }

                byte[] data;
                if (this.Width * this.Height + 1 < 65536) {
                    filestream.WriteByte (1); // id to know, its 16-bit based
                    // write 16bit-numbers to save file length
                    data = new byte[(startpoints.Count * 4 + entrycount) * 2 + 2]; //every tile (tile + 3 * 0 uint) and entry are a short (2 byte)

                    int currentindex = 0;
                    foreach (KeyValuePair<int, List<uint>[]> tilentry in startpoints) {
                        Array.Copy (BitConverter.GetBytes ((short)tilentry.Key + 1), 0, data, currentindex, 2);
                        currentindex += 2;

                        for (int layer = 0; layer < 3; layer++) {
                            for (int i = 0; i < tilentry.Value[layer].Count; i++) {
                                Array.Copy (BitConverter.GetBytes ((short)tilentry.Value[layer][i] + 1), 0, data, currentindex, 2);
                                currentindex += 2;
                            }

                            Array.Copy (BitConverter.GetBytes ((short)0), 0, data, currentindex, 2);
                            currentindex += 2;
                        }
                    }
                    Array.Copy (BitConverter.GetBytes ((short)0), 0, data, currentindex, 2);
                } else {
                    filestream.WriteByte (2); // id to know, its 32-bit based
                    // write 32bit-numbers
                    data = new byte[(startpoints.Count * 4 + entrycount) * 4 + 4]; //every tile (tile + 3 * 0 uint) and entry are a int (4 byte)

                    int currentindex = 0;
                    foreach (KeyValuePair<int, List<uint>[]> tilentry in startpoints) {
                        Array.Copy (BitConverter.GetBytes (tilentry.Key + 1), 0, data, currentindex, 4);
                        currentindex += 4;

                        for (int layer = 0; layer < 3; layer++) {
                            for (int i = 0; i < tilentry.Value[layer].Count; i++) {
                                Array.Copy (BitConverter.GetBytes (tilentry.Value[layer][i] + 1), 0, data, currentindex, 4);
                                currentindex += 4;
                            }

                            Array.Copy (BitConverter.GetBytes (0), 0, data, currentindex, 4);
                            currentindex += 4;
                        }
                    }
                    Array.Copy (BitConverter.GetBytes (0), 0, data, currentindex, 4);
                }

                filestream.Write (data, 0, data.Length);

                // save additional map info info (at the end) with hash to secure, that the map does not get changed
                // get byte of infostring (data seperated by last char of utf-8)
                byte[] infostring = Encoding.UTF8.GetBytes (String.Join (Encoding.UTF8.GetString (new byte[] { 255 }), this.Creator, this.Name).ToCharArray ());
                byte[] hash = SHA1.Create ().ComputeHash (infostring);
                byte[] info = new byte[infostring.Length + 4 + hash.Length + 4]; // infostring, infostring-length, hash, hashlength
                Array.Copy (BitConverter.GetBytes (infostring.Length), 0, info, 0, 4);
                Array.Copy (infostring, 0, info, 4, infostring.Length);
                Array.Copy (BitConverter.GetBytes (hash.Length), 0, info, infostring.Length + 4, 4);
                Array.Copy (hash, 0, info, infostring.Length + 8, hash.Length);

                filestream.Write (info, 0, info.Length);
            }
        }

        public void Undo () {
            if (Cache.Count > 0)
                Data = Cache.Pop ();
        }

        public void PrepareUndo () {
            Cache.Push ((int[,,])Data.Clone ());
        }

        public override string ToString () {
            return Name;
        }
    }
}
