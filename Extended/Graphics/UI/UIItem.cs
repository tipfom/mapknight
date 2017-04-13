using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public abstract class UIItem : IDisposable {
        public event Action PositionChanged;

        public delegate void HandleItemClick( );
        public event HandleItemClick Click;
        public event HandleItemClick Release;
        public event HandleItemClick Leave;
        public bool Clicked { get { return (clickCount > 0); } }
        private bool multiClick;
        private int clickCount;

        private UIRectangle _Bounds;
        public UIRectangle Bounds { get { return _Bounds; } }
        private UIMargin horizontalMargin;
        private UIMargin verticalMargin;

        private Vector2 _Position;
        public Vector2 Position {
            get { return _Position; }
        }
        public readonly IUISize Size;

        private bool _Visible = true;
        public bool Visible { get { return Screen.IsActive && _Visible; } set { if(_Visible != value) IsDirty = true; _Visible = value; } }

        private int _Depth;
        public int Depth { get { return _Depth; } set { _Depth = value; IsDirty = true; } }

        protected bool IsDirty;

        public readonly Screen Screen;

        public UIItem(Screen owner, UIMargin hmargin, UIMargin vmargin, IUISize size, int depth, bool multiclick = false) {
            UIRenderer.Add(owner, this);
            Screen = owner;

            this.Size = size;
            this.multiClick = multiclick;
            this._Depth = depth;

            verticalMargin = vmargin;
            verticalMargin.Bind(this);
            verticalMargin.Changed += ( ) => IsDirty = true;

            horizontalMargin = hmargin;
            horizontalMargin.Bind(this);
            horizontalMargin.Changed += ( ) => IsDirty = true;

            Size.Changed += ( ) => IsDirty = true;

            _Position = new Vector2(hmargin.ScreenPosition, vmargin.ScreenPosition);
            _Bounds = new UIRectangle(_Position, Size.Size);
        }

        public virtual void HandleTouch(UITouchAction action, UITouch touch) {
            switch (action) {
                case UITouchAction.Begin:
                case UITouchAction.Enter:
                    if (!Clicked || multiClick) {
                        clickCount++;
                        Click?.Invoke( );
                    }
                    break;
                case UITouchAction.End:
                    if (Clicked) {
                        clickCount--;
                        if (!Clicked)
                            Release?.Invoke( );
                    }
                    break;
                case UITouchAction.Leave:
                    if (Clicked) {
                        clickCount--;
                        if (!Clicked)
                            Leave?.Invoke( );
                    }
                    break;
            }
        }

        public bool Collides(Vector2 touchPosition) {
            return Bounds.Collides(touchPosition);
        }

        public virtual void Update(DeltaTime dt) {
            if (IsDirty) {
                if (horizontalMargin.IsDirty || verticalMargin.IsDirty) {
                    _Position.X = horizontalMargin.ScreenPosition;
                    _Position.Y = verticalMargin.ScreenPosition;
                    _Bounds.Position = _Position;
                    _Bounds.Size = Size.Size;
                    PositionChanged?.Invoke( );
                }
                UIRenderer.Update(this);
                IsDirty = false;
            }
        }

        public virtual void Dispose( ) {
            UIRenderer.Remove(this);
        }

        public abstract IEnumerable<DepthVertexData> ConstructVertexData( );
    }
}