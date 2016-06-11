using mapKnight.Core;
using System;
using System.Collections.Generic;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;

namespace mapKnight.Extended.Graphics {
    public class EntityRenderer : IEntityRenderer {
        const int MAX_QUAD_COUNT = 400;

        private Dictionary<int, SpriteBatch> entityTextures = new Dictionary<int, SpriteBatch>( );
        private Dictionary<int, Queue<VertexData>> frameVertexData = new Dictionary<int, Queue<VertexData>>( );

        private ColorBufferBatch buffer;

        public EntityRenderer ( ) {
            buffer = new ColorBufferBatch(MAX_QUAD_COUNT, 3);
        }

        public void AddTexture (int entity, SpriteBatch entityTexture) {
            if (!entityTextures.ContainsKey(entity)) {
                entityTextures.Add(entity, entityTexture);
                frameVertexData.Add(entity, new Queue<VertexData>( ));
            }
        }

        public void QueueVertexData (int entity, List<VertexData> vertexData) {
            frameVertexData[entity].Enqueue(vertexData);
        }

        public void Update (float dt) {

        }

        public void Draw ( ) {
            Program.Begin( );
            foreach (int entity in frameVertexData.Keys) {
                int currentIndex = 0;
                while (frameVertexData[entity].Count > 0) {
                    VertexData vertexData = frameVertexData[entity].Dequeue( );
                    float[ ] verticies = {
                        vertexData.Verticies[0], vertexData.Verticies[1], vertexData.Depth,
                        vertexData.Verticies[2], vertexData.Verticies[3], vertexData.Depth,
                        vertexData.Verticies[4], vertexData.Verticies[5], vertexData.Depth,
                        vertexData.Verticies[6], vertexData.Verticies[7], vertexData.Depth,
                    };
                    Array.Copy(verticies, 0, buffer.Verticies, currentIndex * 12, 12);
                    Array.Copy(vertexData.Color.ToOpenGL( ), 0, buffer.Color, currentIndex * 16, 16);
                    Array.Copy(entityTextures[entity].Get(vertexData.Texture), 0, buffer.Texture, currentIndex * 8, 8);
                    currentIndex++;
                }
                //Program.Draw(buffer, entityTextures[entity].ID, Matrix.Default.MVP, currentIndex * 6, true);
            }
            Program.End( );
        }
    }
}
