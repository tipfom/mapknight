using System;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Screens;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended {

    public class Screen : IDisposable {
        private static Screen _Active;
        public static Screen Active { get { return _Active; } set { UIRenderer.Prepare(value); _Active.IsActive = false; value.IsActive = true; value.Activated( ); _Active = value; } }

        public static MainMenuScreen MainMenu;
        public static GameplayScreen Gameplay;

        static Screen ( ) {
            MainMenu = new MainMenuScreen( );
            _Active = MainMenu;
        }

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