using System;
using System.Collections.Generic;
using mapKnight.Core;
using static mapKnight.Extended.Components.SkeletComponent;

namespace mapKnight.Extended.Components {
    [ComponentRequirement(typeof(SkeletComponent))]
    [ComponentOrder(ComponentEnum.Skelet)]
    public class AnimationComponent : Component {
        private List<Animation> animations;
        private int currentAnimation = -1;
        private Animation current { get { return animations[currentAnimation]; } }
        private bool isAnimating { get { return currentAnimation != -1; } set { currentAnimation = value ? currentAnimation : -1; } }

        public AnimationComponent (Entity owner, List<Animation> animations) : base(owner) {
            this.animations = animations;
        }

        public override void Update (DeltaTime dt) {
            while (Owner.HasComponentInfo(ComponentEnum.Animation)) {
                ComponentInfo ComponentInfo = Owner.GetComponentInfo(ComponentEnum.Animation);
                if (ComponentInfo.Action == ComponentData.Animation) {
                    setAnimation((string)ComponentInfo.Data);
                }
            }

            if (isAnimating) {
                isAnimating = current.IsRunning;
                if (!isAnimating)
                    return;
                Owner.SetComponentInfo(ComponentEnum.Skelet, ComponentEnum.Animation, ComponentData.Verticies, animations[currentAnimation].Update(dt.Milliseconds));
            }
        }

        private void setAnimation (string name) {
            currentAnimation = animations.FindIndex(animation => animation.Name == name);
            if (isAnimating)
                current.Reset( );
        }

        public class Animation {
            public bool Repeat;
            public string Name;
            public List<Step> Steps;
            public bool IsRunning { get; private set; }

            private int nextStepTime;
            private int currentStep;
            private int nextStep;

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

        public struct Step {
            public int Time;
            public Dictionary<string, Bone> State;
        }

        public new class Configuration : Component.Configuration {
            public List<Animation> Animations;

            public override Component Create (Entity owner) {
                return new AnimationComponent(owner, Animations);
            }
        }
    }
}