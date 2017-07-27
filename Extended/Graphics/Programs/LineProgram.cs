using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Programs {
    public class LineProgram : Program {
        public static LineProgram Program;

        public static void Init ( ) {
            Program = new LineProgram( );
        }

        public static void Destroy ( ) {
            Program.Dispose( );
        }

        private UniformVec4Handle colorHandle;
        private UniformMatrixHandle mvpmatrixHandle;

        public LineProgram ( ) : base("line.vert", "line.frag") {
            colorHandle = new UniformVec4Handle(glProgram, "u_color");
            mvpmatrixHandle = new UniformMatrixHandle(glProgram, "u_mvpmatrix");
        }

        public void Draw (IAttributeBuffer vertexBuffer, Color color, Matrix matrix, float width, int count, bool alphaBlending = true) {
            Apply(vertexBuffer, alphaBlending);
            colorHandle.Set(color.ToArray());
            mvpmatrixHandle.Set(matrix.MVP);
            GL.LineWidth(width);
            GL.DrawArrays(BeginMode.LineStrip, 0, count);
        }
    }
}
