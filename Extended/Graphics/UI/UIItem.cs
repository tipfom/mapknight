using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public abstract class UIItem : IDisposable {
        public const Anchor DEFAULT_ANCHOR = Anchor.Left | Anchor.Top;

        public event Action PositionChanged;
        public event Action SizeChanged;

        public delegate void HandleUpdate (UIItem sender);
        public event HandleUpdate Changed;
        public delegate void HandleItemClick ( );
        public event HandleItemClick Click;
        public event HandleItemClick Release;
        public bool Clicked { get { return (clickCount > 0); } }
        private bool multiClick;
        private int clickCount;

        public Rectangle Bounds { get; private set; }
        private UIMargin horizontalMargin;
        private UIMargin verticalMargin;

        private Vector2 _Position;
        public Vector2 Position {
            get { return _Position; }
            private set { _Position = value; Bounds = new Rectangle(Position, Size); RequestUpdate( ); }
        }
        private Vector2 _Size;
        public Vector2 Size {
            get { return _Size; }
            protected set { _Size = value; Bounds = new Rectangle(Position, value); SizeChanged?.Invoke( ); RequestUpdate( ); }
        }

        private bool _Visible = true;
        public bool Visible { get { return Screen.IsActive && _Visible; } set { _Visible = value; RequestUpdate( ); } }

        private int _Depth;
        public int Depth { get { return _Depth; } set { _Depth = value; RequestUpdate( ); } }

        public readonly Screen Screen;

        public UIItem (Screen owner, UIMargin hmargin, UIMargin vmargin, Vector2 size, int depth, bool multiclick = false) {
            UIRenderer.Add(owner, this);
            Screen = owner;

            this._Size = size;
            this.multiClick = multiclick;
            this._Depth = depth;

            vmargin.Bind(this);
            vmargin.Changed += ( ) => {
                Position = new Vector2(hmargin.ScreenPosition, vmargin.ScreenPosition);
            };
            hmargin.Bind(this);
            hmargin.Changed += ( ) => {
                Position = new Vector2(hmargin.ScreenPosition, vmargin.ScreenPosition);
            };
            Position = new Vector2(hmargin.ScreenPosition, vmargin.ScreenPosition);
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

        public virtual void Update (DeltaTime dt) {

        }

        public virtual void Dispose ( ) {
            UIRenderer.Remove(this);
        }

        public abstract List<DepthVertexData> ConstructVertexData ( );
    }
}