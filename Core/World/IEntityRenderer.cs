using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Particles;
using System.Collections.Generic;

namespace mapKnight.Core.World {
    public interface IEntityRenderer {
        void QueueVertexData(int species, IEnumerable<VertexData> vertexData);

        void AddTexture(int species, Spritebatch2D entityTexture);
        bool HasTexture(int species);

        void AddParticles (Emitter emitter);
        void RemoveParticles (Emitter emitter);
    }
}
