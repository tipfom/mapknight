using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public abstract class GUIItem {
        public delegate void HandleUpdate (GUIItem sender);
        public static event HandleUpdate Changed;
        public delegate void HandleItemClick ();
        public event HandleItemClick Click;
        public event HandleItemClick Release;
        public bool Clicked { get { return (clickCount > 0); } }
        private bool multiClick;
        private int clickCount;

        public fRectangle Bounds;
        public fVector2D Position { get { return Bounds.Position; } set { Bounds.Position = value; RequestUpdate ( ); } }
        public fVector2D Size { get { return Bounds.Size; } set { Bounds.Size = value; RequestUpdate ( ); } }

        private bool _Visible = true;
        public bool Visible { get { return _Visible; } set { _Visible = value; RequestUpdate ( ); } }

        public GUIItem (fRectangle bounds, bool multiclick = false) {
            this.Bounds = bounds;
            this.multiClick = multiclick;
        }

        public virtual void HandleTouch (GUITouchHandler.Touch.Action action) {
            switch (action) {
            case GUITouchHandler.Touch.Action.Begin:
            case GUITouchHandler.Touch.Action.Enter:
                if (!Clicked || multiClick) {
                    clickCount++;
                    Click?.Invoke ( );
                }
                break;
            case GUITouchHandler.Touch.Action.End:
            case GUITouchHandler.Touch.Action.Leave:
                if (Clicked) {
                    clickCount--;
                    if (!Clicked)
                        Release?.Invoke ( );
                }
                break;
            }
        }

        public bool Collides (fVector2D touchPosition) {
            return (this.Bounds.Left < touchPosition.X && this.Bounds.Right > touchPosition.X &&
                            this.Bounds.Top < touchPosition.Y && this.Bounds.Bottom > touchPosition.Y);
        }

        protected void RequestUpdate () {
            if (this.Visible)
                Changed?.Invoke (this);
        }

        public virtual void Update (float dt) {

        }

        public virtual List<CGLVertexData> GetVertexData () {
            return new List<CGLVertexData> ( );
        }
    }
}