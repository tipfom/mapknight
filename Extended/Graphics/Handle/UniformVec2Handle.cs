using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Handle {
    public class UniformVec4Handle {
        public readonly int Location;

        public UniformVec4Handle (int program, string name) {
            Location = GL.GetUniformLocation(program, name);
        }

        public void Set (float[ ] vec4) {
            GL.Uniform4(Location, 1, vec4);
        }
    }
}
