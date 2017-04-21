using System;
#if __ANDROID__
using OpenTK.Graphics.ES20;
#endif

namespace mapKnight.Core.Graphics {
    public class Texture2D : IDisposable {
        public readonly Size Size;
        public readonly int Width;
        public readonly int Height;
        public readonly string Name;
        public readonly int ID;
        public bool Disposed { get; private set; } = false;

        public Texture2D(int ID, int Width, int Height, string Name = "%") : this(ID, new Size(Width, Height), Name) {
        }

        public Texture2D(int ID, Size Size, string Name = "%") {
            this.ID = ID;
            this.Size = Size;
            this.Width = Size.Width;
            this.Height = Size.Height;
            this.Name = Name;
        }

#if __ANDROID__
        public void Dispose( ) {
            if (Disposed)
                return;
            GL.DeleteTexture(ID);
            Disposed = true;
        }
#elif __WINDOWS__
        public void Dispose( ) {
        }
#endif

        public override string ToString( ) {
            return $"{Name}({ID}) w:{Width}, h:{Height}";
        }

        public override bool Equals(object obj) {
            return obj.GetHashCode( ) == ID;
        }

        public override int GetHashCode( ) {
            return ID;
        }

#if __ANDROID__
        public static Texture2D CreateEmpty() {
            int id = GL.GenTexture( );
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 1, 1, 0, PixelFormat.Rgb, PixelType.UnsignedByte, new byte[ ] { 255, 255, 255 });
            return new Texture2D(id, new Size(1, 1), "empty");
        }
#endif
    }
}
