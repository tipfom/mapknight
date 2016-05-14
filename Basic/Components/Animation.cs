using System;
using System.Collections.Generic;

namespace mapKnight.Basic.Components {
    public class Animation {
        public bool Repeat;
        public string Name;
        public List<Step> Steps;
        public bool IsRunning { get; private set; }

        private int nextStepTime;
        private int currentStep;
        private int nextStep;

        public void Reset () {
            nextStepTime = Environment.TickCount + Steps[0].Time;
            currentStep = 0;
            nextStep = 1;
            IsRunning = true;
        }

        public Dictionary<string, float[]> Update (float dt) {
            if (Environment.TickCount > nextStepTime) {
                if (nextStep +1 < Steps.Count) {
                    // if the next step isnt the last ont
                    currentStep = nextStep;
                    nextStep++;
                    nextStepTime += Steps[currentStep].Time;
                } else if (Repeat) {
                    currentStep = nextStep;
                    nextStep = 0;
                    nextStepTime += Steps[currentStep].Time;
                } else {
                    IsRunning = false;
                }
            }
            float progress = IsRunning ? (nextStepTime - Environment.TickCount) / (float)Steps[currentStep].Time : 1f;

            Dictionary<string, float[]> result = new Dictionary<string, float[]> ();

            foreach (string bone in Steps[currentStep].State.Keys) {
                Vector2 interpolatedSize = MathHelper.Interpolate (Steps[nextStep].State[bone].Size, Steps[currentStep].State[bone].Size, progress);
                Vector2 interpolatedPosition = MathHelper.Interpolate (Steps[nextStep].State[bone].Position, Steps[currentStep].State[bone].Position, progress);
                float interpolatedRotation = MathHelper.Interpolate (Steps[nextStep].State[bone].Rotation, Steps[currentStep].State[bone].Rotation, progress);

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
