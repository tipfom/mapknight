using System;

namespace mapKnight.Android.Map {
    [Flags]
    public enum TileMask {
        None = 0,
        Collision = 1
    }
}