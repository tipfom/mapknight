using System;
using System.Collections.Generic;
using System.Drawing;

using mapKnight.Utils;

namespace mapKnight.ToolKit
{
    class Map
    {
        public string Author;
        public string Name;
        public Point Spawn;

        public int Width;
        public int Height;

        public ushort[,,] Data;

        private Stack<ushort[,,]> Cache = new Stack<ushort[,,]>();

        public Map(XMLElemental config, Dictionary<string, ushort> tileindex, Dictionary<string, ushort> overlayindex)
        {
            Author = config.Attributes["Author"];
            Name = config.Attributes["Name"];
            Spawn = new Point(Convert.ToInt32(config.Attributes["Spawn"].Split(new char[] { ';' })[0]), Convert.ToInt32(config.Attributes["Spawn"].Split(new char[] { ';' })[1]));

            Width = Convert.ToInt32(config["Data"].Attributes["Width"]);
            Height = Convert.ToInt32(config["Data"].Attributes["Height"]);
            Data = new ushort[Width, Height, 2];

            Dictionary<string, ushort[]> StringTileIndex = new Dictionary<string, ushort[]>();

            foreach (string data in config["Def"].Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                ushort tile = (ushort)tileindex[data.Split('=')[1].Split(',')[0]];
                ushort overlay = (ushort)overlayindex[data.Split('=')[1].Split(',')[1]];

                StringTileIndex.Add(data.Split('=')[0], new ushort[] { tile, overlay });
            }

            int cX = 0;
            int cY = 0;
            foreach (string data in config["Data"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string cTile = data.Split('~')[1];
                for (int i = 0; i < Convert.ToInt32(data.Split('~')[0]); i++)
                {
                    Data[cX, cY, 0] = StringTileIndex[cTile][0];
                    Data[cX, cY, 1] = StringTileIndex[cTile][1];

                    cX++;
                    if (cX == Width)
                    {
                        cX = 0;
                        cY++;
                        if (cY == Height)
                        {
                            break;
                        }
                    }
                }
            }

            PrepareUndo();
        }

        public Map(int width, int height, string author, string name, Point spawn)
        {
            Width = width;
            Height = height;
            Author = author;
            Name = name;
            Spawn = spawn;

            Data = new ushort[width, height, 2];

            PrepareUndo();
        }

        public void Fill(int x, int y, ushort replacementtile, ushort replacementoverlay)
        {
            if (Data[x, y, 0] == replacementtile & Data[x, y, 1] == replacementoverlay)
                return;

            bool[,] allreadycomputed = new bool[Width, Height];
            ushort searchingtile = Data[x, y, 0];
            ushort searchingoverlay = Data[x, y, 1];

            Queue<Point> TileQueue = new Queue<Point>();
            TileQueue.Enqueue(new Point(x, y));
            allreadycomputed[x, y] = true;

            while (TileQueue.Count > 0)
            {
                Point ComputingTile = TileQueue.Dequeue();

                if (Data[ComputingTile.X, ComputingTile.Y, 0] == searchingtile && Data[ComputingTile.X, ComputingTile.Y, 1] == searchingoverlay)
                {
                    Data[ComputingTile.X, ComputingTile.Y, 0] = replacementtile;
                    Data[ComputingTile.X, ComputingTile.Y, 1] = replacementoverlay;

                    if (ComputingTile.X > 0 && allreadycomputed[ComputingTile.X - 1, ComputingTile.Y] == false)
                    {
                        TileQueue.Enqueue(new Point(ComputingTile.X - 1, ComputingTile.Y));
                        allreadycomputed[ComputingTile.X - 1, ComputingTile.Y] = true;
                    }
                    if (ComputingTile.X < Width - 1 && allreadycomputed[ComputingTile.X + 1, ComputingTile.Y] == false)
                    {
                        TileQueue.Enqueue(new Point(ComputingTile.X + 1, ComputingTile.Y));
                        allreadycomputed[ComputingTile.X + 1, ComputingTile.Y] = true;
                    }
                    if (ComputingTile.Y > 0 && allreadycomputed[ComputingTile.X, ComputingTile.Y - 1] == false)
                    {
                        TileQueue.Enqueue(new Point(ComputingTile.X, ComputingTile.Y - 1));
                        allreadycomputed[ComputingTile.X, ComputingTile.Y - 1] = true;
                    }
                    if (ComputingTile.Y < Height - 1 && allreadycomputed[ComputingTile.X, ComputingTile.Y + 1] == false)
                    {
                        TileQueue.Enqueue(new Point(ComputingTile.X, ComputingTile.Y + 1));
                        allreadycomputed[ComputingTile.X, ComputingTile.Y + 1] = true;
                    }
                }
            }
        }

        public XMLElemental Save(Dictionary<int, ushort> tileidindex, Dictionary<int, ushort> overlayidindex)
        {
            XMLElemental parsedelemental = XMLElemental.EmptyRootElemental("map");

            parsedelemental.Attributes.Add("Author", Author);
            parsedelemental.Attributes.Add("Name", Name);
            parsedelemental.Attributes.Add("Spawn", Spawn.X.ToString() + ";" + Spawn.X.ToString());

            parsedelemental.AddChild("Def");

            int CurrentIndex = 0;
            Dictionary<string, string> ValueStringIndex = new Dictionary<string, string>();
            for (int tile = 0; tile < tileidindex.Count; tile++)
            {
                for (int overlay = 0; overlay < overlayidindex.Count; overlay++)
                {
                    parsedelemental["Def"].Value += CurrentIndex.ToString() + "=" + tileidindex[tile].ToString() + "," + overlayidindex[overlay].ToString() + ";";
                    ValueStringIndex.Add(tile.ToString() + "*" + overlay.ToString(), CurrentIndex.ToString());
                    CurrentIndex++;
                }
            }

            parsedelemental.AddChild("Data");
            parsedelemental["Data"].Attributes.Add("Width", Width.ToString());
            parsedelemental["Data"].Attributes.Add("Height", Height.ToString());
            int CurrentCount = 0;
            ushort CurrentTile = Data[0, 0, 0];
            ushort CurrentOverlay = Data[0, 0, 1];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (CurrentTile != Data[x, y, 0] || CurrentOverlay != Data[x, y, 1])
                    {
                        parsedelemental["Data"].Value += CurrentCount.ToString() + "~" + ValueStringIndex[CurrentTile.ToString() + "*" + CurrentOverlay.ToString()].ToString() + ",";
                        CurrentTile = Data[x, y, 0];
                        CurrentOverlay = Data[x, y, 1];
                        CurrentCount = 0;
                    }
                    CurrentCount++;
                }
            }
            if (CurrentCount != 0)
            {
                parsedelemental["Data"].Value += CurrentCount.ToString() + "~" + ValueStringIndex[CurrentTile.ToString() + "*" + CurrentOverlay.ToString()].ToString() + ",";
            }
            parsedelemental.AddChild("Background");

            return parsedelemental;
        }

        public void Undo()
        {
            if (Cache.Count > 0)
                Data = Cache.Pop();
        }

        public void PrepareUndo()
        {
            Cache.Push((ushort[,,])Data.Clone());
        }
    }
}
