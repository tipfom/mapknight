using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Android.CGL;
using mapKnight.Basic;

namespace mapKnight.Android {

    public class TileManager {
        public const float TILE_SIZE = 20;

        public CGLTexture2D Texture;
        private Tile[] tiles;

        public TileManager (XMLElemental config) {
            tiles = new Tile[config.ChildCount];

            Texture = Assets.LoadTexture (Convert.FromBase64String (config.Attributes["texture"]));

            fSize tvertexsize = new fSize (TILE_SIZE / Texture.Width, TILE_SIZE / Texture.Height); // size of each texturevertex

            for (int i = 0; i < config.ChildCount; i++) {
                tiles[i] = new Tile (config.Get (i), tvertexsize, Texture.Width, Texture.Height);
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
                    x, y + tvertexsize.Height,
                    x, y,
                    x + tvertexsize.Width, y,
                    x + tvertexsize.Width, y + tvertexsize.Height
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