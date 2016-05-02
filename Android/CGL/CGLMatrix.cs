using Android.Opengl;

namespace mapKnight.Android.CGL {
    public class CGLMatrix {
        public float[ ] View { get; set; }
        public float[ ] Projection { get; private set; }
        public float[ ] MVP { get; private set; }

        public CGLMatrix () {
            View = new float[16];
            Projection = new float[16];
            MVP = new float[16];

            ResetView ( );
            UpdateProjection ( );
            CalculateMVP ( );

            Screen.Changed += () => {
                UpdateProjection ( );
                CalculateMVP ( );
            };
        }

        public void UpdateProjection () {
            Matrix.OrthoM (Projection, 0, -Screen.ScreenRatio, Screen.ScreenRatio, -1, 1, 3, 7);
        }

        public void CalculateMVP () {
            Matrix.MultiplyMM (MVP, 0, Projection, 0, View, 0);
        }

        public void ResetView () {
            Matrix.SetLookAtM (View, 0, 0, 0, 3, 0f, 0f, 0f, 0f, 1f, 0f);
        }
    }
}
