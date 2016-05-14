using mapKnight.Android.CGL;

namespace mapKnight.Android.ECS {
    public interface IEntityRenderer {
        void QueueVertexData (EntityVertexData vertexData);
        void AddTexture (int entityID, CGLSprite2D entityTexture);
    }
}