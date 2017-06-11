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
                Centre.Count > 0 &&
                CTR.Count > 0 &&
                CTL.Count > 0 &&
                CBR.Count > 0 &&
                CBL.Count > 0 &&
                IT.Count > 0 &&
                IB.Count > 0 &&
                IR.Count > 0 &&
                IL.Count > 0 &&
                LTR.Count > 0 &&
                LTL.Count > 0 &&
                LBR.Count > 0 &&
                LBL.Count > 0;
        }

        public (Tile tile, float rotation) Get (Tile currentTile, float currentRotation, params bool[ ] data) {
            /* 0 1 2
             * 3 - 4 
             * 5 6 7
             */

            if (data[3] && data[6] && !data[4] && !data[1]) return GetRandom(currentTile, currentRotation, CTR);
            if (data[4] && data[6] && !data[3] && !data[1]) return GetRandom(currentTile, currentRotation, CTL);
            if (data[3] && data[1] && !data[4] && !data[6]) return GetRandom(currentTile, currentRotation, CBR);
            if (data[4] && data[1] && !data[3] && !data[6]) return GetRandom(currentTile, currentRotation, CBL);

            if (!data[1] && data[4] && data[6] && data[3]) return GetRandom(currentTile, currentRotation, IT);
            if (data[1] && data[4] && !data[6] && data[3]) return GetRandom(currentTile, currentRotation, IB);
            if (data[1] && !data[4] && data[6] && data[3]) return GetRandom(currentTile, currentRotation, IR);
            if (data[1] && data[4] && data[6] && !data[3]) return GetRandom(currentTile, currentRotation, IL);

            if (data[1] && data[4] && data[6] && data[3]) {
                if (!data[2]) return GetRandom(currentTile, currentRotation, LTR);
                if (!data[0]) return GetRandom(currentTile, currentRotation, LTL);
                if (!data[7]) return GetRandom(currentTile, currentRotation, LBR);
                if (!data[5]) return GetRandom(currentTile, currentRotation, LBL);
            }

            return GetRandom(currentTile, currentRotation, Centre);
        }

        public void GeneratePreviewImages (EditorMap map) {
            foreach (TileBrushStroke stroke in All( )) {
                stroke.GeneratePreviewImage(map);
            }
        }

        private (Tile tile, float rotation) GetRandom (Tile currentTile, float currentRotation, TileBrushStrokeCollection array) {
            foreach(TileBrushStroke stroke in array) {
                if(stroke.Tile.Name == currentTile.Name) {
                    return (stroke.Tile, stroke.Rotation);
                }
            }

            float summedPossibility = array.SummedPossibility;
            float currentPossibility = 0;
            float targetPossibility = (float)random.NextDouble( );
            int currentIndex = 0;

            while (currentPossibility < targetPossibility && currentIndex < array.Count) {
                currentPossibility += array[currentIndex].Possibility / summedPossibility;
                currentIndex++;
            }

            return (array[currentIndex - 1].Tile, array[currentIndex - 1].Rotation);
        }

        public IEnumerable<TileBrushStroke> All ( ) {
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
