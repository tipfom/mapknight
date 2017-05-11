using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using OpenTK.Graphics.ES20;
using mapKnight.Extended.Graphics.Programs;

namespace mapKnight.Extended.Graphics.Lightning {
    public class LightManager {
        private const int MAXIMUM_LIGHT_COUNT = 20;
        private const int TILE_LIGHT_RESOLUTION = 10;
        private const float TILE_LIGHT_DISTANCE = 2 * TILE_LIGHT_RESOLUTION - .5f;
        private const int LIGHT_VERTEX_COUNT_PER_TILE = TILE_LIGHT_RESOLUTION * TILE_LIGHT_RESOLUTION * 8;
        private static GPUBuffer DRAW_VERTEX_BUFFER;

        private static float[ ] GenerateLightTextureBuffer ( ) {
            float[ ] textureBuffer = new float[8 * MAXIMUM_LIGHT_COUNT];
            for (int i = 0; i < MAXIMUM_LIGHT_COUNT; i++) {
                int p = i * 8;
                textureBuffer[p + 0] = 0f;
                textureBuffer[p + 1] = 1f;
                textureBuffer[p + 2] = 0f;
                textureBuffer[p + 3] = 0f;
                textureBuffer[p + 4] = 1f;
                textureBuffer[p + 5] = 0f;
                textureBuffer[p + 6] = 1f;
                textureBuffer[p + 7] = 1f;
            }
            return textureBuffer;
        }

        public static void Init ( ) {
            DRAW_VERTEX_BUFFER = new GPUBuffer(2, 1, PrimitiveType.Quad, new float[ ] { -1, 1, -1, -1, 1, -1, 1, 1 }, BufferUsage.StaticDraw);
        }

        public static void Destroy ( ) {
            DRAW_VERTEX_BUFFER.Dispose( );
        }

        public float Brightness;

        private Vector2 _Position;
        public Vector2 Position {
            get { return _Position; }
            set {
                if (_Position != value) {
                    tilemapMatrix.ResetView( );
                    tilemapMatrix.TranslateView(-value.X, -value.Y, 0);
                    tilemapMatrix.CalculateMVP( );

                    Vector2 bufferPosition = value - tilemapPresentedSize / 2f;
                    tilemapTextureBuffer.Cache[0] = bufferPosition.X / tilemapSize.X;
                    tilemapTextureBuffer.Cache[1] = (bufferPosition.Y + tilemapPresentedSize.Y) / tilemapSize.Y;
                    tilemapTextureBuffer.Cache[2] = tilemapTextureBuffer.Cache[0];
                    tilemapTextureBuffer.Cache[3] = bufferPosition.Y / tilemapSize.Y;
                    tilemapTextureBuffer.Cache[4] = (bufferPosition.X + tilemapPresentedSize.X) / tilemapSize.X;
                    tilemapTextureBuffer.Cache[5] = tilemapTextureBuffer.Cache[3];
                    tilemapTextureBuffer.Cache[6] = tilemapTextureBuffer.Cache[4];
                    tilemapTextureBuffer.Cache[7] = tilemapTextureBuffer.Cache[1];
                    tilemapTextureBuffer.Apply( );
                }
                _Position = value;
            }
        }

        private List<Light> lights;

        private Vector2 tilemapPresentedSize, tilemapSize;
        private Matrix tilemapMatrix;
        private Framebuffer tilemapBuffer;
        private Framebuffer lightBuffer;
        private Texture2D pointLightMap;

        private CachedGPUBuffer tilemapTextureBuffer;
        private ClientBuffer vertexBuffer;
        private CachedGPUBuffer colorBuffer;
        private GPUBuffer textureBuffer;
        private IndexBuffer indexBuffer;

        public LightManager (float Brightness, Size mapSize) {
            this.Brightness = Brightness;

            lights = new List<Light>(MAXIMUM_LIGHT_COUNT);
            lightBuffer = new Framebuffer(Window.Size.Width / 2, Window.Size.Height / 2, true, (int)All.Linear);
            tilemapBuffer = new Framebuffer(mapSize.Width * TILE_LIGHT_RESOLUTION, mapSize.Height * TILE_LIGHT_RESOLUTION, true, (int)All.Linear);
            pointLightMap = Assets.Load<Texture2D>("lightning/point");
            indexBuffer = new IndexBuffer(MAXIMUM_LIGHT_COUNT);
            vertexBuffer = new ClientBuffer(2, MAXIMUM_LIGHT_COUNT, PrimitiveType.Quad);
            textureBuffer = new GPUBuffer(2, MAXIMUM_LIGHT_COUNT, PrimitiveType.Quad, GenerateLightTextureBuffer( ), BufferUsage.StaticDraw);
            colorBuffer = new CachedGPUBuffer(4, MAXIMUM_LIGHT_COUNT, PrimitiveType.Quad);
        }

