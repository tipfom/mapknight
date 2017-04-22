using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Graphics.Animation;
using System.Linq;

namespace mapKnight.Extended.Components.Graphics {
    [UpdateBefore(typeof(DrawComponent))]
    public class AnimationComponent : Component {
        private static Dictionary<int, float[ ][ ]> loadCache = new Dictionary<int, float[ ][ ]>( );

        private VertexAnimation[ ] animations;
        private int currentAnimationIndex;

        private AnimationCallback currentAnimationCallback;
        private AnimationCallback queuedAnimationCallback;
        private int queuedAnimationIndex = -1;

        private Configuration config;
        private float[ ] scales;
        private float[ ][ ] boneVerticies;

        public AnimationComponent (Entity owner, VertexAnimation[ ] animations, float[ ] scales, Configuration config) : base(owner) {
            this.animations = animations;
            this.scales = scales;
            this.config = config;
        }

        public delegate void AnimationCallback (bool success);

        private VertexAnimation currentAnimation { get { return animations[currentAnimationIndex]; } }

        public override void Update (DeltaTime dt) {
            if (!Owner.IsOnScreen)
                return;

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

            while (Owner.HasComponentInfo(ComponentData.VertexAnimation)) {
                object[ ] data = Owner.GetComponentInfo(ComponentData.VertexAnimation);
                if ((bool)data[1]) {
                    // force animation
                    currentAnimationCallback?.Invoke(false);
                    if (data.Length == 3) {
                        currentAnimationCallback = (AnimationCallback)data[2];
                    } else {
                        currentAnimationCallback = null;
                    }
                    SetAnimation(FindAnimationIndex((string)data[0]));
                } else {
                    queuedAnimationIndex = FindAnimationIndex((string)data[0]);
                    if (data.Length == 3) {
                        queuedAnimationCallback = (AnimationCallback)data[2];
                    } else {
                        queuedAnimationCallback = null;
                    }
                }
            }
            animations[currentAnimationIndex].Update(dt.TotalMilliseconds, Owner.Transform, Owner.World.VertexSize, boneVerticies);
        }

        public override void Draw ( ) {
            if (!Owner.IsOnScreen)
                return;
            Owner.SetComponentInfo(ComponentData.Verticies, animations[currentAnimationIndex].Verticies);
            Owner.SetComponentInfo(ComponentData.Texture, animations[currentAnimationIndex].Textures);
        }

        public override void Prepare ( ) {
            if (!loadCache.ContainsKey(Owner.Species) || !Owner.World.Renderer.HasTexture(Owner.Species)) {
                Spritebatch2D sprite;
                Compiler.Compile(animations[0].Frames[0].State.Select(b => b.Texture).ToArray( ), scales, config.Offsets, config.Textures, Owner, out boneVerticies, out sprite);
                Owner.World.Renderer.AddTexture(Owner.Species, sprite);
                if (!loadCache.ContainsKey(Owner.Species))
                    loadCache.Add(Owner.Species, boneVerticies);
            } else {
                boneVerticies = loadCache[Owner.Species];
            }
            SetAnimation(0);
            currentAnimation.Reset( );
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
                if (animations[i].Name == name)
                    return i;
            }
            return -1;
        }

        public new class Configuration : Component.Configuration {
            public VertexAnimation[ ] Animations;
            public float[ ] Scales;
            public string[ ] Textures;
            public Vector2[ ] Offsets;

            public override Component Create (Entity owner) {
                return new AnimationComponent(owner, Animations, Scales, this);
            }
        }
    }
}