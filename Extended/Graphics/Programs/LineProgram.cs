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

        public LineProgram ( ) : base(Assets.GetVertexShader("line"), Assets.GetFragmentShader("line")) {
            colorHandle = new UniformVec4Handle(glProgram, "u_color");
        }

        public void Draw (IAttributeBuffer vertexBuffer, Color color, float width, int count, bool alphaBlending = true) {
            Apply(vertexBuffer, alphaBlending);
            colorHandle.Set(new float[ ] { 1.0f, 0.0f, 0.0f, 1.0f });
            GL.LineWidth(10f);
            GL.DrawArrays(BeginMode.LineStrip, 0, count);
        }
    }
}
