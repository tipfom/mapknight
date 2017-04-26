using System;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Screens;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended {

    public class Screen : IDisposable {
        public static MainMenuScreen MainMenu;
        public static GameplayScreen Gameplay;
        private static Screen _Active;

        static Screen ( ) {
            MainMenu = new MainMenuScreen( );
            _Active = MainMenu;
        }

        public Vector2 Size { get { return new Vector2(2 * Window.Ratio, 2); } }
        public Vector2 Position {
            get { return new Vector2(-Window.Ratio, 1); }
        }

        public static Screen Active { get { return _Active; } set { UIRenderer.Prepare(value); _Active.IsActive = false; value.IsActive = true; value.Activated( ); _Active = value; } }
        public bool IsActive { get; private set; }

        public virtual void Dispose ( ) {
            UIRenderer.Delete(this);
        }

        public virtual void Draw ( ) {
            UIRenderer.Draw( );
        }

        public virtual void Load ( ) {
        }

        public virtual void Update (DeltaTime dt) {
            UIRenderer.Update(dt);
        }

        protected virtual void Activated ( ) {
        }
    }
}