using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using mapKnight.Basic;
using Newtonsoft.Json;

namespace mapKnight.ToolKit {
    public class Tileset {
        public readonly string Name;
        public List<Tile> Tiles = new List<Tile> ( );

        private Bitmap texture;

        public Tileset (string name) {
            Name = name;
            this.Tiles.Add (new Tile ("None", new Bitmap (1, 1)) { MaskFlag = new string[ ] { } });
        }

        public Tileset (XMLElemental config) {
            Name = config.Attributes["name"];
            using (MemoryStream mstream = new MemoryStream (Convert.FromBase64String (config.Attributes["texture"]))) {
                texture = new Bitmap (mstream);
            }

            foreach (XMLElemental tile in config.GetAll ( )) {
                this.Tiles.Add (new Tile (tile, texture));
            }
        }

        public override string ToString () {
            return Name + " ( " + this.Tiles.Count + " items ) ";
        }

        public Image[ ] GetBitmapsOrdered () {
            Image[ ] bitmaps = new Image[Tiles.Count];

            for (int i = 0; i < Tiles.Count; i++) {
                bitmaps[i] = (Image)Tiles[i].Texture;
            }

            return bitmaps;
        }

        private struct ExportTile {
            public int X;
            public int Y;
            public string Name;
            public string[ ] MaskFlag;
            public Dictionary<string, string> Attributes;
        }

        private struct ExportData {
            public string Name;
            public string Texture;
            public Dictionary<int, ExportTile> Tiles;
        }

        public void Export (string filename) {
            using (FileStream stream = File.OpenWrite (filename)) {
                using (StreamWriter writer = new StreamWriter (stream)) {
                    int imagewidthtile = (int)(Math.Sqrt (this.Tiles.Count)) + 1;
                    int imagewidth = imagewidthtile * (Tile.TILE_SIZE + 1); // + offset
                    int imagewidthpower = (int)Math.Log (imagewidth, 2) + 1;
                    if (imagewidthpower > 12)
                        throw new ArithmeticException ("too many tiles in tileset, may some devices cant handle the amount of data produced");

                    Bitmap exportedTexture = new Bitmap (imagewidth, imagewidth);
                    Dictionary<int, ExportTile> tileData = new Dictionary<int, ExportTile> ( );

                    using (Graphics g = Graphics.FromImage (exportedTexture)) {
                        for (int y = 0; y <= (int)(this.Tiles.Count / imagewidthtile) + 1; y++) {
                            for (int x = 0; x < Math.Min (imagewidthtile, this.Tiles.Count - y * imagewidthtile); x++) {
                                g.DrawImage (this.Tiles[y * imagewidthtile + x].Texture, x * (Tile.TILE_SIZE + 1), y * (Tile.TILE_SIZE + 1), Tile.TILE_SIZE, Tile.TILE_SIZE);
                                int currentIndex = y * imagewidthtile + x;
                                tileData.Add (currentIndex, new ExportTile {
                                    Name = this.Tiles[currentIndex].Name,
                                    X = x * Tile.TILE_SIZE + x, // + offset
                                    Y = y * Tile.TILE_SIZE + y,
                                    MaskFlag = this.Tiles[currentIndex].MaskFlag,
                                    Attributes = this.Tiles[currentIndex].Attributes
                                });
                            }
                        }
                    }

                    writer.WriteLine (JsonConvert.SerializeObject (new ExportData { Name = this.Name, Texture = $"{this.Name}.png", Tiles = tileData }, Formatting.Indented));
                    exportedTexture.Save (Path.ChangeExtension (filename, "png"), ImageFormat.Png);
                }
            }
        }

