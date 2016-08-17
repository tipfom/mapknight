using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using static mapKnight.Extended.Components.SkeletComponent;

namespace mapKnight.Extended.Components {

    [ComponentRequirement(typeof(SkeletComponent))]
    [UpdateBefore(ComponentEnum.Skelet)]
    public class AnimationComponent : Component {
        private List<Animation> animations;
        private int currentAnimation = -1;

        public AnimationComponent (Entity owner, List<Animation> animations, string defaultanimation) : base(owner) {
            this.animations = animations;
            if (defaultanimation != null) setAnimation(defaultanimation);
        }

        private Animation current { get { return animations[currentAnimation]; } }
        private bool isAnimating { get { return currentAnimation != -1; } set { currentAnimation = value ? currentAnimation : -1; } }

        public override void Update (DeltaTime dt) {
            while (Owner.HasComponentInfo(ComponentEnum.Animation)) {
                string animationToPlay = (string)Owner.GetComponentInfo(ComponentEnum.Animation);
                setAnimation(animationToPlay);
            }

            if (isAnimating) {
                isAnimating = current.IsRunning;
                if (!isAnimating)
                    return;
                Owner.SetComponentInfo(ComponentEnum.Skelet, animations[currentAnimation].Update(dt.Milliseconds));
            }
        }

        private void setAnimation (string name) {
            currentAnimation = animations.FindIndex(animation => animation.Name == name);
            if (isAnimating)
                current.Reset( );
        }

        public struct Step {
            public Dictionary<string, Bone> State;
            public int Time;
        }

        public class Animation {
            public string Name;
            public bool Repeat;
            public List<Step> Steps;
            private int currentStep;
            private int nextStep;
            private int nextStepTime;
            public bool IsRunning { get; private set; }

            public void Reset ( ) {
                nextStepTime = Environment.TickCount + Steps[0].Time;
                currentStep = 0;
                nextStep = 1;
                IsRunning = true;
            }

            public Dictionary<string, float[ ]> Update (float dt) {
                if (Environment.TickCount > nextStepTime) {
                    if (nextStep + 1 < Steps.Count) {
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

                Dictionary<string, float[ ]> result = new Dictionary<string, float[ ]>( );

                foreach (string bone in Steps[currentStep].State.Keys) {
                    Vector2 interpolatedSize = Mathf.Interpolate(Steps[nextStep].State[bone].Size, Steps[currentStep].State[bone].Size, progress);
                    Vector2 interpolatedPosition = Mathf.Interpolate(Steps[nextStep].State[bone].Position, Steps[currentStep].State[bone].Position, progress);
                    float interpolatedRotation = Mathf.Interpolate(Steps[nextStep].State[bone].Rotation, Steps[currentStep].State[bone].Rotation, progress);

                    result.Add(bone, Mathf.Transform(
                        interpolatedSize.ToQuad( ),
                        interpolatedSize.X / 2, interpolatedSize.Y / 2,
                        interpolatedPosition.X, interpolatedPosition.Y,
                        interpolatedRotation, Steps[currentStep].State[bone].Mirrored));
                }

                return result;
            }
        }

        public new class Configuration : Component.Configuration {
            public List<Animation> Animations;
            public string DefaultAnimation;

            public override Component Create (Entity owner) {
                return new AnimationComponent(owner, Animations, DefaultAnimation);
            }
        }
    }
}