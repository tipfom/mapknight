using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.Particles.VelocityProvider {
    public class CartesianVelocityProvider : IVelocityProvider {
        private Vector2 x;
        private Vector2 y;

        public CartesianVelocityProvider (float xmin, float xmax, float ymin, float ymax) {
            x = new Vector2(xmin, xmax);
            y = new Vector2(ymin, ymax);
        }

        public Vector2 GetVelocity ( ) {
            return new Vector2(Mathf.Random(x.X, x.Y), Mathf.Random(y.X, y.Y));
        }
    }
}
