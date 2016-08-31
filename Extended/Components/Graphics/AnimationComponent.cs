using System.Collections.Generic;
using System;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Components.Attributes;

namespace mapKnight.Extended.Components.Graphics {

    [ComponentRequirement(typeof(DrawComponent))]
    [UpdateBefore(ComponentEnum.Draw)]
    public class AnimationComponent : Component {
        private List<VertexAnimation> animations;
        private int currentAnimationIndex;
        private int defaultAnimationIndex;

        private AnimationCallback currentAnimationCallback;
        private AnimationCallback queuedAnimationCallback;
        private string queuedAnimation;
        private float currentDT;

        public AnimationComponent (Entity owner, List<VertexAnimation> animations, string defaultanimation) : base(owner) {
            this.animations = animations;
            setAnimation(defaultanimation);
            defaultAnimationIndex = animations.FindIndex(anim => anim.Name == defaultanimation);
            currentAnimationIndex = defaultAnimationIndex;
            if (defaultAnimationIndex == -1) throw new ArgumentException("invalid default animation at entity " + Owner.Name);
        }

        public delegate void AnimationCallback (bool success);

        private VertexAnimation currentAnimation { get { return animations[currentAnimationIndex]; } }

        public override void Update (DeltaTime dt) {
            while (Owner.HasComponentInfo(ComponentData.VertexAnimation)) {
                object[ ] data = Owner.GetComponentInfo(ComponentData.VertexAnimation);
                if ((bool)data[1]) {
                    // force animation
                    currentAnimationCallback(false);
                    currentAnimationCallback = (AnimationCallback)data[2];
                    setAnimation((string)data[0]);
                } else {
                    queuedAnimation = (string)data[0];
                    if (data.Length == 3) queuedAnimationCallback = (AnimationCallback)data[2];
                }
            }

            if (!currentAnimation.IsRunning) {
                if (!currentAnimation.CanRepeat) {
                    currentAnimationCallback?.Invoke(true);
                    if (queuedAnimation != null) {
                        currentAnimationCallback = queuedAnimationCallback;
                        setAnimation(queuedAnimation);
                        queuedAnimation = null;
                    } else {
                        currentAnimationIndex = defaultAnimationIndex;
                    }
                }
                currentAnimation.Reset( );
            }
            currentDT = dt.Milliseconds;
        }

        public override void PostUpdate ( ) {
            float[ ][ ] vertexData = animations[currentAnimationIndex].Update(currentDT);
            for (int i = 0; i < vertexData.Length; i++) {
                for (int j = 0; j < 4; j++) {
                    vertexData[i][j * 2 + 0] = (vertexData[i][j * 2 + 0] - 0.5f) * Owner.Transform.Size.X * Owner.World.VertexSize;
                    vertexData[i][j * 2 + 1] = (vertexData[i][j * 2 + 1] - 0.5f) * Owner.Transform.Size.Y * Owner.World.VertexSize;
                }
            }
            Owner.SetComponentInfo(ComponentData.Verticies, vertexData);
        }

        private void setAnimation (string name) {
            currentAnimationIndex = animations.FindIndex(animation => animation.Name == name);
            if (currentAnimationIndex == -1) {
                currentAnimationIndex = defaultAnimationIndex;
                currentAnimationCallback?.Invoke(false);
            }
            currentAnimation.Reset( );
        }

        public new class Configuration : Component.Configuration {
            public List<VertexAnimation> Animations;
            public string Default;

            public override Component Create (Entity owner) {
                return new AnimationComponent(owner, Animations, Default);
            }
        }
    }
}