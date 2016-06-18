using mapKnight.Core;
using mapKnight.Extended.Graphics.Handle;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Programs {
    public class MatrixProgram : Program {
        private UniformMatrixHandle mvpMatrixHandle;

        public MatrixProgram ( ) : base(Assets.GetVertexShader("matrix"), Assets.GetFragmentShader("normal")) {
            mvpMatrixHandle = new UniformMatrixHandle(glProgram, "u_mvpmatrix");
        }

        public void Draw(BufferBatch buffer, Texture2D texture, Matrix4 matrix, bool alphaBlending = false) {
            Draw(buffer.Verticies, buffer.Dimesions, buffer.Texture, buffer.Indicies, texture.ID, matrix, buffer.QuadCount*6, alphaBlending);
        }

        public void Draw (float[ ] vertexBuffer, float[ ] textureBuffer, short[ ] indexBuffer, int texture, Matrix4 matrix, bool alphaBlending = true) {
            Draw(vertexBuffer, 2, textureBuffer, indexBuffer, texture, matrix, indexBuffer.Length, alphaBlending);
        }

        public void Draw (float[ ] vertexBuffer, int dimension, float[ ] textureBuffer, short[ ] indexBuffer, int texture, Matrix4 matrix, int count, bool alphaBlending = true) {
            ErrorCode error = GL.GetErrorCode( );
            Apply(texture, vertexBuffer, dimension, textureBuffer, alphaBlending);
            error = GL.GetErrorCode( );
            mvpMatrixHandle.Set(matrix);
            error = GL.GetErrorCode( );
            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, indexBuffer);
            error = GL.GetErrorCode( );
        }

        public static MatrixProgram Program { get; set; }
    }
}
