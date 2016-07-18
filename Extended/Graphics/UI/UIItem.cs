using System;
using System.Collections.Generic;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI {
    public abstract class UIItem {
        public const Anchor DEFAULT_ANCHOR = Anchor.Left | Anchor.Top;
        public const int DEFAULT_DEPTH = 3;

        public delegate void HandleUpdate (UIItem sender);
        public event HandleUpdate Changed;
        public delegate void HandleItemClick ( );
        public event HandleItemClick Click;
        public event HandleItemClick Release;
        public bool Clicked { get { return (clickCount > 0); } }
        private bool multiClick;
        private int clickCount;

        public Rectangle Bounds;
        public Vector2 Position { get { return Bounds.Position; } set { Bounds.Position = value; RequestUpdate( ); } }
        public Vector2 Size { get { return Bounds.Size; } set { Bounds.Size = value; RequestUpdate( ); } }

        private bool _Visible = true;
        public bool Visible { get { return _Visible; } set { _Visible = value; RequestUpdate( ); } }

        private int _Depth;
        public int Depth { get { return _Depth; } set { _Depth = value; DepthOnScreen = DEFAULT_DEPTH + value; RequestUpdate( ); } }
        protected int DepthOnScreen { get; private set; }

        public UIItem (Screen owner, Rectangle bounds, int depth, bool multiclick = false) {
            UIRenderer.Add(owner, this);

            this.Bounds = bounds;
            this.multiClick = multiclick;
            this._Depth = depth;
            this.DepthOnScreen = depth + DEFAULT_DEPTH;
        }

        public virtual void HandleTouch (UITouchAction action) {
            switch (action) {
                case UITouchAction.Begin:
                case UITouchAction.Enter:
                    if (!Clicked || multiClick) {
                        clickCount++;
                        Click?.Invoke( );
                    }
                    break;
                case UITouchAction.End:
                case UITouchAction.Leave:
                    if (Clicked) {
                        clickCount--;
                        if (!Clicked)
                            Release?.Invoke( );
                    }
                    break;
            }
        }

        public bool Collides (Vector2 touchPosition) {
            return Bounds.Collides(touchPosition);
        }

        protected void RequestUpdate ( ) {
            if (this.Visible)
                Changed?.Invoke(this);
        }

        public virtual void Update (TimeSpan dt) {

        }

        public virtual List<VertexData> GetVertexData ( ) {
            return new List<VertexData>( );
        }
    }
}