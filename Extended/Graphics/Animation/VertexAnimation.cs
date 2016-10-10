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
        private int nextFrameTime = 0;
        [JsonIgnore]
        public bool IsRunning;

        [JsonIgnore]
        public float[ ][ ] Verticies;
        [JsonIgnore]
        public string[ ] Textures;

        public void Reset ( ) {
            nextFrameTime = Environment.TickCount + Frames[0].Time;
            currentFrame = 0;
            nextFrame = Math.Min(1, Frames.Length - 1);
            IsRunning = true;

            Verticies = new float[Frames[0].State.Length][ ];
            Textures = new string[Frames[0].State.Length];
        }

        public void Update (float dt, Transform ownerTransform, float vsize, float[ ][ ] verticies) {
            if (IsRunning && Environment.TickCount > nextFrameTime) {
                currentFrame = nextFrame;
                nextFrame++;
                nextFrameTime += Frames[currentFrame].Time;
                if (nextFrame >= Frames.Length) {
                    IsRunning = false;
                    if (CanRepeat) nextFrame = 0;
                    else nextFrame = currentFrame;
                }
            }
            float progress = Mathf.Clamp01((nextFrameTime - Environment.TickCount) / (float)Frames[currentFrame].Time);

            for (int i = 0; i < Verticies.Length; i++) {
                Vector2 interpolatedPosition = Mathf.Interpolate(Frames[nextFrame].State[i].Position, Frames[currentFrame].State[i].Position, progress) * ownerTransform.Size * vsize;
                float interpolatedRotation = Mathf.Interpolate(Frames[nextFrame].State[i].Rotation, Frames[currentFrame].State[i].Rotation, progress);

                Verticies[i] = verticies[i];
                Verticies[i] = Mathf.TransformAtOrigin(
                    verticies[i],
                    interpolatedPosition.X, interpolatedPosition.Y,
                    interpolatedRotation, Frames[currentFrame].State[i].Mirrored, ownerTransform.Size * vsize);
                Textures[i] = Frames[currentFrame].State[i].Texture;
            }
        }
    }
}
