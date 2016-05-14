using mapKnight.Android.CGL;
using mapKnight.Basic;

namespace mapKnight.Android.ECS {
    public interface IEntityContainer {
        Vector2 Gravity { get; }
        Vector2 Bounds { get; }
        bool HasCollider (int x, int y);
        int CreateID ();
        IEntityRenderer Renderer { get; }
        void Add (Entity entity);
        float VertexSize { get; }
        bool IsOnScreen (Entity entity);
        CGLCamera Camera { get; }
    }
}