        public XMLElemental Save () {
            XMLElemental parsed = new XMLElemental ("tileset");
            parsed.Attributes.Add ("name", this.Name);

            // create texture
            Dictionary<int, Basic.Point> texturecoords = new Dictionary<int, Basic.Point> ( );
            // get best image width with base 2 size
            int imagewidthtile = (int)(Math.Sqrt (this.Tiles.Count)) + 1;
            int imagewidthpixelnopower = imagewidthtile * Tile.TILE_SIZE;
            int imagewidthpower = (int)Math.Log (imagewidthpixelnopower, 2) + 1;
            int imagewidth = (int)Math.Pow (2, imagewidthpower);
            if (imagewidthpower > 12)
                throw new ArithmeticException ("too many tiles in tileset, may some devices cant handle the amount of data produced");
            this.texture = new Bitmap (imagewidth, imagewidth);

            using (Graphics g = Graphics.FromImage (this.texture)) {
                for (int y = 0; y <= (int)(this.Tiles.Count / imagewidthtile) + 1; y++) {
                    for (int x = 0; x < Math.Min (imagewidthtile, this.Tiles.Count - y * imagewidthtile); x++) {
                        texturecoords.Add (y * imagewidthtile + x, new Basic.Point (x * Tile.TILE_SIZE, y * Tile.TILE_SIZE));
                        g.DrawImage (this.Tiles[y * imagewidthtile + x].Texture, x * Tile.TILE_SIZE, y * Tile.TILE_SIZE, Tile.TILE_SIZE, Tile.TILE_SIZE);
                    }
                }
            }

            // load image into xml-document
            using (MemoryStream texturestream = new MemoryStream ( )) {
                texture.Save (texturestream, ImageFormat.Png);
                texturestream.Close ( );

                parsed.Attributes.Add ("texture", Convert.ToBase64String (texturestream.ToArray ( )));
            }

            // save tiles
            for (int i = 0; i < this.Tiles.Count; i++) {
                parsed.AddChild (new XMLElemental ("tile"));
                parsed.Get (i).Attributes.Add ("name", this.Tiles[i].Name);
                parsed.Get (i).Attributes.Add ("x", texturecoords[i].X.ToString ( ));
                parsed.Get (i).Attributes.Add ("y", texturecoords[i].Y.ToString ( ));
                parsed.Get (i).Attributes.Add ("maskflag", string.Join (",", this.Tiles[i].MaskFlag));
                parsed.Get (i).Attributes.Add ("attributes", string.Join (" ", this.Tiles[i].Attributes.Select (str => str.Key + Encoding.UTF8.GetString (new byte[ ] { 255 }) + str.Value)));
            }

            return parsed;
        }

        public class Tile {
            public const int TILE_SIZE = 20;

            public string Name;
            public string[ ] MaskFlag;
            public Bitmap Texture;
            public Dictionary<string, string> Attributes;

            public Tile () {
                Attributes = new Dictionary<string, string> ( );
            }

            public Tile (string name, Bitmap texture) {
                Attributes = new Dictionary<string, string> ( );
                Name = name;
                Texture = texture;
                MaskFlag = new string[ ] { "COLLISION" };
            }

            public Tile (XMLElemental config, Bitmap texture) {
                Name = config.Attributes["name"];
                MaskFlag = config.Attributes["maskflag"].Split (new char[ ] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                Texture = new Bitmap (TILE_SIZE, TILE_SIZE);
                using (Graphics g = Graphics.FromImage (Texture)) {
                    g.DrawImage (texture, new System.Drawing.Rectangle (0, 0, TILE_SIZE, TILE_SIZE), float.Parse (config.Attributes["x"]), float.Parse (config.Attributes["y"]), (float)TILE_SIZE, (float)TILE_SIZE, GraphicsUnit.Pixel);
                }
                if (config.Attributes["attributes"].Length > 0) {
                    Attributes = config.Attributes["attributes"].Split (',')
                        .Select (str => str.Split (Encoding.UTF8.GetString (new byte[ ] { 255 }).ToCharArray ( )[0])) // make set from string array
                        .ToDictionary (str => str[0], str => str[1]); // parse set to dictionary
                } else {
                    Attributes = new Dictionary<string, string> ( );
                }
            }
        }
    }
}
