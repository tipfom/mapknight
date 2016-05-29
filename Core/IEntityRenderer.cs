using mapKnight.Graphics;
using System.Collections.Generic;

namespace mapKnight.Core {
    public interface IEntityRenderer {
        void QueueVertexData (int entity, List<VertexData> vertexData);
        void AddTexture (int entity, SpriteBatch entityTexture);
    }
}