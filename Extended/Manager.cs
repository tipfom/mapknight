using System;
using System.Diagnostics;
using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.Programs;
using mapKnight.Extended.Graphics.UI;
using OpenTK.Graphics.ES20;
using mapKnight.Core.Graphics;

namespace mapKnight.Extended {
    public static class Manager {
        public const int TICKS_PER_SECOND = 1;

        public static void Initialize ( ) {
            int begin = Environment.TickCount;

            ColorProgram.Program = new ColorProgram( );
            MatrixProgram.Program = new MatrixProgram( );
            FBOProgram.Program = new FBOProgram( );
            ParticleProgram.Program = new ParticleProgram( );
            GaussianBlurProgram.Program = new GaussianBlurProgram( );

            UIRenderer.Init( );
            UIRenderer.Texture = Assets.Load<Spritebatch2D>("interface");

            Screen.MainMenu.Load( );
            Screen.Active = Screen.MainMenu;

            Window.Background = new Color(25, 25, 50, 255);

#if DEBUG
            Debug.Print(typeof(Manager), $"Loading took {Environment.TickCount - begin} ms");
#endif
        }

        private static Stopwatch stopWatch = new Stopwatch( );
        public static DeltaTime FrameTime { get; private set; }
        public static DeltaTime DrawTime { get; private set; }
        public static DeltaTime UpdateTime { get; private set; }

        public static void Update ( ) {
            stopWatch.Restart( );
            GL.Clear(ClearBufferMask.ColorBufferBit);
            UpdateFrametime( );

            Screen.Active.Update(FrameTime);
            UpdateTime = new DeltaTime((float)stopWatch.Elapsed.TotalMilliseconds); stopWatch.Restart( );

            Screen.Active.Draw( );

            DrawTime = new DeltaTime((float)stopWatch.Elapsed.TotalMilliseconds);
        }

        public static void Destroy ( ) {
            Assets.Destroy( );
            Screen.MainMenu.Dispose( );
            Screen.Gameplay?.Dispose( );
            UIRenderer.Dispose( );
        }

        private static int lastUpdate = Environment.TickCount;
        private static void UpdateFrametime ( ) {
            FrameTime = new DeltaTime(Environment.TickCount - lastUpdate);
            lastUpdate = Environment.TickCount;
        }
    }
}
