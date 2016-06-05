using mapKnight.Extended.Graphics.GUI;
using System;

namespace mapKnight.Extended.Graphics {
    public abstract class Screen {
        #region delegates
        public delegate void HandleScreenChange (Screen oldScreen, Screen newScreen);
        public delegate void HandleActivationChange ( );
        #endregion

        #region static
        public static event HandleScreenChange Changed;
        public static Screen Current;
        #endregion

        public event HandleActivationChange ActivationChanged;
        private bool _Active = false;
        public bool Active {
            get { return _Active; }
            set {
                if (_Active != value) {
                    _Active = value;
                    ActivationChanged?.Invoke( );
                    if (Active)
                        Activated( );
                }
            }
        }

        protected GUICollection Interface;

        public Screen ( ) {
            Interface = new GUICollection(this);
        }

        protected virtual void Activated ( ) {

        }

        public virtual void Draw ( ) {

        }

        public virtual void Update (TimeSpan time) {

        }

        protected void Switch (Screen nextScreen) {
            this.Active = false;
            nextScreen.Active = true;
            Current = nextScreen;
            Changed?.Invoke(this, nextScreen);
        }
    }
}
