using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public abstract class UIItem : IDisposable {
        public delegate void HandleItemClick( );
        public event HandleItemClick Click;
        public event HandleItemClick Release;
        public event HandleItemClick Leave;
        public bool Clicked { get { return (clickCount > 0); } }
        private bool multiClick;
        private int clickCount;

        public readonly UILayout Layout;

        private bool _Visible = true;
        public bool Visible { get { return Screen.IsActive && _Visible; } set { if(_Visible != value) IsDirty = true; _Visible = value; } }

        private int _Depth;
        public int Depth { get { return _Depth; } set { _Depth = value; IsDirty = true; } }

        protected bool IsDirty;

        public readonly Screen Screen;

        public UIItem(Screen owner, UILayout layout, int depth, bool multiclick = false) {
            Screen = owner;

            this.multiClick = multiclick;
            this._Depth = depth;
            this.Layout = layout;
            UIRenderer.Add(owner, this);

            Layout.Initialize(this);
            Layout.UpdateRequired += ( ) => IsDirty = true;
        }

        public virtual bool HandleTouch(UITouchAction action, UITouch touch) {
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
            return true;
        }

        public bool Collides(Vector2 touchPosition) {
            return Layout.Rectangle.Collides(touchPosition);
        }

        public virtual void Update(DeltaTime dt) {
            if (IsDirty) {
                if (Layout.IsDirty) {
                    Layout.Refresh( );
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