using System;
using System.Collections.Generic;
using System.Diagnostics;
#if __ANDROID__
#else
using System.Collections.ObjectModel;
#endif
using System.Text;
using Newtonsoft.Json;

namespace mapKnight.Core.Graphics {
    public class VertexAnimation {
        public string Name { get; set; }
        public bool CanRepeat
#if __ANDROID__
        ;
#else
        { get; set; }
#endif
#if __ANDROID__
        public VertexAnimationFrame[ ] Frames;
#else
        public ObservableCollection<VertexAnimationFrame> Frames { get; set; }
#endif
        private int currentFrame;
        private int nextFrame;
        private int nextFrameTime;
        [JsonIgnore]
        public bool IsRunning;

#if __ANDROID__
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

            float[ ][ ] result = new float[Frames[currentFrame].Bones.Length][ ];

            for (int i = 0; i < result.Length; i++) {
                Vector2 interpolatedSize = Mathf.Interpolate(Frames[nextFrame].Bones[i].Size, Frames[currentFrame].Bones[i].Size, progress) * ownerTransform.Size * vsize;
                Vector2 interpolatedPosition = Mathf.Interpolate(Frames[nextFrame].Bones[i].Position, Frames[currentFrame].Bones[i].Position, progress) * ownerTransform.Size * vsize;
                float interpolatedRotation = Mathf.Interpolate(Frames[nextFrame].Bones[i].Rotation, Frames[currentFrame].Bones[i].Rotation, progress);

                result[i] = Mathf.TransformAtOrigin(
                    interpolatedSize.ToQuad( ),
                    interpolatedPosition.X, interpolatedPosition.Y,
                    interpolatedRotation, Frames[currentFrame].Bones[i].Mirrored);
            }

            return result;
        }
#endif
    }
}
