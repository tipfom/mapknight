using System;
using System.Collections.Generic;
#if WINDOWS
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
        public List<VertexAnimationFrame> Frames;
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

        public Dictionary<string, float[ ]> Update (float dt) {
            if (Environment.TickCount > nextFrameTime) {
                if (nextFrame + 1 < Frames.Count) {
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

            Dictionary<string, float[ ]> result = new Dictionary<string, float[ ]>( );

            foreach (string bone in Frames[currentFrame].State.Keys) {
                Vector2 interpolatedSize = Mathf.Interpolate(Frames[nextFrame].State[bone].Size, Frames[currentFrame].State[bone].Size, progress);
                Vector2 interpolatedPosition = Mathf.Interpolate(Frames[nextFrame].State[bone].Position, Frames[currentFrame].State[bone].Position, progress);
                float interpolatedRotation = Mathf.Interpolate(Frames[nextFrame].State[bone].Rotation, Frames[currentFrame].State[bone].Rotation, progress);

                result.Add(bone, Mathf.Transform(
                    interpolatedSize.ToQuad( ),
                    interpolatedSize.X / 2, interpolatedSize.Y / 2,
                    interpolatedPosition.X, interpolatedPosition.Y,
                    interpolatedRotation, Frames[currentFrame].State[bone].Mirrored));
            }

            return result;
        }

    }
}
