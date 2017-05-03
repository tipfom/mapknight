using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using OpenTK.Graphics.ES20;
using static mapKnight.Extended.Graphics.Programs.ColorProgram;

namespace mapKnight.Extended.Graphics.Lightning {
    public class LightManager {
        private const int MAXIMUM_LIGHT_COUNT = 20;
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
        public Vector2 Position;

        private List<Light> lights;

        private Framebuffer lightBuffer;
        private Texture2D lightTexture;
        private ClientBuffer vertexBuffer;
        private CachedGPUBuffer colorBuffer;
        private GPUBuffer textureBuffer;
        private IndexBuffer indexBuffer;

        public LightManager (float Brightness) {
            this.Brightness = Brightness;

            lights = new List<Light>(MAXIMUM_LIGHT_COUNT);
            lightBuffer = new Framebuffer(Window.Size.Width, Window.Size.Height, true);
            lightTexture = Assets.Load<Texture2D>("lightmap");
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
                Vector2 transformedPosition = light.Position - Position;

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

            GL.ClearColor(0f, 0f, 0f, 1f - Brightness);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BlendEquationSeparate(All.Max, All.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            Program.Begin( );
            Program.Draw(indexBuffer, vertexBuffer, textureBuffer, colorBuffer, lightTexture, Matrix.Default, 6 * vertexBufferSize, 0, true);
            Program.End( );
            lightBuffer.Unbind( );

            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.SrcColor, BlendingFactorDest.SrcColor);

            Programs.FBOProgram.Program.Begin( );
            Programs.FBOProgram.Program.Draw(indexBuffer, DRAW_VERTEX_BUFFER, textureBuffer, lightBuffer.Texture, 6, true);
            Programs.FBOProgram.Program.End( );

            // RESET
            Window.UpdateBackgroundColor( );
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        public void Add (Light light) {
            lights.Add(light);
        }

        public void Remove (Light light) {
            lights.Remove(light);
        }
    }
}
