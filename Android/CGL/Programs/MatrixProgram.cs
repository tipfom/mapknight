using Java.Nio;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL.Programs {
    public class MatrixProgram {
        private int vertexShader;
        private int fragmentShader;

        private int program;
        private int positionHandle;
        private int textureUniformHandle;
        private int textureCoordinateHandle;
        private int mvpMatrixHandle;

        public MatrixProgram () {
            // get shader
            vertexShader = ProgramHelper.GetVertexShader ("matrix");
            fragmentShader = ProgramHelper.GetFragmentShader ("normal");
            program = ProgramHelper.CreateProgram (vertexShader, fragmentShader);

            // get handles
            positionHandle = GL.GlGetAttribLocation (program, "a_position");
            textureUniformHandle = GL.GlGetUniformLocation (program, "u_texture");
            textureCoordinateHandle = GL.GlGetAttribLocation (program, "a_texcoord");
            mvpMatrixHandle = GL.GlGetUniformLocation (program, "u_mvpmatrix");
        }

        public void Draw (ShortBuffer indexBuffer) {
            Draw (indexBuffer, indexBuffer.Limit ( ), GL.GlTriangles);
        }

        public void Draw (ShortBuffer indexBuffer, int count) {
            Draw (indexBuffer, count, GL.GlTriangles);
        }

        public void Draw (ShortBuffer indexBuffer, int count, int mode) {
            GL.GlDrawElements (mode, count, GL.GlUnsignedShort, indexBuffer);
        }

        public void Draw (FloatBuffer vertexBuffer, FloatBuffer textureBuffer, ShortBuffer indexBuffer, int texture, float[ ] matrix, bool alphaBlending = false) {
            SetTexture (texture);
            SetMVPMatrix (matrix);
            SetTextureBuffer (textureBuffer);
            SetVertexBuffer (vertexBuffer);
            if (alphaBlending)
                EnableAlphaBlending ( );
            Draw (indexBuffer);
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

        public void SetMVPMatrix (float[ ] matrix) {
            GL.GlUniformMatrix4fv (mvpMatrixHandle, 1, false, matrix, 0);
        }
    }
}