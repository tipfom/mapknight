using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [ComponentRequirement(typeof(DrawComponent))]
    [UpdateBefore(ComponentEnum.Draw)]
    public class SpriteComponent : Component {
        private SpriteAnimation[ ] animations;
        private string[] cache;

        public delegate void AnimationSuccessCallback (bool success);

        private AnimationSuccessCallback currentAnimationCallback;
        private SpriteAnimation currentAnimation;
        private SpriteAnimation queuedAnimation;
        private AnimationSuccessCallback queuedAnimationCallback;
        private SpriteAnimation defaultAnimation;

        public SpriteComponent (Entity owner, SpriteAnimation[ ] animations, string defaultanimation, string texture) : base(owner) {
            this.animations = animations;
            this.defaultAnimation = animations.First(anim => anim.Name == defaultanimation);
            this.currentAnimation = defaultAnimation;
            this.cache = new string[defaultAnimation.Frames[0].Bones.Length];
            Owner.World.Renderer.AddTexture(Owner.Species, Assets.Load<SpriteBatch>(texture));
        }

        public override void Update (DeltaTime dt) {
            while (Owner.HasComponentInfo(ComponentData.SpriteAnimation)) {
                object[ ] data = Owner.GetComponentInfo(ComponentData.SpriteAnimation);
                if ((bool)data[1]) {
                    // force animation
                    currentAnimation = animations.FirstOrDefault(anim => anim.Name == (string)data[0]);
                    if (currentAnimation == null) currentAnimation = defaultAnimation;
                    currentAnimationCallback = (data.Length == 3) ? (AnimationSuccessCallback)data[2] : null;
                    queuedAnimation = null;
                } else {
                    // queue animation
                    queuedAnimation = animations.FirstOrDefault(anim => anim.Name == (string)data[0]);
                    queuedAnimationCallback = (data.Length == 3) ? (AnimationSuccessCallback)data[2] : null;
                }
            }

            if (!currentAnimation.IsRunning) {
                currentAnimationCallback?.Invoke(true);
                if (queuedAnimation != null) {
                    currentAnimation = queuedAnimation;
                    queuedAnimation = null;
                    currentAnimationCallback = queuedAnimationCallback;
                    queuedAnimationCallback = null;
                } else if (!currentAnimation.CanRepeat) {
                    currentAnimation = defaultAnimation;
                    currentAnimationCallback = null;
                }
                currentAnimation.Reset( );
            }
            currentAnimation.Update(dt.TotalMilliseconds);
            for (int i = 0; i < cache.Length; i ++)
                cache[i] = currentAnimation.CurrentFrame.Bones[i];

            Owner.SetComponentInfo(ComponentData.Texture, cache);
        }

        public new class Configuration : Component.Configuration {
            public SpriteAnimation[ ] Animations;
            public string Texture;
            public string Default;

            public override Component Create (Entity owner) {
                return new SpriteComponent(owner, Animations, Default, Texture);
            }
        }
    }
}