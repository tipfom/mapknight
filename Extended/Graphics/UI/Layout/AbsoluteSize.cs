using System;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class AbsoluteSize : IUISize {
        private Vector2 _Size;
        public Vector2 Size { get { return _Size; } set { _Size = value; Changed?.Invoke( ); } }

        public float X { get { return _Size.X; } }
        public float Y { get { return _Size.Y; } }

        public event Action Changed;

        public AbsoluteSize( ) {
        }

        public AbsoluteSize(Vector2 size) {
            _Size = size;
        }

        public AbsoluteSize(float width, float height) {
            _Size = new Vector2(width, height);
        }
    }
}
