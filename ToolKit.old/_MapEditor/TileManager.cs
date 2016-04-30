using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using mapKnight.Basic;

namespace mapKnight.Android {

    public class TileManager {
        public const float TILE_SIZE = 20;

        public Bitmap Texture;
        private Tile[] tiles;
        private Dictionary<int, int> loadIDs;

        public TileManager (XMLElemental config) {
            int length = config.ChildCount - 1;
            tiles = new Tile[length];
            loadIDs = new Dictionary<int, int> (length);

            Texture = new Bitmap (config.Attributes["texture"]);

            for (int i = 0; i < config.ChildCount; i++) {
                tiles[i] = new Tile (config.Get (i), Texture.Width, Texture.Height);
                loadIDs.Add (int.Parse (config.Get (i).Attributes["loadid"]), i);
            }
        }

        public Tile GetTile (int id) {
            return tiles[id];
        }

        public int GetID (int loadid) {
            // gets the index of the tile in the array
            return loadIDs[loadid];
        }
    }

    public struct Tile {
        public string Name;
        public System.Drawing.Rectangle TextureRectangle;
        public TileMask Mask;
        public Dictionary<TileAttribute, string> Attributes;

        public Tile (XMLElemental config, int imagewidth, int imageheight) : this () {
            this.Name = config.Name; // name is name of xml-element
            this.Mask = (TileMask)Enum.Parse (typeof (TileMask), config.Attributes["maskflag"]);
            this.Attributes = config.Attributes["attributes"].Split (';')
                .Select (str => str.Split (':')) // make set from string array
                .ToDictionary (str => (TileAttribute)Enum.Parse (typeof (TileAttribute), str[0]), str => str[1]); // parse set to dictionary

            int x = Convert.ToInt32 (config.Attributes["x"]);
            int y = Convert.ToInt32 (config.Attributes["y"]);

            this.TextureRectangle = new System.Drawing.Rectangle (x, y, imagewidth, imageheight);
        }

        [Flags]
        public enum TileMask {

        }

        public enum TileAttribute {

        }
    }
}