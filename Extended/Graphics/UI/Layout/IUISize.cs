using System;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public interface IUISize {
        Vector2 Size { get; }
        float X { get; }
        float Y { get; }
        event Action Changed;
    }
}
