using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Handle {
    public class TextureHandle {
        public int Location { get; private set; }

        public TextureHandle (int program) : this(program, "u_texture") {

        }

        public TextureHandle (int program, string name) {
            Location = GL.GetUniformLocation(program, name);
        }

        public void Set (int texture) {
            Set(texture, 0);
        }

        public void Set (int texture, int unit) {
            ErrorCode error = GL.GetErrorCode( );
            //GL.Enable(EnableCap.Texture2D);
            error = GL.GetErrorCode( );
            GL.ActiveTexture((TextureUnit)(TextureUnit.Texture0 + unit));
            error = GL.GetErrorCode( );
            GL.BindTexture(TextureTarget.Texture2D, texture);
            error = GL.GetErrorCode( );
            GL.Uniform1(Location, unit);
            error = GL.GetErrorCode( );
        }
    }
}
