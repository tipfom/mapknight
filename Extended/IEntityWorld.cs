using System;
using System.Collections.Generic;
using mapKnight.Core;

namespace mapKnight.Extended {
    public interface IEntityWorld {
        IEntityRenderer Renderer { get; }

        Vector2 Gravity { get; }
        Vector2 Bounds { get; }
        Vector2 GetPositionOnScreen (Entity entity);

        bool HasCollider (int x, int y);
        bool IsOnScreen (Entity entity);

        float VertexSize { get; }
    }
}