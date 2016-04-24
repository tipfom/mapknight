using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public abstract class GUIElement {
        public fVector2D Position;
        public fVector2D Size;

        public virtual void HandleTouchBegin () {

        }

        public virtual void HandleTouchMoved () {

        }

        public virtual void HandleTouchLeave () {

        }

        public virtual void HandleTouchEnd () {

        }

        public bool Collides (fVector2D touchPosition) {
            return (this.Position.X < touchPosition.X && this.Position.X + this.Size.X > touchPosition.X &&
                            this.Position.Y > touchPosition.Y && this.Position.Y - this.Size.Y < touchPosition.Y);
        }
    }
}