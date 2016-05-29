using mapKnight.Core;
using System;
using System.Collections.Generic;

namespace mapKnight.Core {
    public interface IEntityContainer {
        IEntityRenderer Renderer { get; }

        Vector2 Gravity { get; }
        Vector2 Bounds { get; }
        Vector2 GetPositionOnScreen (Entity entity);

        List<Entity> GetEntities ();
        List<Entity> GetEntities (Predicate<Entity> predicate);

        bool HasCollider (int x, int y);
        bool IsOnScreen (Entity entity);

        int CreateID ();

        float VertexSize { get; }

        void Add (Entity entity);
    }
}