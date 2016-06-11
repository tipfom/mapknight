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

        public void Draw (float[ ] vertexBuffer, float[ ] textureBuffer, short[ ] indexBuffer, int texture, Matrix4 matrix, bool alphaBlending = true) {
            Draw(vertexBuffer, 2, textureBuffer, indexBuffer, texture, matrix, indexBuffer.Length, alphaBlending);
        }

        public void Draw (float[ ] vertexBuffer, int dimension, float[ ] textureBuffer, short[ ] indexBuffer, int texture, Matrix4 matrix, int count, bool alphaBlending = true) {
            Apply(texture, vertexBuffer, dimension, textureBuffer, alphaBlending);
            mvpMatrixHandle.Set(matrix);
            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, indexBuffer);
            ErrorCode error = GL.GetErrorCode( );
        }

        public static MatrixProgram Program { get; set; }
    }
}
