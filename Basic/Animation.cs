using System;
using System.Collections.Generic;

namespace mapKnight.Basic {
    public struct Animation {
        public bool Repeat;
        public List<Step> Steps;
        public bool IsRunning { get; private set; }

        private int nextStepTime;
        private int currentStep;

        public void Reset () {
            nextStepTime = Environment.TickCount + Steps[0].Time;
            currentStep = 0;
            IsRunning = true;
        }

        public Dictionary<string, float[ ]> Update (float dt) {
            if (Environment.TickCount > nextStepTime) {
                if (currentStep + 1 < Steps.Count) {
                    // if the next step isnt the last ont
                    currentStep++;
                    nextStepTime += Steps[currentStep].Time;
                } else if (Repeat) {
                    Reset ( );
                } else {
                    IsRunning = false;
                }
            }
            float progress = IsRunning ? (nextStepTime - Environment.TickCount) / Steps[currentStep].Time : 1f;

            Dictionary<string, float[ ]> result = new Dictionary<string, float[ ]> ( );

            foreach (string bone in Steps[currentStep].State.Keys) {
                Vector2 interpolatedSize = MathHelper.Interpolate (Steps[currentStep].State[bone].Size, Steps[currentStep + 1].State[bone].Size, progress);
                Vector2 interpolatedPosition = MathHelper.Interpolate (Steps[currentStep].State[bone].Position, Steps[currentStep + 1].State[bone].Position, progress);
                float interpolatedRotation = MathHelper.Interpolate (Steps[currentStep].State[bone].Rotation, Steps[currentStep + 1].State[bone].Rotation, progress);

                result.Add (bone, MathHelper.TranslateRotateMirror (
                    MathHelper.GetVerticies (interpolatedSize),
                    0, 0,
                    interpolatedPosition.X, interpolatedPosition.Y,
                    interpolatedRotation, Steps[currentStep].State[bone].Mirrored));
            }

            return result;
        }
    }
}
