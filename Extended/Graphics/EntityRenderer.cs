using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;

namespace mapKnight.Extended.Graphics {
    public class EntityRenderer : IEntityRenderer {
        const int MAX_QUAD_COUNT = 400;

        private Dictionary<int, SpriteBatch> entityTextures = new Dictionary<int, SpriteBatch>( );
        private Dictionary<int, Queue<VertexData>> frameVertexData = new Dictionary<int, Queue<VertexData>>( );

        private BufferBatch buffer;
        private ClientBuffer vertexBuffer { get { return (ClientBuffer)buffer.VertexBuffer; } }
        private ClientBuffer textureBuffer { get { return (ClientBuffer)buffer.TextureBuffer; } }
        private ClientBuffer colorBuffer { get { return (ClientBuffer)buffer.ColorBuffer; } }

        public EntityRenderer ( ) {
            buffer = new BufferBatch(new IndexBuffer(MAX_QUAD_COUNT), new ClientBuffer(2, MAX_QUAD_COUNT, PrimitiveType.Quad), new ClientBuffer(4, MAX_QUAD_COUNT, PrimitiveType.Quad), new ClientBuffer(2, MAX_QUAD_COUNT, PrimitiveType.Quad));
        }

        public void AddTexture (int species, SpriteBatch entityTexture) {
            if (!entityTextures.ContainsKey(species)) {
                entityTextures.Add(species, entityTexture);
                frameVertexData.Add(species, new Queue<VertexData>( ));
            }
        }

        public void QueueVertexData (int species, List<VertexData> vertexData) {
            frameVertexData[species].Enqueue(vertexData);
        }

        public void Update (float dt) {

        }

        public void Draw ( ) {
            foreach (int species in frameVertexData.Keys) {
                int currentIndex = 0;
                while (frameVertexData[species].Count > 0) {
                    VertexData vertexData = frameVertexData[species].Dequeue( );
                    Array.Copy(vertexData.Verticies, 0, vertexBuffer.Data, currentIndex * 8, 8);
                    Array.Copy(vertexData.Color.ToOpenGL( ), 0, colorBuffer.Data, currentIndex * 16, 16);
                    Array.Copy(entityTextures[species].Get(vertexData.Texture), 0, textureBuffer.Data, currentIndex * 8, 8);
                    currentIndex++;
                }
                Program.Begin( );
                Program.Draw(buffer, entityTextures[species], Matrix.Default, currentIndex * 6, 0, true);
                Program.End( );
            }
        }
    }
}
