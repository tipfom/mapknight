using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.Particles.VelocityProvider {
    public class PolarVelocityProvider : IVelocityProvider {
        private Vector2 magnitude;
        private Vector2 angle;

        public PolarVelocityProvider (float magnitudemin, float magnitudemax, float anglemin = 0, float anglemax = 360) {
            magnitude = new Vector2(magnitudemin, magnitudemax);
            angle = new Vector2(anglemin, anglemax);
        }

        public Vector2 GetVelocity ( ) {
            float a = Mathf.Random(angle.X, angle.Y);
            float m = Mathf.Random(magnitude.X, magnitude.Y);
            return new Vector2(Mathf.Cos(a) * m, Mathf.Sin(a) * m);
        }
    }
}
