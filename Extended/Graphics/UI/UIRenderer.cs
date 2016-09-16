using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;

namespace mapKnight.Extended.Graphics.UI {

    public static class UIRenderer {
        public static SpriteBatch Texture;
        private const int MAX_QUADS = 400;
        private static BufferBatch[ ] buffer;
        private static bool[ ] bufferUpdated = { false, false, false };
        private static Screen currentScreen;
        private static List<Tuple<UIItem, int>>[ ] indexUsage;
        private static int[ ] renderCount = { 0, 0, 0 };
        private static Dictionary<Screen, List<UIItem>> uiItems;
        private static int[ ] vertexCount = { 0, 0, 0 };
        private static Queue<UIItem> updateQueue = new Queue<UIItem>();

        static UIRenderer ( ) {
            IndexBuffer sharedIndexBuffer = new IndexBuffer(MAX_QUADS);
            buffer = new BufferBatch[ ] {
                new BufferBatch(sharedIndexBuffer, new CachedGPUBuffer(2, MAX_QUADS), new CachedGPUBuffer(4, MAX_QUADS), new CachedGPUBuffer(2, MAX_QUADS)),
                new BufferBatch(sharedIndexBuffer, new CachedGPUBuffer(2, MAX_QUADS), new CachedGPUBuffer(4, MAX_QUADS), new CachedGPUBuffer(2, MAX_QUADS)),
                new BufferBatch(sharedIndexBuffer, new CachedGPUBuffer(2, MAX_QUADS), new CachedGPUBuffer(4, MAX_QUADS), new CachedGPUBuffer(2, MAX_QUADS))
            };

            uiItems = new Dictionary<Screen, List<UIItem>>( );
            indexUsage = new List<Tuple<UIItem, int>>[ ] { new List<Tuple<UIItem, int>>( ), new List<Tuple<UIItem, int>>( ), new List<Tuple<UIItem, int>>( ) };
        }

        public static List<UIItem> Current { get { return uiItems[currentScreen]; } }

        public static void Add (Screen screen, UIItem item) {
            if (!uiItems.ContainsKey(screen))
                uiItems.Add(screen, new List<UIItem>( ));
            uiItems[screen].Add(item);
        }

        public static void Remove (UIItem uiItem) {
            if (!uiItems.ContainsKey(uiItem.Screen)) return;
            uiItems[uiItem.Screen].Remove(uiItem);
            if (uiItem.Screen.IsActive) Update(uiItem);
        }

        public static void Delete ( ) {
            uiItems.Clear( );
        }

        public static void Delete (Screen target) {
            uiItems.Remove(target);
        }

        public static void Draw ( ) {
            Program.Begin( );
            Program.Draw(buffer[0], Texture, Matrix.Default, renderCount[0], 0, true);
            Program.Draw(buffer[1], Texture, Matrix.Default, renderCount[1], 0, true);
            Program.Draw(buffer[2], Texture, Matrix.Default, renderCount[2], 0, true);
            Program.End( );
        }

        public static void Prepare (Screen target) {
            currentScreen = target;
            vertexCount = new int[ ] { 0, 0, 0 };
            renderCount = new int[ ] { 0, 0, 0 };
            indexUsage = new List<Tuple<UIItem, int>>[ ] { new List<Tuple<UIItem, int>>( ), new List<Tuple<UIItem, int>>( ), new List<Tuple<UIItem, int>>( ) };
            updateQueue.Clear( );

            if (uiItems.ContainsKey(target)) {
                foreach (UIItem item in uiItems[target]) {
                    Update(item);
                }
            }
        }

        public static void Update (UIItem item) {
            updateQueue.Enqueue(item);
        }

