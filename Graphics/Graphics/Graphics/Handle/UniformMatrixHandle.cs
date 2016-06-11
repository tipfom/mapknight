using OpenTK.Graphics.ES20;
using OpenTK;

namespace mapKnight.Extended.Graphics.Handle {
    public class UniformMatrixHandle {
        public int Location { get; private set; }

        public UniformMatrixHandle (int program, string name) {
            Location = GL.GetUniformLocation(program, name);
        }

        public void Set (Matrix4 matrix) {
            GL.UniformMatrix4(Location,1, false, ref matrix.Row0.X);
        }
    }
}
