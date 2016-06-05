using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using MatrixMath = Android.Opengl.Matrix;

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
            _Projection = Matrix4.CreateOrthographicOffCenter(-Window.Ratio, Window.Ratio, -1, 1, 3, 7);
        }

        public void CalculateMVP ( ) {
            _MVP = Matrix4.Mult(_View, _Projection);
        }

        public void ResetView ( ) {
            _View = Matrix4.LookAt(0, 0, 3, 0f, 0f, 0f, 0f, 1f, 0f);
        }

        public static Matrix Default { get; } = new Matrix( );
    }
}
