using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;

namespace mapKnight.Extended.Graphics.UI {

    public static class UIRenderer {
        public static Spritebatch2D Texture;
        private const int MAX_QUADS = 800;
        private static BufferBatch buffer;
        private static Screen currentScreen;
        private static List<Tuple<UIItem, int>>[ ] indexUsage;
        private static int renderCount;
        private static Dictionary<Screen, List<UIItem>> uiItems;
        private static Dictionary<Screen, int> uiItemsOffset;
        private static int vertexCount;
        private static Queue<UIItem> updateQueue;
        private static int[ ] startPositions;

        private static CachedGPUBuffer vertexBuffer;
        private static CachedGPUBuffer textureBuffer;
        private static CachedGPUBuffer colorBuffer;

        public static void Init( ) {
            startPositions = new int[] { 0, 0, 0 };
            renderCount = 0;
            vertexCount = 0;
            updateQueue = new Queue<UIItem>( );

            IndexBuffer sharedIndexBuffer = new IndexBuffer(MAX_QUADS);
            vertexBuffer = new CachedGPUBuffer(2, MAX_QUADS, PrimitiveType.Quad);
            textureBuffer = new CachedGPUBuffer(2, MAX_QUADS, PrimitiveType.Quad);
            colorBuffer = new CachedGPUBuffer(4, MAX_QUADS, PrimitiveType.Quad);
            buffer = new BufferBatch(sharedIndexBuffer, vertexBuffer, colorBuffer, textureBuffer);

            uiItems = new Dictionary<Screen, List<UIItem>>( );
            uiItemsOffset = new Dictionary<Screen, int>( );
            indexUsage = new List<Tuple<UIItem, int>>[ ] { new List<Tuple<UIItem, int>>( ), new List<Tuple<UIItem, int>>( ), new List<Tuple<UIItem, int>>( ) };
        }

        public static void Dispose( ) {
            buffer.Dispose( );
            uiItems.Clear( );
            indexUsage[0].Clear( );
            updateQueue.Clear( );
        }

        public static List<UIItem> Current { get { return uiItems[currentScreen]; } }

        public static void Add (Screen screen, UIItem item) {
            if (!uiItems.ContainsKey(screen)) {
                uiItems.Add(screen, new List<UIItem>( ));
                uiItemsOffset.Add(screen, 0);
            }
            switch (item.Depth) {
                case UIDepths.FOREGROUND:
                    uiItems[screen].Insert(0, item);
                    uiItemsOffset[screen]++;
                    break;
                case UIDepths.MIDDLE:
                    uiItems[screen].Insert(uiItemsOffset[screen], item);
                    break;
                case UIDepths.BACKGROUND:
                    uiItems[screen].Add(item);
                    break;
            }
        }

        public static void Remove (UIItem uiItem) {
            if (!uiItems.ContainsKey(uiItem.Screen)) return;
            uiItems[uiItem.Screen].Remove(uiItem);
            uiItem.Visible = false;
            if (uiItem.Depth == UIDepths.FOREGROUND) uiItemsOffset[uiItem.Screen]--;
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
            Program.Draw(buffer, Texture, Matrix.Default, renderCount, 0, true);
            Program.End( );
        }

        public static void Prepare (Screen target) {
            currentScreen = target;
            vertexCount = 0;
            renderCount = 0;
            Array.Clear(startPositions, 0, 3);

            indexUsage[0].Clear( );
            indexUsage[1].Clear( );
            indexUsage[2].Clear( );
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

        private static void UpdateBuffer (UIItem item) {
            if (item.Visible) {
                Queue<DepthVertexData>[ ] data = { new Queue<DepthVertexData>( ), new Queue<DepthVertexData>( ), new Queue<DepthVertexData>( ) };
                foreach (DepthVertexData vertexData in item.ConstructVertexData( )) {
                    data[vertexData.Depth].Enqueue(vertexData);
                }

                for (int d = 0; d < 3; d++) {
                    Queue<DepthVertexData> queue = data[d];
                    int verticies = queue.Count * 8;
                    int position, index = FindCurrentIndex(item, d, out position);
                    int delta = verticies - ((index == -1) ? 0 : indexUsage[d][index].Item2);

                    if (delta != 0) {
                        // create space
                        int oldend = position + ((index == -1) ? 0 : indexUsage[d][index].Item2), newend = position + verticies, length = Math.Max(oldend, newend);
                        Array.Copy(vertexBuffer.Cache, oldend, vertexBuffer.Cache, newend, vertexBuffer.Cache.Length - length);
                        Array.Copy(textureBuffer.Cache, oldend, textureBuffer.Cache, newend, textureBuffer.Cache.Length - length);
                        Array.Copy(colorBuffer.Cache, oldend * 2, colorBuffer.Cache, newend * 2, colorBuffer.Cache.Length - length * 2);
                    }

                    if (index != -1) indexUsage[d][index] = new Tuple<UIItem, int>(item, verticies);
                    else indexUsage[d].Add(new Tuple<UIItem, int>(item, verticies));

                    vertexCount += delta;
                    renderCount += delta * 6 / 8;
                    while (queue.Count > 0) {
                        DepthVertexData vertexData = queue.Dequeue( );
                        Array.Copy(vertexData.Verticies, 0, vertexBuffer.Cache, position, 8);
                        Array.Copy(Texture[vertexData.Texture], 0, textureBuffer.Cache, position, 8);
                        Array.Copy(vertexData.Color.ToOpenGL( ), 0, colorBuffer.Cache, position * 2, 16);
                        position += 8;
                    }
                    for (int di = d; di < 3; di++) startPositions[di] += delta;
                }
            } else {
                int index;
                int position;
                for (int d = 0; d < 3; d++) {
                    index = FindCurrentIndex(item, d, out position);
                    if (index > -1) {
                        int verticiesToClear = indexUsage[d][index].Item2;
                        indexUsage[d].RemoveAt(index);
                        vertexCount -= verticiesToClear;
                        renderCount -= verticiesToClear * 6 / 8;
                        for (int di = d; di < 3; di++) startPositions[di] -= verticiesToClear;
                        int end = position + verticiesToClear;
                        Array.Copy(vertexBuffer.Cache, end, vertexBuffer.Cache, position, vertexBuffer.Cache.Length - end);
                        Array.Copy(textureBuffer.Cache, end, textureBuffer.Cache, position, textureBuffer.Cache.Length - end);
                        Array.Copy(colorBuffer.Cache, end * 2, colorBuffer.Cache, position * 2, colorBuffer.Cache.Length - end * 2);
                    }
                }
            }
        }

        private static int FindCurrentIndex (UIItem item, int depth, out int position) {
            position = (depth == 0) ? 0 : startPositions[depth - 1];
            for (int i = 0; i < indexUsage[depth].Count; i++) {
                Tuple<UIItem, int> entry = indexUsage[depth][i];
                if (entry.Item1 == item) {
                    return i;
                } else {
                    position += entry.Item2;
                }
            }
            return -1;
        }

        public static void Update (DeltaTime dt) {
            if (updateQueue.Count > 0) {
                while (updateQueue.Count > 0)
                    UpdateBuffer(updateQueue.Dequeue( ));
                ApplyBufferUpdates( );
            }

            foreach (UIItem item in uiItems[currentScreen])
                item.Update(dt);
        }

        private static void ApplyBufferUpdates ( ) {
            vertexBuffer.Apply( );
            textureBuffer.Apply( );
            colorBuffer.Apply( );
        }
    }
}