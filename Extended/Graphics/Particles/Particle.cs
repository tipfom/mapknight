using System;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.Particles {
    public class Particle {
        public Vector2 Position;
        public Vector2 Velocity;
        public int Lifetime;
        public float Size;
        public Color Color;

        public Particle (Emitter emitter) {
            Setup(emitter);
        }

        public void Setup (Emitter emitter) {
            Position = emitter.Position;
            Velocity = emitter.VelocityProvider.GetVelocity( );
            float lifetimernd = Mathf.Random( );
            Lifetime = emitter.Lifetime.Min + (int)(lifetimernd * lifetimernd * (emitter.Lifetime.Max - emitter.Lifetime.Min));
            Size = (int)Mathf.Random(emitter.Size.Min, emitter.Size.Max);
            Color = new Color(emitter.Color);
        }

        public bool Update (DeltaTime dt, Vector2 gravity) {
            Velocity += gravity * dt.TotalSeconds;
            Position += Velocity * dt.TotalSeconds;
            return (Lifetime -= (int)dt.TotalMilliseconds) < 0;
        }
    }
}
