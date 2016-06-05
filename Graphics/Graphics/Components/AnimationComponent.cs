using mapKnight.Extended.Components.Communication;
using mapKnight.Extended.Graphics;
using System.Collections.Generic;

namespace mapKnight.Extended.Components {
    public class AnimationComponent : Component {
        private List<Animation> animations;
        private int currentAnimation = -1;
        private Animation current { get { return animations[currentAnimation]; } }
        private bool isAnimating { get { return currentAnimation != -1; } set { currentAnimation = value ? currentAnimation : -1; } }

        public AnimationComponent (Entity owner, List<Animation> animations) : base (owner) {
            this.animations = animations;
        }

        public override void Update (float dt) {
            while (Owner.HasComponentInfo (Identifier.Animation)) {
                Info ComponentInfo = Owner.GetComponentInfo (Identifier.Animation);
                if (ComponentInfo.Action == Data.Animation) {
                    setAnimation ((string)ComponentInfo.Data);
                }
            }

            if (isAnimating) {
                isAnimating = current.IsRunning;
                if (!isAnimating)
                    return;
                Owner.SetComponentInfo (Identifier.Skelet, Identifier.Animation, Data.Verticies, animations[currentAnimation].Update (dt));
            }
        }

        private void setAnimation (string name) {
            currentAnimation = animations.FindIndex (animation => animation.Name == name);
            if (isAnimating)
                current.Reset ();
        }
    }
}