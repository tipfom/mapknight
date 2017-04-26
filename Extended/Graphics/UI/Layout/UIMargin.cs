using System;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class UIMargin {
        public event Action Changed;

        private float _Left;
        public float Left { get { return _Left; } set { _Left = value; Changed?.Invoke( ); } }

        private float _Right;
        public float Right { get { return _Right; } set { _Right = value; Changed?.Invoke( ); } }

        private float _Top;
        public float Top { get { return _Top; } set { _Top = value; Changed?.Invoke( ); } }

        private float _Bottom;
        public float Bottom { get { return _Bottom; } set { _Bottom = value; Changed?.Invoke( ); } }

        public UIMargin (float horizontal, float vertical) {
            _Left = horizontal;
            _Right = horizontal;
            _Top = vertical;
            _Bottom = vertical;
        }

        public UIMargin (float left, float right, float top, float bottom) {
            _Left = left;
            _Right = right;
            _Top = top;
            _Bottom = bottom;
        }
    }
}
