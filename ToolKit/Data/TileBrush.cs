using mapKnight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit.Data {
    public class TileBrush {
        public (Tile tile, float rotation, int possibility)[ ] Centre;
        public (Tile tile, float rotation, int possibility)[ ] CTR, CTL, CBR, CBL;
        public (Tile tile, float rotation, int possibility)[ ] IT, IB, IR, IL;
        public (Tile tile, float rotation, int possibility)[ ] LTR, LTL, LBR, LBL;

        public BitmapImage PrevCentre { get; set; }
        public BitmapImage PrevCTR { get; set; }
        public BitmapImage PrevCTL { get; set; }
        public BitmapImage PrevCBR { get; set; }
        public BitmapImage PrevCBL { get; set; }
        public BitmapImage PrevIT { get; set; }
        public BitmapImage PrevIB { get; set; }
        public BitmapImage PrevIR { get; set; }
        public BitmapImage PrevIL { get; set; }
        public BitmapImage PrevLTR { get; set; }
        public BitmapImage PrevLTL { get; set; }
        public BitmapImage PrevLBR { get; set; }
        public BitmapImage PrevLBL { get; set; }

        private Random random = new Random( );

        public TileBrush ( ) {
            Centre = new(Tile tile, float rotation, int possibility)[0];
            CTR = new(Tile tile, float rotation, int possibility)[0];
            CTL = new(Tile tile, float rotation, int possibility)[0];
            CBR = new(Tile tile, float rotation, int possibility)[0];
            CBL = new(Tile tile, float rotation, int possibility)[0];
            IT = new(Tile tile, float rotation, int possibility)[0];
            IB = new(Tile tile, float rotation, int possibility)[0];
            IR = new(Tile tile, float rotation, int possibility)[0];
            IL = new(Tile tile, float rotation, int possibility)[0];
            LTR = new(Tile tile, float rotation, int possibility)[0];
            LTL = new(Tile tile, float rotation, int possibility)[0];
            LTL = new(Tile tile, float rotation, int possibility)[0];
            LBR = new(Tile tile, float rotation, int possibility)[0];
            LBL = new(Tile tile, float rotation, int possibility)[0];
        }

        public bool Contains (string tileName) {
            foreach (Tile tile in All( )) {
                if (tile.Name == tileName) return true;
            }
            return false;
        }

        public bool IsValid (EditorMap map) {
            return
                Centre.Length > 0 &&
                CTR.Length > 0 &&
                CTL.Length > 0 &&
                CBR.Length > 0 &&
                CBL.Length > 0 &&
                IT.Length > 0 &&
                IB.Length > 0 &&
                IR.Length > 0 &&
                IL.Length > 0 &&
                LTR.Length > 0 &&
                LTL.Length > 0 &&
                LBR.Length > 0 &&
                LBL.Length > 0;
        }

        public (Tile tile, float rotation) Get (params bool[ ] data) {
            /* 0 1 2
             * 3 - 4 
             * 5 6 7
             */

            if (data[3] && data[6] && !data[4] && !data[1]) return GetRandom(CTR);
            if (data[4] && data[6] && !data[3] && !data[1]) return GetRandom(CTL);
            if (data[3] && data[1] && !data[4] && !data[6]) return GetRandom(CBR);
            if (data[4] && data[1] && !data[3] && !data[6]) return GetRandom(CBL);

            if (!data[1] && data[4] && data[6] && data[3]) return GetRandom(IT);
            if (data[1] && data[4] && !data[6] && data[3]) return GetRandom(IB);
            if (data[1] && !data[4] && data[6] && data[3]) return GetRandom(IR);
            if (data[1] && data[4] && data[6] && !data[3]) return GetRandom(IL);

            if (data[1] && data[4] && data[6] && data[3]) {
                if (!data[2]) return GetRandom(LTR);
                if (!data[0]) return GetRandom(LTL);
                if (!data[7]) return GetRandom(LBR);
                if (!data[5]) return GetRandom(LBL);
            }

            return GetRandom(Centre);
        }

        public void GeneratePreviewImages(EditorMap map) {
            PrevCentre = map.WpfTextures[Centre[0].tile.Name];
            PrevCTR = map.WpfTextures[CTR[0].tile.Name];
            PrevCTL = map.WpfTextures[CTL[0].tile.Name];
            PrevCBR = map.WpfTextures[CBR[0].tile.Name];
            PrevCBL = map.WpfTextures[CBL[0].tile.Name];
            PrevIT = map.WpfTextures[IT[0].tile.Name];
            PrevIB = map.WpfTextures[IB[0].tile.Name];
            PrevIR = map.WpfTextures[IR[0].tile.Name];
            PrevIL = map.WpfTextures[IL[0].tile.Name];
            PrevLTR = map.WpfTextures[LTR[0].tile.Name];
            PrevLTL = map.WpfTextures[LTL[0].tile.Name];
            PrevLBR = map.WpfTextures[LBR[0].tile.Name];
            PrevLBL = map.WpfTextures[LBL[0].tile.Name];
        }

        private (Tile tile, float rotation) GetRandom ((Tile tile, float rotation, int possibility)[ ] array) {
            float summedPossibility = array.Sum(item => item.possibility);
            float currentPossibility = 0;
            float targetPossibility = (float)random.NextDouble( );
            int currentIndex = 0;

            while (currentPossibility < targetPossibility && currentIndex < array.Length) {
                currentPossibility += array[currentIndex].possibility / summedPossibility;
                currentIndex++;
            }

            return (array[currentIndex - 1].tile, array[currentIndex - 1].rotation);
        }

        private IEnumerable<Tile> All ( ) {
            foreach ((Tile tile, float rotation, int possibility) item in Centre) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in CTR) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in CTL) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in CBR) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in CBL) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in IT) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in IB) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in IL) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in IR) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in LTR) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in LBR) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in LTL) yield return item.tile;
            foreach ((Tile tile, float rotation, int possibility) item in LBL) yield return item.tile;
        }
    }
}
