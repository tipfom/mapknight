using System;
using Newtonsoft.Json;

namespace mapKnight.Core.Graphics {
    public class VertexAnimation {
        public string Name;
        public bool CanRepeat;
        public VertexAnimationFrame[ ] Frames;

        private int currentFrame = 0;
        private int nextFrame = 0;
        private int nextFrameTime = 0;
        [JsonIgnore]
        public bool IsRunning;

        public void Reset ( ) {
            nextFrameTime = Environment.TickCount + Frames[0].Time;
            currentFrame = 0;
            nextFrame = Math.Min(1, Frames.Length - 1);
            IsRunning = true;
        }

        public float[ ][ ] Update (float dt, Transform ownerTransform, float vsize) {
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

            float[ ][ ] result = new float[Frames[currentFrame].State.Length][ ];

            for (int i = 0; i < result.Length; i++) {
                Vector2 interpolatedSize = Mathf.Interpolate(Frames[nextFrame].State[i].Size, Frames[currentFrame].State[i].Size, progress) * ownerTransform.Size * vsize;
                Vector2 interpolatedPosition = Mathf.Interpolate(Frames[nextFrame].State[i].Position, Frames[currentFrame].State[i].Position, progress) * ownerTransform.Size * vsize;
                float interpolatedRotation = Mathf.Interpolate(Frames[nextFrame].State[i].Rotation, Frames[currentFrame].State[i].Rotation, progress);

                result[i] = Mathf.TransformAtOrigin(
                    interpolatedSize.ToQuad( ),
                    interpolatedPosition.X, interpolatedPosition.Y,
                    interpolatedRotation, Frames[currentFrame].State[i].Mirrored);
            }

            return result;
        }
    }
}
