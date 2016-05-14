using mapKnight.Android.CGL;
using mapKnight.Basic;

namespace mapKnight.Android {
    public static class Screen {
        public delegate void HandleScreenChange ();
        public static event HandleScreenChange Changed;

        public static Vector2 ScreenSize { get; private set; }
        public static float ScreenRatio { get; private set; }
        public static CGLMatrix DefaultMatrix { get; private set; }

        public static void Change (Size screensize) {
            ScreenSize = new Vector2 (screensize.Width, screensize.Height);
            ScreenRatio = ScreenSize.X / ScreenSize.Y;

            if (DefaultMatrix == null) {
                DefaultMatrix = new CGLMatrix ( );
            } else {
                DefaultMatrix.ResetView ( );
                DefaultMatrix.UpdateProjection ( );
                DefaultMatrix.CalculateMVP ( );
            }
            Changed?.Invoke ( );
        }

        public static Vector2 ToLocal (Vector2 globalPosition) {
            return new Vector2 (globalPosition.X / 2 / ScreenRatio + 0.5f, globalPosition.Y / 2 + 0.5f);
        }

        public static Vector2 ToGlobal (Vector2 localPosition) {
            return new Vector2 ((localPosition.X - 0.5f) * 2 * ScreenRatio, (localPosition.Y - 0.5f) * -2);
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