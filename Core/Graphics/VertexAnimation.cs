using System;
using System.Collections.Generic;
#if __ANDROID__
#else
using System.Collections.ObjectModel;
#endif
using System.Text;

namespace mapKnight.Core.Graphics {
    public class VertexAnimation {
        public string Name { get; set; }
        public bool Repeat
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
        public bool IsRunning { get; private set; }

        public void Reset ( ) {
            nextFrameTime = Environment.TickCount + Frames[0].Time;
            currentFrame = 0;
            nextFrame = 1;
            IsRunning = true;
        }

#if __ANDROID__
        public float[ ][ ] Update (float dt) {
            if (Environment.TickCount > nextFrameTime) {
                if (nextFrame + 1 < Frames.Length) {
                    // if the next Frame isnt the last ont
                    currentFrame = nextFrame;
                    nextFrame++;
                    nextFrameTime += Frames[currentFrame].Time;
                } else if (Repeat) {
                    currentFrame = nextFrame;
                    nextFrame = 0;
                    nextFrameTime += Frames[currentFrame].Time;
                } else {
                    IsRunning = false;
                }
            }
            float progress = IsRunning ? (nextFrameTime - Environment.TickCount) / (float)Frames[currentFrame].Time : 1f;

            float[ ][ ] result = new float[Frames[currentFrame].Bones.Length][ ];

            for (int i = 0; i < result.Length; i++) {
                Vector2 interpolatedSize = Mathf.Interpolate(Frames[nextFrame].Bones[i].Size, Frames[currentFrame].Bones[i].Size, progress);
                Vector2 interpolatedPosition = Mathf.Interpolate(Frames[nextFrame].Bones[i].Position, Frames[currentFrame].Bones[i].Position, progress);
                float interpolatedRotation = Mathf.Interpolate(Frames[nextFrame].Bones[i].Rotation, Frames[currentFrame].Bones[i].Rotation, progress);

                result[i] = Mathf.Transform(
                    interpolatedSize.ToQuad( ),
                    interpolatedSize.X / 2, interpolatedSize.Y / 2,
                    interpolatedPosition.X, interpolatedPosition.Y,
                    interpolatedRotation, Frames[currentFrame].Bones[i].Mirrored);
            }

            return result;
        }
#endif
    }
}
