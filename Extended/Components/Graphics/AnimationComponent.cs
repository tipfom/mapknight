using System.Collections.Generic;
using System;
using mapKnight.Core;
using System.Linq;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics.Animation;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [UpdateBefore(typeof(DrawComponent))]
    public class AnimationComponent : Component {
        private VertexAnimation[ ] animations;
        private int currentAnimationIndex;

        private AnimationCallback currentAnimationCallback;
        private AnimationCallback queuedAnimationCallback;
        private int queuedAnimationIndex = -1;

        private float[ ] scales;
        private float[ ][ ] boneVerticies;
        private SpriteBatch sprite;

        public AnimationComponent (Entity owner, VertexAnimation[ ] animations, float[ ] scales) : base(owner) {
            this.animations = animations;
            this.scales = scales;
        }

        public delegate void AnimationCallback (bool success);

        private VertexAnimation currentAnimation { get { return animations[currentAnimationIndex]; } }

        public override void Update (DeltaTime dt) {
            if (!Owner.IsOnScreen) return;

            while (Owner.HasComponentInfo(ComponentData.VertexAnimation)) {
                object[ ] data = Owner.GetComponentInfo(ComponentData.VertexAnimation);
                if ((bool)data[1]) {
                    // force animation
                    currentAnimationCallback(false);
                    currentAnimationCallback = (AnimationCallback)data[2];
                    SetAnimation(FindAnimationIndex((string)data[0]));
                } else {
                    queuedAnimationIndex = FindAnimationIndex((string)data[0]);
                    if (data.Length == 3) queuedAnimationCallback = (AnimationCallback)data[2];
                }
            }

            if (!currentAnimation.IsRunning) {
                currentAnimationCallback?.Invoke(true);
                if (queuedAnimationIndex > -1) {
                    currentAnimationCallback = queuedAnimationCallback;
                    SetAnimation(queuedAnimationIndex);
                    queuedAnimationIndex = -1;
                } else if (currentAnimation.CanRepeat) {
                    currentAnimation.IsRunning = true;
                } else {
                    currentAnimationIndex = 0;
                    currentAnimation.Reset( );
                }

            }
            animations[currentAnimationIndex].Update(dt.TotalMilliseconds, Owner.Transform, Owner.World.VertexSize, boneVerticies);
        }

        public override void PostUpdate ( ) {
            if (!Owner.IsOnScreen) return;
            Owner.SetComponentInfo(ComponentData.Verticies, animations[currentAnimationIndex].Verticies);
            Owner.SetComponentInfo(ComponentData.Texture, animations[currentAnimationIndex].Textures);
        }

        public override void Prepare ( ) {
            currentAnimation.Reset( );
        }

        public override void Load ( ) {
            List<string> sprites = new List<string>(4);
            while (Owner.HasComponentInfo(ComponentData.BoneTexture))
                sprites.Add((string)Owner.GetComponentInfo(ComponentData.BoneTexture)[0]);
            Compiler.Compile(animations, scales, new Vector2[scales.Length], sprites, Owner, out boneVerticies, out sprite);
            Owner.World.Renderer.AddTexture(Owner.Species, sprite);
            SetAnimation(0);
        }

        private void SetAnimation (int index) {
            currentAnimationIndex = index;
            if (currentAnimationIndex < 0 || currentAnimationIndex >= animations.Length) {
                currentAnimationIndex = 0;
                currentAnimationCallback?.Invoke(false);
            }
            currentAnimation.Reset( );
        }

        private int FindAnimationIndex (string name) {
            for (int i = 0; i < animations.Length; i++) {
                if (animations[i].Name == name) return i;
            }
            return -1;
        }

        public new class Configuration : Component.Configuration {
            public VertexAnimation[ ] Animations;
            public float[ ] Scales;

            public override Component Create (Entity owner) {
                return new AnimationComponent(owner, Animations, Scales);
            }
        }
    }
}