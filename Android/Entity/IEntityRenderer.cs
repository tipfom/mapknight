using mapKnight.Android.CGL;

namespace mapKnight.Android.Entity {
    public interface IEntityRenderer {
        void QueueVertexData (EntityVertexData vertexData);
        void AddTexture (int entityID, CGLSprite2D entityTexture);
    }
}