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
        private Framebuffer lightBuffer;
        private Texture2D pointLightMap;
        private Texture2D tilemapLightMap;

        private CachedGPUBuffer tilemapTextureBuffer;
        private ClientBuffer vertexBuffer;
        private CachedGPUBuffer colorBuffer;
        private GPUBuffer textureBuffer;
        private IndexBuffer indexBuffer;

        public LightManager (float Brightness, Map map) {
            this.Brightness = Brightness;

            lights = new List<Light>(MAXIMUM_LIGHT_COUNT);
            lightBuffer = new Framebuffer(Window.Size.Width / 2, Window.Size.Height / 2, true, (int)All.Linear);
            pointLightMap = Assets.Load<Texture2D>("lightning/point");
            tilemapLightMap = Assets.GetTexture(InterpolationMode.Linear, "textures/maps/shadows/" + map.Name + ".png");
            indexBuffer = new IndexBuffer(MAXIMUM_LIGHT_COUNT);
            vertexBuffer = new ClientBuffer(2, MAXIMUM_LIGHT_COUNT, PrimitiveType.Quad);
            tilemapTextureBuffer = new CachedGPUBuffer(2, 1, PrimitiveType.Quad);
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

                Array.Copy(light.Color.ToOpenGL4( ), 0, colorBuffer.Cache, posColor, 16);

                vertexBufferSize++;
            }
            colorBuffer.Apply( );

            lightBuffer.Bind( );

            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            DarkenProgram.Program.Begin( );
            DarkenProgram.Program.Draw(indexBuffer, DRAW_VERTEX_BUFFER, tilemapTextureBuffer, Brightness, tilemapLightMap, 6, 0, true);
            DarkenProgram.Program.End( );

#pragma warning disable CS0168
            GL.BlendEquationSeparate(All.Max, All.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            ColorProgram.Program.Begin( );
            ColorProgram.Program.Draw(indexBuffer, vertexBuffer, textureBuffer, colorBuffer, pointLightMap, tilemapMatrix, 6 * vertexBufferSize, 0, true);
            ColorProgram.Program.End( );
            lightBuffer.Unbind( );
            GL.Clear(0);

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

        public void Add (Light light) {
            lights.Add(light);
        }

        public void Remove (Light light) {
            lights.Remove(light);
        }
    }
}
