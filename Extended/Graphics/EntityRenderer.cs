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
            buffer = new BufferBatch(new IndexBuffer(MAX_QUAD_COUNT), new ClientBuffer(3, MAX_QUAD_COUNT), new ClientBuffer(4, MAX_QUAD_COUNT), new ClientBuffer(2, MAX_QUAD_COUNT));
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
                    float[ ] verticies = {
                        vertexData.Verticies[0], vertexData.Verticies[1], vertexData.Depth,
                        vertexData.Verticies[2], vertexData.Verticies[3], vertexData.Depth,
                        vertexData.Verticies[4], vertexData.Verticies[5], vertexData.Depth,
                        vertexData.Verticies[6], vertexData.Verticies[7], vertexData.Depth,
                    };
                    Array.Copy(verticies, 0, vertexBuffer.Data, currentIndex * 12, 12);
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
