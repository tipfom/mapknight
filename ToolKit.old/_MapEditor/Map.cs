using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

using XMLElemental = mapKnight.Basic.XMLElemental;

namespace mapKnight.ToolKit {
    class Map {
        public string Creator;
        public string Name;
        public string TileSet;
        public Point Spawn;

        public int Width;
        public int Height;

        public int[,,] Data;

        private Stack<int[,,]> Cache = new Stack<int[,,]> ();

        public Map (XMLElemental config) {
            using (BinaryReader reader = new BinaryReader (new MemoryStream (Encoding.UTF8.GetBytes (config["header"].Value)))) {
                // read header data
                TileSet = reader.ReadInt32 ().ToString ();
                Width = reader.ReadInt32 ();
                Height = reader.ReadInt32 ();
                Spawn = new Point (reader.ReadInt32 (), reader.ReadInt32 ());
            }

            // read mapdata
            Data = new int[Width, Height, 3];
            using (BinaryReader reader = new BinaryReader (new MemoryStream (Encoding.UTF8.GetBytes (config["data"].Value)))) {

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
            }

            int[] currentTile = new int[] { Data[0, 0, 0], Data[0, 0, 1], Data[0, 0, 2] };
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    for (int layer = 0; layer < 3; layer++) {
                        if (Data[x, y, layer] != currentTile[layer]) {
                            currentTile[layer] = Data[x, y, layer];
                        } else {
                            Data[x, y, layer] = currentTile[layer];
                        }

                        Data[x, y, layer] -= 1;
                    }
                }
            }

            // read info
            using (BinaryReader reader = new BinaryReader (new MemoryStream (Encoding.UTF8.GetBytes (config["info"].Value)))) {
                int infohash = reader.ReadInt32 ();
                int infostringlength = reader.ReadInt32 ();
                string infostring = Encoding.UTF8.GetString (reader.ReadBytes (infostringlength));
                string[] info = infostring.Split (new char[] { Encoding.UTF8.GetChars (new byte[] { 255 })[0] });
                Creator = info[0];
                Name = info[1];

                if (infostring.GetHashCode () != infohash) {
                    throw new InvalidDataException ("map has been modified!");
                }
            }

            PrepareUndo ();
        }

        public Map (int width, int height, string creator, string name, Point spawn) {
            Width = width;
            Height = height;
            Creator = creator;
            Name = name;
            Spawn = spawn;

            Data = new int[width, height, 3];

            PrepareUndo ();
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

        public XMLElemental Save (string tileset) {
            XMLElemental parsedelemental = XMLElemental.EmptyRootElemental ("map");

            byte[] header = new byte[20]; // 4 uint, hash of tilesetname
            Array.Copy (BitConverter.GetBytes (tileset.GetHashCode ()), 0, header, 0, 4);
            Array.Copy (BitConverter.GetBytes (this.Width), 0, header, 4, 4);
            Array.Copy (BitConverter.GetBytes (this.Height), 0, header, 8, 4);
            Array.Copy (BitConverter.GetBytes (this.Spawn.X), 0, header, 12, 4);
            Array.Copy (BitConverter.GetBytes (this.Spawn.Y), 0, header, 16, 4);

            parsedelemental.AddChild ("header").Value = Encoding.UTF8.GetString (header);

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

            byte[] data = new byte[(startpoints.Count * 4 + entrycount) * 4]; //every tile (tile + 3 * 0 uint) and entry are a uint (4 byte)

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

            parsedelemental.AddChild ("data").Value = Encoding.UTF8.GetString (data);

            // save additional map info info (at the end) with hash to secure, that the map does not get changed
            // get byte of infostring (data seperated by last char of utf-8)
            byte[] infostring = Encoding.UTF8.GetBytes (String.Join (Encoding.UTF8.GetString (new byte[] { 255 }), this.Creator, this.Name).ToCharArray ());
            byte[] info = new byte[infostring.Length + 4]; // infostring, hash
            Array.Copy (BitConverter.GetBytes (infostring.GetHashCode ()), 0, info, 0, 4);
            Array.Copy (infostring, 0, info, 4, infostring.Length);

            parsedelemental.AddChild ("info").Value = Encoding.UTF8.GetString (info);

            return parsedelemental;
        }

        public void Save (string filename, string tileset) {
            using (FileStream filestream = new FileStream (Path.ChangeExtension (filename, "tmsl4"), FileMode.OpenOrCreate)) {
                byte[] header = new byte[20]; // 4 uint, hash of tilesetname
                Array.Copy (BitConverter.GetBytes (tileset.GetHashCode ()), 0, header, 0, 4);
                Array.Copy (BitConverter.GetBytes (this.Width), 0, header, 4, 4);
                Array.Copy (BitConverter.GetBytes (this.Height), 0, header, 8, 4);
                Array.Copy (BitConverter.GetBytes (this.Spawn.X), 0, header, 12, 4);
                Array.Copy (BitConverter.GetBytes (this.Spawn.Y), 0, header, 16, 4);

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

                byte[] data = new byte[(startpoints.Count * 4 + entrycount) * 4]; //every tile (tile + 3 * 0 uint) and entry are a uint (4 byte)

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

                filestream.Write (data, 0, data.Length);

                // save additional map info info (at the end) with hash to secure, that the map does not get changed
                // get byte of infostring (data seperated by last char of utf-8)
                byte[] infostring = Encoding.UTF8.GetBytes (String.Join (Encoding.UTF8.GetString (new byte[] { 255 }), this.Creator, this.Name).ToCharArray ());
                byte[] info = new byte[infostring.Length + 4]; // infostring, hash
                Array.Copy (BitConverter.GetBytes (infostring.GetHashCode ()), 0, info, 0, 4);
                Array.Copy (infostring, 0, info, 4, infostring.Length);

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
    }
}
