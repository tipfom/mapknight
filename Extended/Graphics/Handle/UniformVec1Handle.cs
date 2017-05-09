using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Handle {
    class UniformVec1Handle {
        public readonly int Location;

        public UniformVec1Handle (int program, string name) {
            Location = GL.GetUniformLocation(program, name);
        }

        public void Set (float vec1) {
            GL.Uniform1(Location, 1, ref vec1);
        }
    }
}
