using mapKnight.Graphics.Programs;
using mapKnight.Graphics;
using mapKnight.Graphics.GUI;
using System;
using OpenTK.Graphics.ES20;
using mapKnight.Graphics.Screens;
using mapKnight.Core;

namespace mapKnight.Graphics {
    public static class Manager {
        public static void Initialize ( ) {
            ColorProgram.Program = new ColorProgram( );
            MatrixProgram.Program = new MatrixProgram( );

            GUIRenderer.Texture = Assets.Load<SpriteBatch>("interface");

            GL.ClearColor(0f, 0f, 0f, 1f);

            Screen.Current = new MainMenuScreen( );
            Screen.Current.Active = true;
        }

        private static TimeSpan frameTime;
        private static int drawTime;
        private static int updateTime;

        public static void Update ( ) {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UpdateFrametime( );
            int updateBegin = Environment.TickCount;

            Screen.Current.Update(frameTime);
            updateTime = Environment.TickCount - updateBegin;

            GUIRenderer.Draw( );
            Screen.Current.Draw( );
            drawTime = Environment.TickCount - updateTime - updateBegin;
        }

        private static int lastUpdate = Environment.TickCount;
        private static void UpdateFrametime ( ) {
            frameTime = new TimeSpan(Environment.TickCount - lastUpdate);
            lastUpdate = Environment.TickCount;
        }
    }
}
