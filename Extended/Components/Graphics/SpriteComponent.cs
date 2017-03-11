using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Animation;
using mapKnight.Core.Graphics;
using mapKnight.Core.World.Components;
using mapKnight.Core.World;

namespace mapKnight.Extended.Components.Graphics {

    [UpdateBefore(typeof(DrawComponent))]
    public class SpriteComponent : Component {
        private SpriteAnimation[ ] animations;
        private string[ ] cache;

        public delegate void AnimationCallback(bool success);

        private AnimationCallback currentAnimationCallback;
        private SpriteAnimation currentAnimation;
        private SpriteAnimation queuedAnimation;
        private AnimationCallback queuedAnimationCallback;

        public SpriteComponent(Entity owner, SpriteAnimation[ ] animations, string texture) : base(owner) {
            this.animations = animations;
            this.currentAnimation = animations[0];
            this.cache = new string[currentAnimation.Frames[0].Bones.Length];
            if (!Owner.World.Renderer.HasTexture(Owner.Species))
                Owner.World.Renderer.AddTexture(Owner.Species, Assets.Load<Spritebatch2D>(texture));
        }

        public override void Update(DeltaTime dt) {
            if (!Owner.IsOnScreen)
                return;

            if (!currentAnimation.IsRunning) {
                currentAnimationCallback?.Invoke(true);
                if (queuedAnimation != null) {
                    currentAnimation = queuedAnimation;
                    queuedAnimation = null;
                    currentAnimationCallback = queuedAnimationCallback;
                    queuedAnimationCallback = null;
                } else if (!currentAnimation.CanRepeat) {
                    currentAnimation = animations[0];
                    currentAnimationCallback = null;
                }
                currentAnimation.Reset( );
            }

            while (Owner.HasComponentInfo(ComponentData.SpriteAnimation)) {
                object[ ] data = Owner.GetComponentInfo(ComponentData.SpriteAnimation);
                if ((bool)data[1] && currentAnimation.Name != (string)data[0]) {
                    // force animation
                    currentAnimationCallback?.Invoke(false);
                    currentAnimation = animations.FirstOrDefault(anim => anim.Name == (string)data[0]);
                    if (currentAnimation == null)
                        currentAnimation = animations[0];
                    currentAnimationCallback = (data.Length == 3) ? (AnimationCallback)data[2] : null;
                    queuedAnimation = null;
                    currentAnimation.Reset( );
                } else {
                    // queue animation
                    queuedAnimation = animations.FirstOrDefault(anim => anim.Name == (string)data[0]);
                    queuedAnimationCallback = (data.Length == 3) ? (AnimationCallback)data[2] : null;
                }
            }

            currentAnimation.Update(dt.TotalMilliseconds);
        }

        public override void Draw( ) {
            for (int i = 0; i < cache.Length; i++)
                cache[i] = currentAnimation.CurrentFrame.Bones[i];

            Owner.SetComponentInfo(ComponentData.Texture, cache);
        }

        public new class Configuration : Component.Configuration {
            public SpriteAnimation[ ] Animations;
            public string Texture;

            public override Component Create(Entity owner) {
                return new SpriteComponent(owner, Animations, Texture);
            }
        }
    }
}