        private static void UpdateBuffer(UIItem item) {
            for (int depth = 0; depth < 3; depth++) {
                int current = 0;
                for (int i = 0; i < indexUsage[depth].Count; i++) {
                    Tuple<UIItem, int> entry = indexUsage[depth][i];
                    if (entry.Item1 == item) {
                        bufferUpdated[depth] = true;
                        indexUsage[depth].RemoveAt(i);
                        vertexCount[depth] -= entry.Item2;
                        renderCount[depth] -= entry.Item2 * 6 / 8;
                        int end = current + entry.Item2;
                        CachedGPUBuffer vbuffer = ((CachedGPUBuffer)buffer[depth].VertexBuffer);
                        Array.Copy(vbuffer.Cache, end, vbuffer.Cache, current, vbuffer.Cache.Length - end);
                        CachedGPUBuffer tbuffer = ((CachedGPUBuffer)buffer[depth].TextureBuffer);
                        Array.Copy(tbuffer.Cache, end, tbuffer.Cache, current, tbuffer.Cache.Length - end);
                        CachedGPUBuffer cbuffer = ((CachedGPUBuffer)buffer[depth].ColorBuffer);
                        Array.Copy(cbuffer.Cache, end * 2, cbuffer.Cache, current * 2, cbuffer.Cache.Length - end * 2);
                        break;
                    } else {
                        current += entry.Item2;
                    }
                }
            }
            if (item.Visible) {
                Dictionary<int, List<DepthVertexData>> data = new Dictionary<int, List<DepthVertexData>>( );
                foreach (DepthVertexData vertexData in item.ConstructVertexData( )) {
                    if (!data.ContainsKey(vertexData.Depth))
                        data.Add(vertexData.Depth, new List<DepthVertexData>( ));
                    data[vertexData.Depth].Add(vertexData);
                }

                foreach (KeyValuePair<int, List<DepthVertexData>> entry in data) {
                    indexUsage[entry.Key].Add(new Tuple<UIItem, int>(item, entry.Value.Count * 8));
                    foreach (DepthVertexData vertexData in entry.Value) {
                        CachedGPUBuffer vbuffer = ((CachedGPUBuffer)buffer[entry.Key].VertexBuffer);
                        Array.Copy(vertexData.Verticies, 0, vbuffer.Cache, vertexCount[entry.Key], 8);
                        CachedGPUBuffer tbuffer = ((CachedGPUBuffer)buffer[entry.Key].TextureBuffer);
                        Array.Copy(Texture.Get(vertexData.Texture), 0, tbuffer.Cache, vertexCount[entry.Key], 8);
                        CachedGPUBuffer cbuffer = ((CachedGPUBuffer)buffer[entry.Key].ColorBuffer);
                        Array.Copy(vertexData.Color.ToOpenGL( ), 0, cbuffer.Cache, vertexCount[entry.Key] * 2, 16);
                        vertexCount[entry.Key] += 8;
                        renderCount[entry.Key] += 6;
                    }
                    bufferUpdated[entry.Key] = true;
                }
            }
        }

        public static void Update (DeltaTime dt) {
            while (updateQueue.Count > 0)
                UpdateBuffer(updateQueue.Dequeue( ));
            ApplyBufferUpdates( );

            foreach (UIItem item in uiItems[currentScreen])
                item.Update(dt);
        }

        private static void ApplyBufferUpdates ( ) {
            if (bufferUpdated[0]) {
                bufferUpdated[0] = false;
                ((CachedGPUBuffer)buffer[0].VertexBuffer).Apply( );
                ((CachedGPUBuffer)buffer[0].TextureBuffer).Apply( );
                ((CachedGPUBuffer)buffer[0].ColorBuffer).Apply( );
            }
            if (bufferUpdated[1]) {
                bufferUpdated[1] = false;
                ((CachedGPUBuffer)buffer[1].VertexBuffer).Apply( );
                ((CachedGPUBuffer)buffer[1].TextureBuffer).Apply( );
                ((CachedGPUBuffer)buffer[1].ColorBuffer).Apply( );
            }
            if (bufferUpdated[2]) {
                bufferUpdated[2] = false;
                ((CachedGPUBuffer)buffer[2].VertexBuffer).Apply( );
                ((CachedGPUBuffer)buffer[2].TextureBuffer).Apply( );
                ((CachedGPUBuffer)buffer[2].ColorBuffer).Apply( );
            }
        }
    }
}