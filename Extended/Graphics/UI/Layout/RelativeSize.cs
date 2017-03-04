using System;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class RelativeSize : IUISize {
        private Vector2 _Size;
        public Vector2 Size { get { return _Size; } }
        private Vector2 _Percent;
        public Vector2 Percent {
            get { return _Percent; }
            set { _Percent = value; UpdateSize( ); }
        }

        public float X { get { return _Size.X; } }
        public float Y { get { return _Size.Y; } }

        public event Action Changed;

        public RelativeSize(float horizontalPercent, float verticalPercent) : this(new Vector2(horizontalPercent, verticalPercent)) {
        }

        public RelativeSize(Vector2 percent) {
            Percent = percent;
            Window.Changed += ( ) => {
                UpdateSize( );
            };
        }

        private void UpdateSize( ) {
            _Size = new Vector2(Window.Ratio * 2f * _Percent.X, 2f * _Percent.Y);
            Changed?.Invoke( );
        }
    }
}
