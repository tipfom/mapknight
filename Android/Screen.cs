using mapKnight.Android.CGL;
using mapKnight.Basic;

namespace mapKnight.Android {
    public static class Screen {
        public delegate void HandleScreenChange ();
        public static event HandleScreenChange Changed;

        public static fVector2D ScreenSize { get; private set; }
        public static float ScreenRatio { get; private set; }
        public static CGLMatrix DefaultMatrix { get; private set; }

        public static void Change (Size screensize) {
            ScreenSize = new fVector2D (screensize.Width, screensize.Height);
            ScreenRatio = ScreenSize.X / ScreenSize.Y;

            if (DefaultMatrix == null)
                DefaultMatrix = new CGLMatrix ( );
            Changed?.Invoke ( );
        }

        public static fVector2D ToLocal (fVector2D globalPosition) {
            return new fVector2D (globalPosition.X / 2 / ScreenRatio + 0.5f, globalPosition.Y / 2 + 0.5f);
        }

        public static fVector2D ToGlobal (fVector2D localPosition) {
            return new fVector2D ((localPosition.X - 0.5f) * 2 * ScreenRatio, (localPosition.Y - 0.5f) * -2);
        }

        public static float[ ] ToGlobal (float[ ] localPositions) {
            float[ ] globalPosition = new float[localPositions.Length];
            for (int i = 0; i < localPositions.Length / 2; i++) {
                globalPosition[2 * i + 0] = (localPositions[2 * i + 0] - 0.5f) * 2 * ScreenRatio; // x
                globalPosition[2 * i + 1] = (localPositions[2 * i + 1] - 0.5f) * -2; // y
            }
            return globalPosition;
        }
    }
}