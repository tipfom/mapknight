using mapKnight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit.Data {
    public class TileBrush {
        public TileBrushStrokeCollection Centre;
        public TileBrushStrokeCollection CTR, CTL, CBR, CBL;
        public TileBrushStrokeCollection IT, IB, IR, IL;
        public TileBrushStrokeCollection LTR, LTL, LBR, LBL;

        public BitmapImage PrevCentre { get { return Centre[0].Preview; } }
        public BitmapImage PrevCTR { get { return CTR[0].Preview; } }
        public BitmapImage PrevCTL { get { return CTL[0].Preview; } }
        public BitmapImage PrevCBR { get { return CBR[0].Preview; } }
        public BitmapImage PrevCBL { get { return CBL[0].Preview; } }
        public BitmapImage PrevIT { get { return IT[0].Preview; } }
        public BitmapImage PrevIB { get { return IB[0].Preview; } }
        public BitmapImage PrevIR { get { return IR[0].Preview; } }
        public BitmapImage PrevIL { get { return IL[0].Preview; } }
        public BitmapImage PrevLTR { get { return LTR[0].Preview; } }
        public BitmapImage PrevLTL { get { return LTL[0].Preview; } }
        public BitmapImage PrevLBR { get { return LBR[0].Preview; } }
        public BitmapImage PrevLBL { get { return LBL[0].Preview; } }

        private Random random = new Random( );

        public TileBrush ( ) {
            Centre = new TileBrushStrokeCollection( );
            CTR = new TileBrushStrokeCollection( );
            CTL = new TileBrushStrokeCollection( );
            CBR = new TileBrushStrokeCollection( );
            CBL = new TileBrushStrokeCollection( );
            IT = new TileBrushStrokeCollection( );
            IB = new TileBrushStrokeCollection( );
            IR = new TileBrushStrokeCollection( );
            IL = new TileBrushStrokeCollection( );
            LTR = new TileBrushStrokeCollection( );
            LTL = new TileBrushStrokeCollection( );
            LTL = new TileBrushStrokeCollection( );
            LBR = new TileBrushStrokeCollection( );
            LBL = new TileBrushStrokeCollection( );
        }

        public bool Contains (string tileName) {
            foreach (TileBrushStroke brushStroke in All( )) {
                if (brushStroke.Tile.Name == tileName) return true;
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

        public void GeneratePreviewImages (EditorMap map) {
            foreach (TileBrushStroke stroke in All( )) {
                stroke.GeneratePreviewImage(map);
            }
        }

        private (Tile tile, float rotation) GetRandom (TileBrushStrokeCollection array) {
            float summedPossibility = array.SummedPossibility;
            float currentPossibility = 0;
            float targetPossibility = (float)random.NextDouble( );
            int currentIndex = 0;

            while (currentPossibility < targetPossibility && currentIndex < array.Length) {
                currentPossibility += array[currentIndex].Possibility / summedPossibility;
                currentIndex++;
            }

            return (array[currentIndex - 1].Tile, array[currentIndex - 1].Rotation);
        }

        private IEnumerable<TileBrushStroke> All ( ) {
            foreach (TileBrushStroke item in Centre) yield return item;
            foreach (TileBrushStroke item in CTR) yield return item;
            foreach (TileBrushStroke item in CTL) yield return item;
            foreach (TileBrushStroke item in CBR) yield return item;
            foreach (TileBrushStroke item in CBL) yield return item;
            foreach (TileBrushStroke item in IT) yield return item;
            foreach (TileBrushStroke item in IB) yield return item;
            foreach (TileBrushStroke item in IL) yield return item;
            foreach (TileBrushStroke item in IR) yield return item;
            foreach (TileBrushStroke item in LTR) yield return item;
            foreach (TileBrushStroke item in LBR) yield return item;
            foreach (TileBrushStroke item in LTL) yield return item;
            foreach (TileBrushStroke item in LBL) yield return item;
        }
    }
}
