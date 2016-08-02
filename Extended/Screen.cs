using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Screens;

namespace mapKnight.Extended {
    public class Screen : IDisposable {
        private static Screen _Active;
        public static Screen Active { get { return _Active; } set { _Active.IsActive = false; value.IsActive = true; _Active = value; UIRenderer.Prepare(value); value.Activated( ); } }

        public static GameplayScreen Gameplay;
        public static MainMenuScreen MainMenu;

        public bool IsActive { get; private set; }

        static Screen ( ) {
            Gameplay = new GameplayScreen( );
            MainMenu = new MainMenuScreen( );
            _Active = MainMenu;
        }

        protected virtual void Activated ( ) {

        }

        public virtual void Load ( ) {

        }

        public virtual void Update (TimeSpan dt) {
            UIRenderer.Update(dt);
        }

        public virtual void Tick ( ) {

        }

        public virtual void Draw ( ) {
            UIRenderer.Draw( );
        }

        public virtual void Dispose ( ) {
            UIRenderer.Delete(this);
        }
    }
}
