using System;
using System.Diagnostics;
using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.GUI;
using mapKnight.Extended.Graphics.Programs;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended {
    public static class Manager {
        public static void Initialize ( ) {
            stopWatch = new Stopwatch( );
            ColorProgram.Program = new ColorProgram( );
            MatrixProgram.Program = new MatrixProgram( );

            GUIRenderer.Texture = Assets.Load<SpriteBatch>("interface");

            Screen.Gameplay.Load( );
            Screen.MainMenu.Load( );
            Screen.Active = Screen.MainMenu;

            GL.ClearColor(0f, 0f, 0f, 1f);
        }

        private static Stopwatch stopWatch;
        private static TimeSpan frameTime;
        private static double drawTime;
        private static double updateTime;

        public static void Update ( ) {
            stopWatch.Restart( );
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateFrametime( );

            Screen.Active.Update(frameTime);
            updateTime = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Restart( );

            Screen.Active.Draw( );
            drawTime = stopWatch.Elapsed.TotalMilliseconds;

            Log.Print(typeof(Manager), $"draw {drawTime:0.000} update {updateTime:0.000} frame {frameTime.TotalMilliseconds:00.000}");
        }

        public static void Destroy ( ) {
            Assets.Destroy( );
        }

        private static int lastUpdate = Environment.TickCount;
        private static void UpdateFrametime ( ) {
            frameTime = new TimeSpan(0, 0, 0, 0, Environment.TickCount - lastUpdate);
            lastUpdate = Environment.TickCount;
        }
    }
}
