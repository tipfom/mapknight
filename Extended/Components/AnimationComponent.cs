using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Components.Attributes;
using static mapKnight.Extended.Components.SkeletComponent;

namespace mapKnight.Extended.Components {

    [ComponentRequirement(typeof(SkeletComponent))]
    [UpdateBefore(ComponentEnum.Skelet)]
    public class AnimationComponent : Component {
        private List<VertexAnimation> animations;
        private int currentAnimation = -1;

        public AnimationComponent (Entity owner, List<VertexAnimation> animations, string defaultanimation) : base(owner) {
            this.animations = animations;
            if (defaultanimation != null) setAnimation(defaultanimation);
        }

        private VertexAnimation current { get { return animations[currentAnimation]; } }
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


        public new class Configuration : Component.Configuration {
            public List<VertexAnimation> Animations;
            public string DefaultAnimation;

            public override Component Create (Entity owner) {
                return new AnimationComponent(owner, Animations, DefaultAnimation);
            }
        }
    }
}