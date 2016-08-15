using System;
using System.Diagnostics;
using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.Programs;
using mapKnight.Extended.Graphics.UI;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended {
    public static class Manager {
        public const int TICKS_PER_SECOND = 8;

        public static void Initialize ( ) {
            ColorProgram.Program = new ColorProgram( );
            MatrixProgram.Program = new MatrixProgram( );

            UIRenderer.Texture = Assets.Load<SpriteBatch>("interface");

            Screen.Gameplay.Load( );
            Screen.MainMenu.Load( );
            Screen.Active = Screen.MainMenu;

            GL.ClearColor(0f, 0f, 0f, 1f);
        }

        private static Stopwatch stopWatch = new Stopwatch( );
        public static DeltaTime FrameTime { get; private set; }
        public static DeltaTime DrawTime { get; private set; }
        public static DeltaTime UpdateTime { get; private set; }

        public static void Update ( ) {
            stopWatch.Restart( );
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateFrametime( );

            Screen.Active.Update(FrameTime);
            UpdateTime = new DeltaTime((float)stopWatch.Elapsed.TotalMilliseconds); stopWatch.Restart( );

            Screen.Active.Draw( );
            DrawTime = new DeltaTime((float)stopWatch.Elapsed.TotalMilliseconds);
        }

        public static void Destroy ( ) {
            Assets.Destroy( );
        }

        private static int lastUpdate = Environment.TickCount;
        private static void UpdateFrametime ( ) {
            FrameTime = new DeltaTime(Environment.TickCount - lastUpdate);
            lastUpdate = Environment.TickCount;
        }
    }
}
