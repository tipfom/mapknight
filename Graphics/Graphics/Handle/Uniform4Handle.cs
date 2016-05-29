using OpenTK.Graphics.ES20;

namespace mapKnight.Graphics.Handle {
    public class Uniform4Handle {
        public int Location { get; private set; }

        public Uniform4Handle (int program, string name) {
            Location = GL.GetUniformLocation(program, name);
        }

        public void Set (float[ ] vec4) {
            GL.Uniform4(Location, 1, vec4);
        }
    }
}
