using System;

namespace mapKnight.Core {
    [Flags]
    public enum Anchor {
        Center = 1,
        Top = 2,
        Bottom = 4,
        Left = 8,
        Right = 16
    }
}
