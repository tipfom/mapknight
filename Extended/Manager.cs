using System;
using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.GUI;
using mapKnight.Extended.Graphics.Programs;
using mapKnight.Extended.Graphics.Screens;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended {
    public static class Manager {
        public static void Initialize ( ) {
            ColorProgram.Program = new ColorProgram( );
            MatrixProgram.Program = new MatrixProgram( );

            GUIRenderer.Texture = Assets.Load<SpriteBatch>("interface");
            Assets.Load<Texture2D>("testMap");
            Assets.Load<SpriteBatch>("potatoe_patrick");

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
            frameTime = new TimeSpan(0, 0, 0, 0, Environment.TickCount - lastUpdate);
            lastUpdate = Environment.TickCount;
        }
    }
}
