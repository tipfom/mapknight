using mapKnight.Core;

namespace mapKnight.Graphics {
    public class Texture2D {
        public Size Size { get; private set; }
        public int Width { get { return Size.Width; } }
        public int Height { get { return Size.Height; } }
        public string Name { get; private set; }
        public int ID { get; private set; }

        public Texture2D (int id, Size size, string name) {
            Size = size;
            ID = id;
            Name = name;
        }

        public override string ToString () {
            return $"ID:{ID} Width:{Width} Height:{Height}";
        }

        public override bool Equals (object obj) {
            return obj.GetType ( ) == typeof (Texture2D) && ((Texture2D)obj).ID == this.ID;
        }

        public override int GetHashCode () {
            return ID.GetHashCode ( );
        }
    }
}
