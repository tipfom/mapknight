using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using mapKnight.Basic;

namespace mapKnight.ToolKit {
    public class Tileset {
        public int RefID {
            get { return Name.GetHashCode (); }
        }

        public readonly string Name;
        public List<Tile> Tiles;

        private Bitmap texture;

        public Tileset (string name) {
            Name = name;
        }

        public Tileset (XMLElemental config) {
            Name = config.Name;
            using (MemoryStream mstream = new MemoryStream (Convert.FromBase64String (config.Attributes["texture"]))) {
                texture = new Bitmap (mstream);
            }
            foreach (XMLElemental tile in config.GetAll ()) {

            }
        }

        public override string ToString () {
            return Name + "( '" + RefID.ToString () + "' )";
        }

        public Image[] GetBitmapsOrdered () {
            Image[] bitmaps = new Image[Tiles.Count];

            for (int i = 0; i < Tiles.Count; i++) {
                bitmaps[i] = (Image)Tiles[i].Texture;
            }

            return bitmaps;
        }

        public class Tile {
            public const int TILE_SIZE = 20;

            public string Name;
            public string[] MaskFlag;
            public Bitmap Texture;
            public Dictionary<string, string> Attributes;

            public Tile () {
                Attributes = new Dictionary<string, string> ();
            }

            public Tile (XMLElemental config, Bitmap texture) {
                Name = config.Name;
                MaskFlag = config.Attributes["maskflag"].Split (new char[] { ';' });
                Texture = new Bitmap (TILE_SIZE, TILE_SIZE);
                using (Graphics g = Graphics.FromImage (Texture)) {
                    g.DrawImage (texture, new System.Drawing.Rectangle (0, 0, TILE_SIZE, TILE_SIZE), float.Parse (config.Attributes["x"]), float.Parse (config.Attributes["y"]), (float)TILE_SIZE, (float)TILE_SIZE, GraphicsUnit.Pixel);
                }
                Attributes = config.Attributes["attributes"].Split (';')
                    .Select (str => str.Split (':')) // make set from string array
                    .ToDictionary (str => str[0], str => str[1]); // parse set to dictionary
            }
        }
    }
}
