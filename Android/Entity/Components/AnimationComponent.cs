using mapKnight.Basic.Components;
using System.Collections.Generic;

namespace mapKnight.Android.Entity.Components {
    public class AnimationComponent : Component {
        private List<Animation> animations;
        private int currentAnimation = -1;
        private Animation current { get { return animations[currentAnimation]; } }
        private bool isAnimating { get { return currentAnimation != -1; } set { currentAnimation = value ? currentAnimation : -1; } }

        public AnimationComponent (Entity owner, List<Animation> animations) : base (owner) {
            this.animations = animations;
        }

        public override void Update (float dt) {
            while (Owner.HasComponentInfo (Type.Animation)) {
                Info componentInfo = Owner.GetComponentInfo (Type.Animation);
                if (componentInfo.Action == Action.Animation) {
                    setAnimation ((string)componentInfo.Data);
                }
            }

            if (isAnimating) {
                isAnimating = current.IsRunning;
                if (!isAnimating)
                    return;
                Owner.SetComponentInfo (Type.Skelet, Type.Animation, Action.VertexData, animations[currentAnimation].Update (dt));
            }
        }

        private void setAnimation (string name) {
            currentAnimation = animations.FindIndex (animation => animation.Name == name);
            if (isAnimating)
                current.Reset ();
        }
    }
}