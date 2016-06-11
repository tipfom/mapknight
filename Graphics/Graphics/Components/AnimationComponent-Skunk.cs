using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.Entity.Components {
    public class AnimationComponent : Component {
        private Dictionary<string, Animation> animations;
        private string currentAnimation;
        private bool isAnimating;

        public AnimationComponent (Entity owner, Dictionary<string, Animation> animations) : base (owner) {
            this.animations = animations;
        }

        public override void Update (float dt) {
            while (Owner.HasComponentInfo (Type.Animation)) {
                Info componentInfo = Owner.GetComponentInfo (Type.Animation);
                if (componentInfo.Action == Action.Animation) {
                    setAnimation ((string)componentInfo.Data);
                }
            }
            if (isAnimating && animations[currentAnimation].IsRunning) {
                Owner.SetComponentInfo (Type.Skelet, Type.Animation, Action.VertexData, animations[currentAnimation].Update (dt));
            }
        }

        private void setAnimation (string name) {
            isAnimating = animations.ContainsKey (name);
            if (isAnimating) {
                currentAnimation = name;
                animations[currentAnimation].Reset ( );
            }
        }
    }
}