using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.Entity {
    public struct Config {
        public string Name;
        public int Weight;
        public fVector2D Bounds;
        public Dictionary<string, Stat> Stats;
    }
}