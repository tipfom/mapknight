using mapKnight.Extended.Graphics;
using System.Collections.Generic;

namespace mapKnight.Extended {
    public interface IEntityRenderer {
        void QueueVertexData (int entity, List<VertexData> vertexData);
        void AddTexture (int entity, SpriteBatch entityTexture);
    }
}