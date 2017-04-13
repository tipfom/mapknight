using System;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Screens;

namespace mapKnight.Extended {

    public class Screen : IDisposable {
        public static MainMenuScreen MainMenu;
        public static GameplayScreen Gameplay;
        private static Screen _Active;

        static Screen ( ) {
            MainMenu = new MainMenuScreen( );
            _Active = MainMenu;
        }

        public static Screen Active { get { return _Active; } set { _Active.IsActive = false; value.IsActive = true; UIRenderer.Prepare(value); value.Activated( ); _Active = value; } }
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