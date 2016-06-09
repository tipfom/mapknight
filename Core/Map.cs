using mapKnight.Core.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace mapKnight.Core {
    public class Map {
        public const int TILE_PXL_SIZE = 18;

        // x,y,layer
        public int[ , , ] Data;

        public Size Size { get; private set; }
        public int Height { get { return Size.Height; } }
        public int Width { get { return Size.Width; } }

        public Vector2 SpawnPoint;
        public Tile[ ] Tiles = new Tile[0];
        public string Texture;
        public string Creator;
        public string Name;

        public Map (Size size, string creator, string name) {
            Size = size;
            Data = new int[size.Width, size.Height, 3];
            Creator = creator;
            Name = name;
            SpawnPoint = new Vector2( );
        }

        public Map (Size size) : this(size, null, null) {

        }
        
        public static Map FromStream(Stream input) {
            return MapSerizalizer.Deserialize(input);
        }

        public void Serialize (Stream stream) {
            MapSerizalizer.Serialize(stream, this);
        }
        
        public Tile GetTile (int x, int y, int layer) {
            return Tiles[Data[x, y, layer]];
        }

        public Tile GetTileBackground (int x, int y) {
            return Tiles[Data[x, y, 0]];
        }

        public Tile GetTile (int x, int y) {
            return Tiles[Data[x, y, 1]];
        }

        public Tile GetTileForeground (int x, int y) {
            return Tiles[Data[x, y, 2]];
        }

        public override string ToString ( ) {
            return $"{Name} ({Size.Width}x{Size.Height})";
        }
    }
}