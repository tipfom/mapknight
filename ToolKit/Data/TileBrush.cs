using mapKnight.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight.ToolKit.Data {
    public class TileBrush {
        public (Tile tile, float rotation) Centre;
        public (Tile tile, float rotation) CTR, CTL, CBR, CBL;
        public (Tile tile, float rotation) IT, IB, IR, IL;
        public (Tile tile, float rotation) LTR, LTL, LBR, LBL;

        public TileBrush ( ) {
        }

        public bool Contains(string tileName) {
            foreach(Tile tile in All()) {
                if (tile.Name == tileName) return true;
            }
            return false;
        }

        public bool IsValid(EditorMap map) {
            foreach (Tile tile in All( )) {
                if (tile.Equals(default(Tile)) || !map.Tiles.Any(t => t.Name == tile.Name)) {
                    return false;
                }
            }
            return true;
        }
 
        public (Tile tile, float rotation) Get(params bool[] data) {
            /* 0 1 2
             * 3 - 4 
             * 5 6 7
             */
             
            if (data[3] && data[6] && !data[4] && !data[1]) return CTR;
            if (data[4] && data[6] && !data[3] && !data[1]) return CTL;
            if (data[3] && data[1] && !data[4] && !data[6]) return CBR;
            if (data[4] && data[1] && !data[3] && !data[6]) return CBL;

            if (!data[1] && data[4] && data[6] && data[3]) return IT;
            if (data[1] && data[4] && !data[6] && data[3]) return IB;
            if (data[1] && !data[4] && data[6] && data[3]) return IR;
            if (data[1] && data[4] && data[6] && !data[3]) return IL;

            if (data[1] && data[4] && data[6] && data[3]) {
                if (!data[2]) return LTR;
                if (!data[0]) return LTL;
                if (!data[7]) return LBR;
                if (!data[5]) return LBL;
            }

            return Centre;
        }

        private IEnumerable<Tile> All ( ) {
            yield return Centre.tile;
            yield return CTR.tile;
            yield return CTL.tile;
            yield return CBR.tile;
            yield return CBL.tile;
            yield return IT.tile;
            yield return IB.tile;
            yield return IR.tile;
            yield return IL.tile;
            yield return LTR.tile;
            yield return LTL.tile;
            yield return LBR.tile;
            yield return LBL.tile;
        }
    }
}
