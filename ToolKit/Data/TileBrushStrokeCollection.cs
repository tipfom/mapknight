using System.Collections;
using System.Linq;

namespace mapKnight.ToolKit.Data {
    public class TileBrushStrokeCollection : IEnumerable {
        public TileBrushStroke[ ] Strokes;
        public float SummedPossibility { get { return Strokes.Sum(stroke => stroke.Possibility); } }
        public int Length { get { return Strokes.Length; } }

        public TileBrushStroke this[int index] {
            get { return Strokes[index]; }
            set { Strokes[index] = value; }
        }

        public TileBrushStrokeCollection ( ) {
            Strokes = new TileBrushStroke[0];
        }

        IEnumerator IEnumerable.GetEnumerator ( ) {
            return Strokes.GetEnumerator( );
        }
    }
}
