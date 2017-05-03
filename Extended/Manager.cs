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
        public const int TICKS_PER_SECOND = 1;

        public static void Initialize ( ) {
            int begin = Environment.TickCount;

            ColorProgram.Init( );
            MatrixProgram.Init( );
            FBOProgram.Init( );
            ParticleProgram.Init( );
            GaussianBlurProgram.Init( );

            LightManager.Init( );
            UIRenderer.Init( );
            UIRenderer.Texture = Assets.Load<Spritebatch2D>("interface");

            Screen.MainMenu.Load( );
            Screen.Active = Screen.MainMenu;

            Window.Background = new Color(25, 25, 50, 255);

            lm = new LightManager(.5f);
            lm.Add(new Light( ) { Intensity = .5f, Position = new Vector2(-.1f, .1f), Radius = .6f, Color = new Color(255, 0, 0, 128) });
            lm.Add(new Light( ) { Intensity = .5f, Position = new Vector2(.0f, -.1f), Radius = .6f, Color = new Color(0, 0, 255, 128) });
            lm.Add(new Light( ) { Intensity = .5f, Position = new Vector2(.1f, .1f), Radius = .6f, Color = new Color(0,255, 0, 255) });
#if DEBUG
            Debug.Print(typeof(Manager), $"Loading took {Environment.TickCount - begin} ms");
#endif
        }

        private static Stopwatch stopWatch = new Stopwatch( );
        public static DeltaTime FrameTime { get; private set; }
        public static DeltaTime DrawTime { get; private set; }
        public static DeltaTime UpdateTime { get; private set; }

        static LightManager lm;

        public static void Update ( ) {
            stopWatch.Restart( );
            GL.Clear(ClearBufferMask.ColorBufferBit);
            UpdateFrametime( );

            Screen.Active.Update(FrameTime);
            UpdateTime = new DeltaTime((float)stopWatch.Elapsed.TotalMilliseconds); stopWatch.Restart( );

            Screen.Active.Draw( );
            lm.Draw( );
            lm.Brightness = (Environment.TickCount % 10000) / 5000f;
            lm.Brightness = lm.Brightness > 1 ? 2 - lm.Brightness : lm.Brightness;


            DrawTime = new DeltaTime((float)stopWatch.Elapsed.TotalMilliseconds);
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
        }

        private static int lastUpdate = Environment.TickCount;
        private static void UpdateFrametime ( ) {
            FrameTime = new DeltaTime(Environment.TickCount - lastUpdate);
            lastUpdate = Environment.TickCount;
        }
    }
}
