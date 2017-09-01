using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.Particles.VelocityProvider {
    public class ConstantVelocityProvider : IVelocityProvider {
        public Vector2 Velocity;

        public ConstantVelocityProvider(Vector2 Velocity) {
            this.Velocity = Velocity;
        }

        public Vector2 GetVelocity ( ) {
            return Velocity;
        }
    }
}
