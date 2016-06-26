using System;
using System.Collections.Generic;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;

namespace mapKnight.Extended.Graphics.GUI {
    public static class GUIRenderer {
        const int MAX_QUADS = 500;

        private static float[ ] nullArray = new float[ ] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

        private static GUICollection _Target;
        public static GUICollection Target { get { return _Target; } set { _Target = value; Prepare( ); } }

        private static Dictionary<GUIItem, Stack<int>> usedIndicies;
        private static Stack<int> freeIndicies;
        public static SpriteBatch Texture;
        private static BufferBatch buffer;
        private static CachedGPUBuffer vertexBuffer { get { return (CachedGPUBuffer)buffer.VertexBuffer; } }
        private static CachedGPUBuffer textureBuffer { get { return (CachedGPUBuffer)buffer.TextureBuffer; } }
        private static CachedGPUBuffer colorBuffer { get { return (CachedGPUBuffer)buffer.ColorBuffer; } }

        static GUIRenderer ( ) {
            buffer = new BufferBatch(new IndexBuffer(MAX_QUADS), new CachedGPUBuffer(2, MAX_QUADS), new CachedGPUBuffer(4, MAX_QUADS), new CachedGPUBuffer(2, MAX_QUADS));
            usedIndicies = new Dictionary<GUIItem, Stack<int>>( );
            freeIndicies = new Stack<int>( );
            for (int i = MAX_QUADS - 1; i >= 0; i--) {
                freeIndicies.Push(i);
            }
        }

        private static void Prepare ( ) {
            usedIndicies = new Dictionary<GUIItem, Stack<int>>( );
            freeIndicies = new Stack<int>( );
            for (int i = MAX_QUADS - 1; i >= 0; i--) {
                freeIndicies.Push(i);
            }
            ((CachedGPUBuffer)buffer.VertexBuffer).Cache = new float[buffer.VertexBuffer.Length];
            ((CachedGPUBuffer)buffer.VertexBuffer).Put( );
            foreach (GUIItem item in Target) {
                usedIndicies.Add(item, new Stack<int>( ));
                Update(item);
            }
        }

        public static void Update (GUIItem item) {
            while (usedIndicies[item].Count > 0) {
                int index = usedIndicies[item].Pop( );
                freeIndicies.Push(index);
                Array.Copy(nullArray, 0, vertexBuffer.Cache, 0, 8);
            }
            if (!item.Visible)
                return;
            foreach (VertexData vertexData in item.GetVertexData( )) {
                int index = freeIndicies.Pop( );

                float[ ] verticies = {
                        vertexData.Verticies[0], vertexData.Verticies[1],
                        vertexData.Verticies[2], vertexData.Verticies[3],
                        vertexData.Verticies[4], vertexData.Verticies[5],
                        vertexData.Verticies[6], vertexData.Verticies[7],
                    };
                Array.Copy(verticies, 0, vertexBuffer.Cache, index * 8, 8);
                Array.Copy(vertexData.Color.ToOpenGL( ), 0, colorBuffer.Cache, index * 16, 16);
                Array.Copy(Texture.Get(vertexData.Texture), 0, textureBuffer.Cache, index * 8, 8);

                usedIndicies[item].Push(index);
            }

            vertexBuffer.Put( );
            colorBuffer.Put( );
            textureBuffer.Put( );
        }

        public static void Draw ( ) {
            vertexBuffer.Put( );
            colorBuffer.Put( );
            textureBuffer.Put( );

            Program.Begin( );
            Program.Draw(buffer, Texture, Matrix.Default, true);
            Program.End( );
        }
    }
}
