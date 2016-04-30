using System;

namespace mapKnight.Basic {
    public class ChangingProperty {
        public event EventHandler<float> Changed;

        private int _Current;
        public int Current { get { return _Current; } set { _Current = value; Changed?.Invoke (this, Percent); } }

        public int Max { get; private set; }

        public float Percent { get { return (float)Current / (float)Max; } }

        public ChangingProperty (int maxValue) {
            Max = maxValue;
        }
    }
}
