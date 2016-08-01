using OpenTK;

namespace mapKnight.Extended.Graphics {
    public class Matrix {
        private Matrix4 _View;
        public Matrix4 View { get { return _View; } set { _View = value; } }

        private Matrix4 _Projection;
        public Matrix4 Projection { get { return _Projection; } }

        private Matrix4 _MVP;
        public Matrix4 MVP { get { return _MVP; } }

        public Matrix ( ) {
            ResetView( );
            UpdateProjection( );
            CalculateMVP( );
            Window.Changed += ( ) => {
                UpdateProjection( );
                CalculateMVP( );
            };
        }

        public void UpdateProjection ( ) {
            _Projection = Matrix4.CreateOrthographicOffCenter(-Window.Ratio, Window.Ratio, -1, 1, -1, 3);
        }

        public void CalculateMVP ( ) {
            _MVP = Matrix4.Mult(_View, _Projection);
        }

        public void ResetView ( ) {
            _View = Matrix4.LookAt(0, 0, 3, 0f, 0f, 0f, 0f, 1f, 0f);
        }

        public void TranslateView (float x, float y, float z) {
            _View = Matrix4.Mult(_View, Matrix4.CreateTranslation(new Vector3(x, y, z)));
        }

        public static Matrix Default { get; } = new Matrix( );
    }
}
