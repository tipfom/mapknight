using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Basic;

namespace mapKnight.Android {

    public class TileManager {
        public const float TILE_SIZE = 20;

        public int Texture;
        private Tile[] tiles;

        public TileManager (XMLElemental config) {
            tiles = new Tile[config.ChildCount];

            CGL.CGLTools.LoadedImage image = CGL.CGLTools.LoadImage (Convert.FromBase64String (config.Attributes["texture"]));
            Texture = image.Texture;
            fSize tvertexsize = new fSize (TILE_SIZE / image.Width, TILE_SIZE / image.Height); // size of each texturevertex

            for (int i = 0; i < config.ChildCount; i++) {
                tiles[i] = new Tile (config.Get (i), tvertexsize, image.Width, image.Height);
            }
        }

        public Tile GetTile (int id) {
            return tiles[id];
        }

        public float[] GetTexture (int id) {
            return tiles[id].Texture;
        }
    }

    public struct Tile {
        public string Name;
        public float[] Texture;
        public TileMask Mask;
        public Dictionary<TileAttribute, string> Attributes;

        public Tile (XMLElemental config, fSize tvertexsize, int imagewidth, int imageheight) : this () {
            this.Name = config.Attributes["name"]; // name is name of xml-element
            if (!Enum.TryParse (config.Attributes["maskflag"], out Mask))
                Mask = TileMask.NONE;

            if (config.Attributes["attributes"] != "") {
                this.Attributes = config.Attributes["attributes"].Split (';')
                    .Select (str => str.Split (':')) // make set from string array
                    .ToDictionary (str => (TileAttribute)Enum.Parse (typeof (TileAttribute), str[0]), str => str[1]); // parse set to dictionary
            } else {
                this.Attributes = new Dictionary<TileAttribute, string> ();
            }

            float x = (float)Convert.ToInt32 (config.Attributes["x"]) / imagewidth;
            float y = (float)Convert.ToInt32 (config.Attributes["y"]) / imageheight;

            this.Texture = new float[] {
                    x + tvertexsize.Width, y,
                    x + tvertexsize.Width, y + tvertexsize.Height,
                    x, y + tvertexsize.Height,
                    x, y
                };
        }

        [Flags]
        public enum TileMask {
            NONE = 0,
            COLLISION = 1
        }

        public enum TileAttribute {

        }
    }
}