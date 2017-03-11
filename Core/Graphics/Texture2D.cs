using OpenTK.Graphics.ES20;
using System;

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
#elif Windows
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
    }
}
