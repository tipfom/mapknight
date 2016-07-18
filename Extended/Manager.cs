using System;
using System.Diagnostics;
using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.Programs;
using mapKnight.Extended.Graphics.UI;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended {
    public static class Manager {
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
        public static TimeSpan FrameTime { get; private set; }
        public static TimeSpan DrawTime { get; private set; }
        public static TimeSpan UpdateTime { get; private set; }

        public static void Update ( ) {
            stopWatch.Restart( );
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateFrametime( );

            Screen.Active.Update(FrameTime);
            UpdateTime = stopWatch.Elapsed; stopWatch.Restart( );

            Screen.Active.Draw( );
            DrawTime = stopWatch.Elapsed;

            Debug.Print(typeof(Manager), $"draw {DrawTime.TotalMilliseconds:0.000} update {UpdateTime.TotalMilliseconds:0.000} frame {FrameTime.TotalMilliseconds:00.000}");
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
