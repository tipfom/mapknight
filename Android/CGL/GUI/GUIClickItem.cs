using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public class GUIClickItem : GUIItem {
        public delegate void HandleClickElement ();
        public event HandleClickElement Click;
        public event HandleClickElement Release;
        public bool Clicked { get { return (clickCount > 0); } }
        private bool multiClick;
        private int clickCount;

        public fVector2D Position { get { return Bounds.Position; } set { Bounds.Position = value; RequestUpdate ( ); } }
        public fVector2D Size { get { return Bounds.Size; } set { Bounds.Size = value; RequestUpdate ( ); } }

        public GUIClickItem (fRectangle bounds, bool multiclick = false) : base (bounds) {
            multiClick = multiclick;
        }

        public override sealed void HandleTouch (Action action) {
            switch (action) {
            case Action.Begin:
            case Action.Enter:
                if (!Clicked || multiClick) {
                    clickCount++;
                    Click?.Invoke ( );
                }
                break;
            case Action.End:
            case Action.Leave:
                if (Clicked) {
                    clickCount--;
                    if (!Clicked)
                        Release?.Invoke ( );
                }
                break;
            }
        }
    }
}