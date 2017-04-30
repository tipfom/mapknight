using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Handle {
    public class UniformVec2Handle {
        public readonly int Location;

        public UniformVec2Handle (int program, string name) {
            Location = GL.GetUniformLocation(program, name);
        }

        public void Set (float[ ] vec4) {
            GL.Uniform2(Location, 1, vec4);
        }
    }
}
