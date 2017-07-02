using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using static mapKnight.Extended.Graphics.Programs.ParticleProgram;

namespace mapKnight.Extended.Graphics.Particles {
    public class Emitter {
        public static Matrix Matrix;

        private Particle[ ] particles;
        private BufferBatch batch;
        private ClientBuffer vertexbuffer;
        private ClientBuffer sizebuffer;
        private ClientBuffer colorbuffer;

        public Vector2 Position;

        public Range<int> Lifetime;
        public Range<float> Size;
        public Range<Color> Color;
        public Range<Vector2> Velocity;
        public Vector2 Gravity;
        public int Count;

        public Emitter ( ) {
        }

        public void Update (DeltaTime dt) {
            for (int i = 0; i < Count; i++) {
                if (particles[i].Update(dt, Gravity)) {
                    particles[i].Setup(this);
                    UpdateParticle(i);
                }
                vertexbuffer.Data[i * 2] = particles[i].Position.X;
                vertexbuffer.Data[i * 2 + 1] = particles[i].Position.Y;
            }
        }

        public void Draw ( ) {
            Program.Begin( );
            Program.Draw(batch, true);
            Program.End( );
        }

        private void UpdateParticle(int index) {
            sizebuffer.Data[index] = particles[index].Size;
            colorbuffer.Data[index * 4] = particles[index].Color.R;
            colorbuffer.Data[index * 4 + 1] = particles[index].Color.G;
            colorbuffer.Data[index * 4 + 2] = particles[index].Color.B;
            colorbuffer.Data[index * 4 + 3] = particles[index].Color.A;
        }

        public void Setup ( ) {
            if (particles != null) return;

            vertexbuffer = new ClientBuffer(2, Count, PrimitiveType.Point);
            sizebuffer = new ClientBuffer(1, Count, PrimitiveType.Point);
            colorbuffer = new ClientBuffer(4, Count, PrimitiveType.Point);
            batch = new BufferBatch(vertexbuffer, sizebuffer, colorbuffer);

            particles = new Particle[Count];
            for (int i = 0; i < Count; i++) {
                particles[i] = new Particle(this);
                UpdateParticle(i);
            }
        }
    }
}
