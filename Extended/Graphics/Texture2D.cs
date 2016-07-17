using System;
using mapKnight.Core;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics {
    public class Texture2D : IDisposable {
        public Size Size { get; private set; }
        public int Width { get { return Size.Width; } }
        public int Height { get { return Size.Height; } }
        public string Name { get; private set; }
        public int ID { get; private set; }
        public bool Disposed { get; private set; } = false;

        public Texture2D (int id, Size size, string name) {
            Size = size;
            ID = id;
            Name = name;
        }

        public void Dispose ( ) {
            GL.DeleteTexture(ID);
            Size = new Size(0, 0);
            Name = null;
            ID = 0;
            Disposed = true;
        }

        public override string ToString ( ) {
            return $"ID:{ID} Width:{Width} Height:{Height}";
        }

        public override bool Equals (object obj) {
            return obj.GetType( ) == typeof(Texture2D) && ((Texture2D)obj).ID == this.ID;
        }

        public override int GetHashCode ( ) {
            return ID.GetHashCode( );
        }
    }
}
