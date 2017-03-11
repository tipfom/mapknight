using mapKnight.Extended.Graphics;
using System.Collections.Generic;

namespace mapKnight.Extended {
    public interface IEntityRenderer {
        void QueueVertexData (int entity, IEnumerable<VertexData> vertexData);
        void AddTexture (int entity, SpriteBatch entityTexture);
        SpriteBatch GetTexture (int entity);
        bool HasTexture (int entity);
    }
}