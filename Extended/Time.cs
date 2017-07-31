using mapKnight.Core;
using System;
using System.Diagnostics;

namespace mapKnight.Extended {
    public static class Time {
        public static float Scale = 1f;
        public static DeltaTime FrameTime;
        public static DeltaTime ScaledTime { get { return FrameTime * Scale; } }

#if DEBUG
        public static DeltaTime UpdateTime;
        public static DeltaTime DrawTime;
#endif

#if DEBUG
        private static Stopwatch stopwatch = new Stopwatch( );
#endif
        private static int lastUpdate = Environment.TickCount;

        public static void Update ( ) {
            FrameTime = new DeltaTime(Environment.TickCount - lastUpdate);
            lastUpdate = Environment.TickCount;

#if DEBUG
            stopwatch.Restart( );
#endif
        }

#if DEBUG
        public static void UpdateFinished ( ) {
            UpdateTime = new DeltaTime((float)stopwatch.Elapsed.TotalMilliseconds); stopwatch.Restart( );
        }

        public static void DrawFinished ( ) {
            DrawTime = new DeltaTime((float)stopwatch.Elapsed.TotalMilliseconds);
        }
#endif
    }
}
