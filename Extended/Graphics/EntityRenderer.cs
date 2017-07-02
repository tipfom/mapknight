using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;

namespace mapKnight.Extended.Graphics {
    public class EntityRenderer : IEntityRenderer {
        const int MAX_QUAD_COUNT = 400;

        private Dictionary<int, Spritebatch2D> entityTextures = new Dictionary<int, Spritebatch2D>( );
        private Dictionary<Spritebatch2D, Queue<VertexData>> frameVertexData = new Dictionary<Spritebatch2D, Queue<VertexData>>( );

        private BufferBatch buffer;
        private ClientBuffer vertexBuffer { get { return (ClientBuffer)buffer.VertexBuffer; } }
        private ClientBuffer textureBuffer { get { return (ClientBuffer)buffer.TextureBuffer; } }
        private ClientBuffer colorBuffer { get { return (ClientBuffer)buffer.ColorBuffer; } }

        public EntityRenderer ( ) {
            buffer = new BufferBatch(new IndexBuffer(MAX_QUAD_COUNT), new ClientBuffer(2, MAX_QUAD_COUNT, PrimitiveType.Quad), new ClientBuffer(4, MAX_QUAD_COUNT, PrimitiveType.Quad), new ClientBuffer(2, MAX_QUAD_COUNT, PrimitiveType.Quad));
        }

        public void AddTexture (int species, Spritebatch2D entityTexture) {
            entityTextures.Add(species, entityTexture);
            if (!frameVertexData.ContainsKey(entityTexture))
                frameVertexData.Add(entityTexture, new Queue<VertexData>( ));
        }

        public bool HasTexture(int species) {
            return entityTextures.ContainsKey(species);
        }

        public void QueueVertexData (int species, IEnumerable<VertexData> vertexData) {
            frameVertexData[entityTextures[species]].Enqueue(vertexData);
        }

        public void Draw ( ) {
            foreach (Spritebatch2D sprite in frameVertexData.Keys) {
                int currentIndex = 0;
                while (frameVertexData[sprite].Count > 0) {
                    VertexData vertexData = frameVertexData[sprite].Dequeue( );
                    Array.Copy(vertexData.Verticies, 0, vertexBuffer.Data, currentIndex * 8, 8);
                    Array.Copy(vertexData.Color.ToArray4( ), 0, colorBuffer.Data, currentIndex * 16, 16);
                    Array.Copy(sprite[vertexData.Texture], 0, textureBuffer.Data, currentIndex * 8, 8);
                    currentIndex++;
                }
                Program.Begin( );
                Program.Draw(buffer, sprite, Matrix.Default, currentIndex * 6, 0, true);
                Program.End( );
            }
        }
    }
}
