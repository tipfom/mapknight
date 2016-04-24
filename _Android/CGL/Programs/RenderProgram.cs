using Java.Nio;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL.Programs {
    public class NormalProgram {
        private int vertexShader;
        private int fragmentShader;

        private int program;
        private int positionHandle;
        private int textureUniformHandle;
        private int textureCoordinateHandle;

        public NormalProgram () {
            // get shader
            vertexShader = ProgramHelper.GetVertexShader ("normal");
            fragmentShader = ProgramHelper.GetFragmentShader ("normal");
            program = ProgramHelper.CreateProgram (vertexShader, fragmentShader);

            // get handles
            positionHandle = GL.GlGetAttribLocation (program, "vPosition");
            textureUniformHandle = GL.GlGetUniformLocation (program, "u_Texture");
            textureCoordinateHandle = GL.GlGetAttribLocation (program, "a_TexCoordinate");
        }

        public void Draw (ShortBuffer indexBuffer) {
            Draw (indexBuffer, indexBuffer.Limit (), GL.GlTriangles);
        }

        public void Draw (ShortBuffer indexBuffer, int count) {
            Draw (indexBuffer, count, GL.GlTriangles);
        }

        public void Draw (ShortBuffer indexBuffer, int count, int mode) {
            GL.GlDrawElements (mode, count, GL.GlUnsignedShort, indexBuffer);
        }

        public void Begin () {
            GL.GlUseProgram (program);
            GL.GlEnableVertexAttribArray (positionHandle);
            GL.GlEnableVertexAttribArray (textureCoordinateHandle);
        }

        public void End () {
            GL.GlDisableVertexAttribArray (positionHandle);
            GL.GlDisableVertexAttribArray (textureCoordinateHandle);
        }

        public void EnableAlphaBlending () {
            GL.GlEnable (GL.GlBlend);
            GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);
        }

        public void SetTexture (int texture) {
            GL.GlActiveTexture (GL.GlTexture0);
            GL.GlBindTexture (GL.GlTexture2d, texture);
            GL.GlUniform1i (textureUniformHandle, 0);
        }

        public void SetTextureBuffer (FloatBuffer textureBuffer) {
            SetTextureBuffer (textureBuffer, 2);
        }

        public void SetTextureBuffer (FloatBuffer textureBuffer, int dimensions) {
            GL.GlVertexAttribPointer (textureCoordinateHandle, dimensions, GL.GlFloat, false, 0, textureBuffer);
        }

        public void SetVertexBuffer (FloatBuffer vertexBuffer) {
            SetVertexBuffer (vertexBuffer, 2);
        }

        public void SetVertexBuffer (FloatBuffer vertexBuffer, int dimensions) {
            GL.GlVertexAttribPointer (positionHandle, dimensions, GL.GlFloat, false, dimensions * sizeof (float), vertexBuffer);
        }
    }
}