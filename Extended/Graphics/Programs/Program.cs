using OpenTK.Graphics.ES20;
using mapKnight.Extended.Graphics.Handle;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.Programs {
    public abstract class Program {
        protected int glProgram;

        private TextureHandle textureHandle;
        private AttributeHandle positionHandle;
        private AttributeHandle textureCoordsHandle;

        public Program (int vertexBuffer, int fragmentBuffer) {
            glProgram = CompileProgram(vertexBuffer, fragmentBuffer);

            positionHandle = new AttributeHandle(glProgram, "a_position");
            textureCoordsHandle = new AttributeHandle(glProgram, "a_texcoord");
            textureHandle = new TextureHandle(glProgram);
        }

        public virtual void Begin ( ) {
            GL.UseProgram(glProgram);
            positionHandle.Enable( );
            textureCoordsHandle.Enable( );
        }

        public virtual void End ( ) {
            positionHandle.Disable( );
            textureCoordsHandle.Disable( );
        }

        protected void Apply(int texture, float[] vertexBuffer, int dimension, float[] textureBuffer, bool alphaBlending) {
            if (alphaBlending)
                EnableAlphaBlending( );
            ErrorCode error = GL.GetErrorCode( );
            textureHandle.Set(texture);
            error = GL.GetErrorCode( );
            positionHandle.Set(vertexBuffer, dimension);
            error = GL.GetErrorCode( );
            textureCoordsHandle.Set(textureBuffer, 2);
            error = GL.GetErrorCode( );
        }

        private void EnableAlphaBlending ( ) {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        private static int CompileProgram (int vertexBuffer, int fragmentBuffer) {
            int program = GL.CreateProgram( );

            GL.AttachShader(program, vertexBuffer);
            GL.AttachShader(program, fragmentBuffer);

            GL.LinkProgram(program);

            Log.Print(typeof(Program), "Loaded new program (id = " + program.ToString( ) + ")");
            Log.Print(typeof(Program), "Log = " + GL.GetProgramInfoLog(program));
            Log.Print(typeof(Program), "GL.GLGetError returned " + GL.GetErrorCode( ).ToString( ));

            return program;
        }
    }
}
