using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.GUI {
    public abstract class GUIItem {
        public enum Action {
            Move,
            Begin,
            End,
            Leave,
            Enter
        }

        public delegate void HandleUpdate (GUIItem sender);
        public static event HandleUpdate Changed;

        public fRectangle Bounds;

        public GUIItem (fRectangle bounds) {
            Bounds = bounds;
        }

        public virtual void HandleTouch (Action action) {

        }

        public bool Collides (fVector2D touchPosition) {
            return (this.Bounds.Left < touchPosition.X && this.Bounds.Right > touchPosition.X &&
                            this.Bounds.Top < touchPosition.Y && this.Bounds.Bottom > touchPosition.Y);
        }

        protected void RequestUpdate () {
            Changed?.Invoke (this);
        }

        public virtual void Update (float dt) {

        }

        public virtual List<VertexData> GetVertexData () {
            return new List<VertexData> ( );
        }

        public struct VertexData {
            public float[ ] Verticies;
            public string Texture;
            public Color Color;

            public VertexData (float[ ] verticies, string texture, Color color) : this ( ) {
                Verticies = verticies;
                Texture = texture;
                Color = color;
            }
        }
    }
}