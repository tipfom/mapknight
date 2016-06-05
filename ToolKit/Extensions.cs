using mapKnight.Core;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight.ToolKit {
    public static class Extensions {
        public static void AddTile(this Map map, Tile tile) {
            List<Tile> tiles = map.Tiles.ToList( );
            tiles.Add(tile);
            map.Tiles = tiles.ToArray( );
        }
    }
}