        public void Draw ( ) {
            int vertexBufferSize = 0;
            for (int i = 0; i < lights.Count; i++) {
                Light light = lights[i];
                int posVertex = vertexBufferSize * 8, posColor = posVertex * 2;
                Vector2 transformedPosition = light.Position;

                float top = transformedPosition.Y + light.Radius;
                float bottom = transformedPosition.Y - light.Radius;
                float left = transformedPosition.X - light.Radius;
                float right = transformedPosition.X + light.Radius;

                vertexBuffer.Data[posVertex] = left;
                vertexBuffer.Data[posVertex + 1] = top;
                vertexBuffer.Data[posVertex + 2] = left;
                vertexBuffer.Data[posVertex + 3] = bottom;
                vertexBuffer.Data[posVertex + 4] = right;
                vertexBuffer.Data[posVertex + 5] = bottom;
                vertexBuffer.Data[posVertex + 6] = right;
                vertexBuffer.Data[posVertex + 7] = top;

                Array.Copy(light.Color.ToOpenGL( ), 0, colorBuffer.Cache, posColor, 16);

                vertexBufferSize++;
            }
            colorBuffer.Apply( );

            lightBuffer.Bind( );

            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            DarkenProgram.Program.Begin( );
            DarkenProgram.Program.Draw(indexBuffer, DRAW_VERTEX_BUFFER, tilemapTextureBuffer, 1f, tilemapBuffer.Texture, 6, 0, true);
            DarkenProgram.Program.End( );

            GL.BlendEquationSeparate(All.Max, All.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            ColorProgram.Program.Begin( );
            ColorProgram.Program.Draw(indexBuffer, vertexBuffer, textureBuffer, colorBuffer, pointLightMap, tilemapMatrix, 6 * vertexBufferSize, 0, true);
            ColorProgram.Program.End( );
            lightBuffer.Unbind( );

            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.SrcColor, BlendingFactorDest.SrcColor);

            FBOProgram.Program.Begin( );
            FBOProgram.Program.Draw(indexBuffer, DRAW_VERTEX_BUFFER, textureBuffer, lightBuffer.Texture, 6, true);
            FBOProgram.Program.End( );

            // RESET
            Window.UpdateBackgroundColor( );
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        public void UpdateTilemapMatrix (Map map, Vector2 realDrawSize) {
            tilemapMatrix = new Matrix(realDrawSize);
            tilemapPresentedSize = realDrawSize * 2f;
            tilemapSize = (Vector2)map.Size;
        }

        public void RenderTilemap (Map map) {
            tilemapTextureBuffer = new CachedGPUBuffer(2, 1, PrimitiveType.Quad);

            float width = 2f / map.Width / TILE_LIGHT_RESOLUTION, height = 2f / map.Height / TILE_LIGHT_RESOLUTION;
            ClientBuffer vertexBuffer = new ClientBuffer(2, map.Width * TILE_LIGHT_RESOLUTION * TILE_LIGHT_RESOLUTION, PrimitiveType.Quad);
            ClientBuffer textureBuffer = new ClientBuffer(2, map.Width * TILE_LIGHT_RESOLUTION * TILE_LIGHT_RESOLUTION, PrimitiveType.Quad);
            ClientBuffer colorBuffer = new ClientBuffer(4, map.Width * TILE_LIGHT_RESOLUTION * TILE_LIGHT_RESOLUTION, PrimitiveType.Quad);
            IndexBuffer indexBuffer = new IndexBuffer(map.Width * TILE_LIGHT_RESOLUTION * TILE_LIGHT_RESOLUTION);
            Texture2D texture = Texture2D.CreateEmpty( );
            Matrix matrix = new Matrix(new Vector2(1f, 1f));

            tilemapBuffer.Bind( );

            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            for (int y = 0; y < map.Height; y++) {
                for (int x = 0; x < map.Width; x++) {
                    int bPosition = LIGHT_VERTEX_COUNT_PER_TILE * x;

                    for (int ty = 0; ty < TILE_LIGHT_RESOLUTION; ty++) {
                        for (int tx = 0; tx < TILE_LIGHT_RESOLUTION; tx++) {
                            int vPosition = bPosition + 8 * (ty * TILE_LIGHT_RESOLUTION + tx);
                            int cPosition = 2 * vPosition;

                            float left = -1f + (TILE_LIGHT_RESOLUTION * x + tx) * width;
                            float right = left + width;
                            float bottom = -1f + (TILE_LIGHT_RESOLUTION * y + ty) * height;
                            float top = bottom + height;

                            vertexBuffer.Data[vPosition + 0] = left;
                            vertexBuffer.Data[vPosition + 1] = top;
                            vertexBuffer.Data[vPosition + 2] = left;
                            vertexBuffer.Data[vPosition + 3] = bottom;
                            vertexBuffer.Data[vPosition + 4] = right;
                            vertexBuffer.Data[vPosition + 5] = bottom;
                            vertexBuffer.Data[vPosition + 6] = right;
                            vertexBuffer.Data[vPosition + 7] = top;

                            if (map.HasCollider(x, y)) {
                                float lightLvl = 1f;

                                for (int cy = 1; cy <= 2; cy++) {
                                    for (int cx = 1; cx <= 2; cx++) {
                                        int px = x + cx, py = y + cy;
                                        if (px >= 0 && px < map.Width && py >= 0 && py < map.Height && !map.HasCollider(px, py)) {
                                            float dx = cx * TILE_LIGHT_RESOLUTION - tx;
                                            float dy = cy * TILE_LIGHT_RESOLUTION - ty;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }

                                        py = y - cy;
                                        if (px >= 0 && px < map.Width && py >= 0 && py < map.Height && !map.HasCollider(px, py)) {
                                            float dx = cx * TILE_LIGHT_RESOLUTION - tx;
                                            float dy = (cy - 1) * TILE_LIGHT_RESOLUTION + ty;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }

                                        px = x - cx;
                                        if (px >= 0 && px < map.Width && py >= 0 && py < map.Height && !map.HasCollider(px, py)) {
                                            float dx = (cx - 1) * TILE_LIGHT_RESOLUTION + tx;
                                            float dy = (cy - 1) * TILE_LIGHT_RESOLUTION + ty;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }

                                        py = y + cy;
                                        if (px >= 0 && px < map.Width && py >= 0 && py < map.Height && !map.HasCollider(px, py)) {
                                            float dx = (cx - 1) * TILE_LIGHT_RESOLUTION + tx;
                                            float dy = cy * TILE_LIGHT_RESOLUTION - ty;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }
                                    }
                                }

                                for (int d = 1; d <= 2; d++) {
                                    int temp = x + d;
                                    if (temp < map.Width && !map.HasCollider(temp, y)) {
                                        lightLvl = Math.Min(lightLvl, (d * TILE_LIGHT_RESOLUTION - tx) / TILE_LIGHT_DISTANCE);
                                    }

                                    temp = x - d;
                                    if (temp > 0 && !map.HasCollider(temp, y)) {
                                        lightLvl = Math.Min(lightLvl, ((d - 1) * TILE_LIGHT_RESOLUTION + tx) / TILE_LIGHT_DISTANCE);
                                    }

                                    temp = y + d;
                                    if (temp < map.Height && !map.HasCollider(x, temp)) {
                                        lightLvl = Math.Min(lightLvl, (d * TILE_LIGHT_RESOLUTION - ty) / TILE_LIGHT_DISTANCE);
                                    }

                                    temp = y - d;
                                    if (temp > 0 && !map.HasCollider(x, temp)) {
                                        lightLvl = Math.Min(lightLvl, ((d - 1) * TILE_LIGHT_RESOLUTION + ty) / TILE_LIGHT_DISTANCE);
                                    }
                                }

                                lightLvl = 1f - lightLvl;
                                colorBuffer.Data[cPosition + 03] = lightLvl;
                                colorBuffer.Data[cPosition + 07] = lightLvl;
                                colorBuffer.Data[cPosition + 11] = lightLvl;
                                colorBuffer.Data[cPosition + 15] = lightLvl;
                            } else {
                                colorBuffer.Data[cPosition + 03] = 1f;
                                colorBuffer.Data[cPosition + 07] = 1f;
                                colorBuffer.Data[cPosition + 11] = 1f;
                                colorBuffer.Data[cPosition + 15] = 1f;
                            }

                            textureBuffer.Data[vPosition + 0] = 0f;
                            textureBuffer.Data[vPosition + 1] = 1f;
                            textureBuffer.Data[vPosition + 2] = 0f;
                            textureBuffer.Data[vPosition + 3] = 0f;
                            textureBuffer.Data[vPosition + 4] = 1f;
                            textureBuffer.Data[vPosition + 5] = 0f;
                            textureBuffer.Data[vPosition + 6] = 1f;
                            textureBuffer.Data[vPosition + 7] = 1f;
                        }
                    }

                }

                ColorProgram.Program.Begin( );
                ColorProgram.Program.Draw(indexBuffer, vertexBuffer, textureBuffer, colorBuffer, texture, matrix, indexBuffer.Length, 0, true);
                ColorProgram.Program.End( );
            }

            tilemapBuffer.Unbind( );

            texture.Dispose( );
            vertexBuffer.Dispose( );
            textureBuffer.Dispose( );
            indexBuffer.Dispose( );
        }

        public void Add (Light light) {
            lights.Add(light);
        }

        public void Remove (Light light) {
            lights.Remove(light);
        }
    }
}
