using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public abstract class UIMargin : IDisposable {
        protected UIItem Owner { get; private set; }

        private float _ScreenPosition;
        public float ScreenPosition { get { return _ScreenPosition; } protected set { _ScreenPosition = value; Changed?.Invoke( ); } }
        public event Action Changed;

        private bool updateOnScreenChange;
        private bool allreadyBound;

        private float _Margin;
        public float Margin { get { return _Margin; } set { _Margin = value; CalculateScreenPosition( ); } }

        public UIMargin (float margin, bool updateonscreenchange) {
            _Margin = margin;
            updateOnScreenChange = updateonscreenchange;
        }

        ~UIMargin ( ) {
            Dispose( );
        }

        protected abstract void CalculateScreenPosition ( );

        public void Bind (UIItem owner) {
            if (allreadyBound) {
                Owner.SizeChanged -= CalculateScreenPosition;
            } else {
                if (updateOnScreenChange)
                    Window.Changed += CalculateScreenPosition;
                allreadyBound = true;
            }
            Owner = owner;
            Owner.SizeChanged += CalculateScreenPosition;

            CalculateScreenPosition( );
        }

        public void Dispose ( ) {
            if (allreadyBound) {
                Owner.SizeChanged -= CalculateScreenPosition;
                if (updateOnScreenChange)
                    Window.Changed -= CalculateScreenPosition;
            }
        }
    }
}
