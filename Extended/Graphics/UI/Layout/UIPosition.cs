using System;

namespace mapKnight.Extended.Graphics.UI.Layout {
    [Flags]
    public enum UIPosition : byte {
        Center = 1 << 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Top = 1 << 3,
        Bottom = 1 << 4,
    }
}
