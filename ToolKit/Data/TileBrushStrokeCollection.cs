using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight.ToolKit.Data {
    public class TileBrushStrokeCollection : ICollection<TileBrushStroke> {
        public List<TileBrushStroke> Strokes;
        public float SummedPossibility { get { return Strokes.Sum(stroke => stroke.Possibility); } }

        public int Count => Strokes.Count;
        public bool IsReadOnly => false;

        public TileBrushStroke this[int index] {
            get { return Strokes[index]; }
            set { Strokes[index] = value; }
        }

        public TileBrushStrokeCollection ( ) {
            Strokes = new List<TileBrushStroke>( );
        }

        public void Add (TileBrushStroke item) {
            Strokes.Add(item);
        }

        public void Clear ( ) {
            Strokes.Clear( );
        }

        public bool Contains (TileBrushStroke item) {
            return Strokes.Contains(item);
        }

        public void CopyTo (TileBrushStroke[ ] array, int arrayIndex) {
            Strokes.CopyTo(array, arrayIndex);
        }

        public bool Remove (TileBrushStroke item) {
            return Strokes.Remove(item);
        }

        public IEnumerator<TileBrushStroke> GetEnumerator ( ) {
            return Strokes.GetEnumerator( );
        }

        IEnumerator IEnumerable.GetEnumerator ( ) {
            return Strokes.GetEnumerator( );
        }
    }
}
