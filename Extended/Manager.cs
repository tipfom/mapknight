using System;
using System.Diagnostics;
using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.Programs;
using mapKnight.Extended.Graphics.UI;
using OpenTK.Graphics.ES20;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Lightning;

namespace mapKnight.Extended {
    public static class Manager {
        public static void Initialize ( ) {
            ColorProgram.Init( );
            MatrixProgram.Init( );
            FBOProgram.Init( );
            ParticleProgram.Init( );
            GaussianBlurProgram.Init( );
            DarkenProgram.Init( );
            UIAbilityIconProgram.Init( );
            LineProgram.Init( );

            LightManager.Init( );
            UIRenderer.Init( );
            UIRenderer.Texture = Assets.Load<Spritebatch2D>("interface");

            Screen.MainMenu.Load( );
            Screen.Active = Screen.MainMenu;

            Window.Background = new Color(25, 25, 50, 255);
        }

        public static void Update ( ) {
            Time.Update( );
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            Screen.Active.Update(Time.FrameTime);
#if DEBUG
            Time.UpdateFinished( );
#endif

            Screen.Active.Draw( );

#if DEBUG
            Time.DrawFinished( );       
#endif 
        }

        public static void Destroy ( ) {
            Assets.Destroy( );
            Screen.MainMenu.Dispose( );
            Screen.Gameplay?.Dispose( );
            UIRenderer.Dispose( );
            LightManager.Destroy( );

            ColorProgram.Destroy( );
            MatrixProgram.Destroy( );
            FBOProgram.Destroy( );
            ParticleProgram.Destroy( );
            GaussianBlurProgram.Destroy( );
            DarkenProgram.Destroy( );
            UIAbilityIconProgram.Destroy( );
            LineProgram.Destroy( );
        }
    }
}
