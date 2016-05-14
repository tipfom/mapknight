using System.Collections.Generic;
using System.IO;
using mapKnight.Android.CGL;
using mapKnight.Basic;
using Newtonsoft.Json;

namespace mapKnight.Android.Map {

    public class TileManager {
        public const float TILE_SIZE = 20;

        public CGLTexture2D Texture;
        private Tile[ ] tiles;

        public TileManager (string name) {
            ExportData exportedData = new ExportData ( );
            JsonConvert.PopulateObject (Assets.Load<string> ("tilesets", $"{name}.tileset"), exportedData);
            Texture = Assets.Load<CGLTexture2D> (Path.Combine ("tilesets", exportedData.Texture));


            Vector2 tvertexsize = new Vector2 (TILE_SIZE / Texture.Width, TILE_SIZE / Texture.Height); // size of each texturevertex
            tiles = new Tile[exportedData.Tiles.Count];
            foreach (var exportedTile in exportedData.Tiles) {
                this.tiles[exportedTile.Key] = new Tile (exportedTile.Value, tvertexsize, Texture.Width, Texture.Height);
            }

            Log.Print (this, $"loaded new tileset (name:{name})");
        }

        public Tile GetTile (int id) {
            return tiles[id];
        }

        public float[ ] GetTexture (int id) {
            return tiles[id].Texture;
        }

        // structs to convert .json into memory
        public struct ExportTile {
            public int X;
            public int Y;
            public string Name;
            public string[ ] MaskFlag;
            public Dictionary<Tile.TileAttribute, string> Attributes;
        }

        public class ExportData {
            public string Name;
            public string Texture;
            public Dictionary<int, ExportTile> Tiles;
        }
    }
}