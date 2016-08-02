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

        private static float timeBetweenTicks = 1f / TICKS_PER_SECOND;
        private static float nextTick = Environment.TickCount + timeBetweenTicks;
        private static Stopwatch stopWatch = new Stopwatch( );
        public static TimeSpan FrameTime { get; private set; }
        public static TimeSpan DrawTime { get; private set; }
        public static TimeSpan UpdateTime { get; private set; }

        public static void Update ( ) {
            stopWatch.Restart( );
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateFrametime( );

            if (Environment.TickCount > nextTick) {
                nextTick += timeBetweenTicks;
                Screen.Active.Tick( );
            }

            Screen.Active.Update(FrameTime);
            UpdateTime = stopWatch.Elapsed; stopWatch.Restart( );

            Screen.Active.Draw( );
            DrawTime = stopWatch.Elapsed;
        }

        public static void Destroy ( ) {
            Assets.Destroy( );
        }

        private static int lastUpdate = Environment.TickCount;
        private static void UpdateFrametime ( ) {
            FrameTime = new TimeSpan(0, 0, 0, 0, Environment.TickCount - lastUpdate);
            lastUpdate = Environment.TickCount;
        }
    }
}
