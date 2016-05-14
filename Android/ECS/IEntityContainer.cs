using mapKnight.Basic;

namespace mapKnight.Android.ECS {
    public interface IEntityContainer {
        IEntityRenderer Renderer { get; }

        Vector2 Gravity { get; }
        Vector2 Bounds { get; }
        Vector2 GetPositionOnScreen (Entity entity);

        bool HasCollider (int x, int y);
        bool IsOnScreen (Entity entity);

        int CreateID ();

        float VertexSize { get; }

        void Add (Entity entity);
    }
}