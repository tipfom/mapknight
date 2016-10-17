using System;
using System.Diagnostics;
using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.Programs;
using mapKnight.Extended.Graphics.UI;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended {
    public static class Manager {
        public const int TICKS_PER_SECOND = 1;

        public static void Initialize ( ) {
            int begin = Environment.TickCount;

            ColorProgram.Program = new ColorProgram( );
            MatrixProgram.Program = new MatrixProgram( );
            FBOProgram.Program = new FBOProgram( );
            ParticleProgram.Program = new ParticleProgram( );

            UIRenderer.Texture = Assets.Load<SpriteBatch>("interface");

            Screen.Gameplay.Load( );
            Screen.MainMenu.Load( );
            Screen.Active = Screen.MainMenu;
            emitter = new Graphics.Particles.Emitter( );
            emitter.Color = new Range<Color>(Color.Yellow, Color.Red);
            emitter.Count = 200;
            emitter.Gravity = new Vector2(0f, 0.1f);
            emitter.Position = new Vector2(0f, 0f);
            emitter.Size = new Range<float>(5f, 10f);
            emitter.Velocity = new Range<Vector2>(new Vector2(-0.01f, -0.01f), new Vector2(0.01f, 0f));
            emitter.Lifetime = new Range<int>(500, 2000);
            emitter.Setup( );

            GL.ClearColor(0f, 0f, 0f, 0f);

            Debug.Print(typeof(Manager), $"Loading took {Environment.TickCount - begin} ms");
        }

        private static Stopwatch stopWatch = new Stopwatch( );
        public static DeltaTime FrameTime { get; private set; }
        public static DeltaTime DrawTime { get; private set; }
        public static DeltaTime UpdateTime { get; private set; }

        static Graphics.Particles.Emitter emitter;

        public static void Update ( ) {
            stopWatch.Restart( );
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            UpdateFrametime( );

            Screen.Active.Update(FrameTime);
            emitter.Update(FrameTime);
            UpdateTime = new DeltaTime((float)stopWatch.Elapsed.TotalMilliseconds); stopWatch.Restart( );

            Screen.Active.Draw( );
            emitter.Draw( );

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
