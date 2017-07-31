using System;
using mapKnight.Core;
using Newtonsoft.Json;

namespace mapKnight.Extended.Graphics.Animation {
    public class VertexAnimation {
        public string Name;
        public bool CanRepeat;
        public VertexAnimationFrame[ ] Frames;

        private int currentFrame = 0;
        private int nextFrame = 0;
        private float frameTimeLeft = 0;
        [JsonIgnore]
        public bool IsRunning;

        [JsonIgnore]
        public float[ ][ ] Verticies;
        [JsonIgnore]
        public string[ ] Textures;

        public void Reset ( ) {
            frameTimeLeft = Frames[0].Time;
            currentFrame = 0;
            nextFrame = Math.Min(1, Frames.Length - 1);
            IsRunning = true;

            Verticies = new float[Frames[0].State.Length][ ];
            for(int i = 0; i < Verticies.Length; i++) {
                Verticies[i] = new float[8];
            }
            Textures = new string[Frames[0].State.Length];
            for(int i = 0;  i < Textures.Length; i++) {
                Textures[i] = Frames[0].State[i].Texture;
            }
        }

        public void Update (float dt, Transform ownerTransform, float vsize, float[ ][ ] verticies, int offset = 0) {
            frameTimeLeft -= dt;

            if (IsRunning && frameTimeLeft <= 0) {
                currentFrame = nextFrame;
                nextFrame++;
                frameTimeLeft += Frames[currentFrame].Time;
                if (nextFrame >= Frames.Length) {
                    IsRunning = false;
                    if (CanRepeat) nextFrame = 0;
                    else nextFrame = currentFrame;
                }
            }
            float progress = Mathf.Clamp01(frameTimeLeft / Frames[currentFrame].Time);

            for (int i = 0; i < Verticies.Length; i++) {
                Vector2 interpolatedPosition = Mathf.Interpolate(Frames[nextFrame].State[i].Position, Frames[currentFrame].State[i].Position, progress) * ownerTransform.Size * vsize;
                float interpolatedRotation = Mathf.Interpolate(Frames[nextFrame].State[i].Rotation, Frames[currentFrame].State[i].Rotation, progress);

                Mathf.TransformAtOrigin(
                    verticies[i + offset], ref Verticies[i],
                    interpolatedPosition.X, interpolatedPosition.Y,
                    interpolatedRotation, Frames[currentFrame].State[i].Mirrored, ownerTransform.Size * vsize);
                Textures[i] = Frames[currentFrame].State[i].Texture;
            }
        }
    }
}
