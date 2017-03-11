using System.Collections.Generic;

namespace mapKnight.Core.World {
    public interface IEntityWorld {
        IEntityRenderer Renderer { get; }

        Vector2 Gravity { get; }
        Size Size { get; }
        Vector2 GetPositionOnScreen(Entity entity);
        List<Entity> Entities { get; set; }

        void Add(Entity entity);
        void Destroy(Entity entity);

        bool HasCollider(int x, int y);
        bool IsOnScreen(Entity entity);

        float VertexSize { get; }
    }
}
