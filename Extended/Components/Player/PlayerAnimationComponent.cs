using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Core.World.Serialization;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Graphics.Animation;

namespace mapKnight.Extended.Components.Player {
    [UpdateBefore(typeof(DrawComponent))]
    public class PlayerAnimationComponent : Component {
        public const int BODY_ANIMATION = 0;
        public const int WEAPON_ANIMATION = 1;

        public delegate string AnimationCallback (bool completed);

        private MotionComponent motionComponent;
        private bool reversed { get { return motionComponent.ScaleX < 0; } }

        private VertexAnimation[ ] bodyAnimations;
        private VertexAnimation[ ] primaryWeaponAnimations;
        private VertexAnimation[ ] secondaryWeaponAnimations;

        private float[ ][ ] baseVertices;
        private float[ ][ ] joinedVertices;
        private string[ ] joinedTextures;

        private AnimationCallback currentBodyAnimationCallback;
        private VertexAnimation currentBodyAnimation;
        private VertexAnimation currentSecondaryWeaponAnimation;

        private AnimationCallback currentPrimaryWeaponAnimationCallback;
        private VertexAnimation currentPrimaryWeaponAnimation;

        public PlayerAnimationComponent (Entity owner) : base(owner) {
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
        }

        public void LoadAnimations (VertexAnimationData bodyData, VertexAnimationData primaryWeaponData, VertexAnimationData secondaryWeaponData, params string[ ] textures) {
            bodyAnimations = bodyData.Animations;
            primaryWeaponAnimations = primaryWeaponData.Animations;
            secondaryWeaponAnimations = secondaryWeaponData.Animations;

            int joinedLength = bodyData.Scales.Length + primaryWeaponData.Scales.Length + secondaryWeaponData.Scales.Length;
            joinedVertices = new float[joinedLength][ ];
            joinedTextures = new string[joinedLength];

            float[ ] joinedScales = new float[joinedLength];
            string[ ] joinedInitialTextures = new string[joinedLength];
            Vector2[ ] joinedOffsets = new Vector2[joinedLength];

            for (int i = 0; i < secondaryWeaponData.Scales.Length; i++) {
                joinedScales[i] = secondaryWeaponData.Scales[i];
                joinedInitialTextures[i] = secondaryWeaponData.Animations[0].Frames[0].State[i].Texture;
                joinedOffsets[i] = secondaryWeaponData.Offsets[i];
            }
            int offset = secondaryWeaponData.Scales.Length;
            for (int i = 0; i < bodyData.Scales.Length; i++) {
                joinedScales[i + offset] = bodyData.Scales[i];
                joinedInitialTextures[i + offset] = bodyData.Animations[0].Frames[0].State[i].Texture;
                joinedOffsets[i + offset] = bodyData.Offsets[i];
            }
            offset += bodyData.Scales.Length;
            for (int i = 0; i < primaryWeaponData.Scales.Length; i++) {
                joinedScales[i + offset] = primaryWeaponData.Scales[i];
                joinedInitialTextures[i + offset] = primaryWeaponData.Animations[0].Frames[0].State[i].Texture;
                joinedOffsets[i + offset] = primaryWeaponData.Offsets[i];
            }

            Spritebatch2D sprite;
            Compiler.Compile(joinedInitialTextures, joinedScales, joinedOffsets, textures, Owner, out baseVertices, out sprite);
            Owner.World.Renderer.AddTexture(Owner.Species, sprite);

            (currentBodyAnimation = bodyData.Animations[0]).Reset( );
            (currentPrimaryWeaponAnimation = primaryWeaponData.Animations[0]).Reset( );
            (currentSecondaryWeaponAnimation = secondaryWeaponData.Animations[0]).Reset( );
        }

        public override void Update (DeltaTime dt) {
            if (!Owner.IsOnScreen) return;

            if (!currentBodyAnimation.IsRunning) {
                string callbackResult = currentBodyAnimationCallback?.Invoke(true);
                if (callbackResult != null) {
                    SetBodyAnimation(callbackResult);
                } else if (currentBodyAnimation.CanRepeat) {
                    currentBodyAnimation.IsRunning = true;
                    currentSecondaryWeaponAnimation.IsRunning = true;
                } else {
                    (currentBodyAnimation = bodyAnimations[0]).Reset( );
                    (currentSecondaryWeaponAnimation = secondaryWeaponAnimations[0]).Reset( );
                }
            }

            if (!currentPrimaryWeaponAnimation.IsRunning) {
                string callbackResult = currentPrimaryWeaponAnimationCallback?.Invoke(true);
                if (callbackResult != null) {
                    SetWeaponAnimation(callbackResult);
                    currentPrimaryWeaponAnimationCallback = null;
                } else if (currentPrimaryWeaponAnimation.CanRepeat) {
                    currentPrimaryWeaponAnimation.IsRunning = true;
                } else {
                    currentPrimaryWeaponAnimationCallback = null;
                    (currentPrimaryWeaponAnimation = primaryWeaponAnimations[0]).Reset( );
                }
            }

            while (Owner.HasComponentInfo(ComponentData.VertexAnimation)) {
                object[ ] data = Owner.GetComponentInfo(ComponentData.VertexAnimation);
                AnimationCallback callback = data.Length == 3 ? (AnimationCallback)data[2] : null;
                switch ((int)data[0]) {
                    case BODY_ANIMATION:
                        currentBodyAnimationCallback?.Invoke(false);
                        SetBodyAnimation((string)data[1]);
                        currentBodyAnimationCallback = callback;
                        break;
                    case WEAPON_ANIMATION:
                        currentPrimaryWeaponAnimationCallback?.Invoke(false);
                        SetWeaponAnimation((string)data[1]);
                        currentPrimaryWeaponAnimationCallback = callback;
                        break;
                }
            }

            currentSecondaryWeaponAnimation.Update(dt.TotalMilliseconds, Owner.Transform, Owner.World.VertexSize, baseVertices, 0);
            currentBodyAnimation.Update(dt.TotalMilliseconds, Owner.Transform, Owner.World.VertexSize, baseVertices, currentSecondaryWeaponAnimation.Verticies.Length);
            currentPrimaryWeaponAnimation.Update(dt.TotalMilliseconds, Owner.Transform, Owner.World.VertexSize, baseVertices, currentBodyAnimation.Verticies.Length + currentSecondaryWeaponAnimation.Verticies.Length);
        }

        public override void Draw ( ) {
            if (!Owner.IsOnScreen) return;
            if (reversed) {
                for (int i = 0; i < currentPrimaryWeaponAnimation.Verticies.Length; i++) {
                    joinedVertices[i] = currentPrimaryWeaponAnimation.Verticies[i];
                    joinedTextures[i] = currentPrimaryWeaponAnimation.Textures[i];
                }
                int offset = currentPrimaryWeaponAnimation.Verticies.Length;
                for (int i = 0; i < currentBodyAnimation.Verticies.Length; i++) {
                    joinedVertices[i + offset] = currentBodyAnimation.Verticies[i];
                    joinedTextures[i + offset] = currentBodyAnimation.Textures[i];
                }
                offset += currentBodyAnimation.Verticies.Length;
                for (int i = 0; i < currentSecondaryWeaponAnimation.Verticies.Length; i++) {
                    joinedVertices[i + offset] = currentSecondaryWeaponAnimation.Verticies[i];
                    joinedTextures[i + offset] = currentSecondaryWeaponAnimation.Textures[i];
                }
            } else {
                for (int i = 0; i < currentSecondaryWeaponAnimation.Verticies.Length; i++) {
                    joinedVertices[i] = currentSecondaryWeaponAnimation.Verticies[i];
                    joinedTextures[i] = currentSecondaryWeaponAnimation.Textures[i];
                }
                int offset = currentSecondaryWeaponAnimation.Verticies.Length;
                for (int i = 0; i < currentBodyAnimation.Verticies.Length; i++) {
                    joinedVertices[i + offset] = currentBodyAnimation.Verticies[i];
                    joinedTextures[i + offset] = currentBodyAnimation.Textures[i];
                }
                offset += currentBodyAnimation.Verticies.Length;
                for (int i = 0; i < currentPrimaryWeaponAnimation.Verticies.Length; i++) {
                    joinedVertices[i + offset] = currentPrimaryWeaponAnimation.Verticies[i];
                    joinedTextures[i + offset] = currentPrimaryWeaponAnimation.Textures[i];
                }
            }

            Owner.SetComponentInfo(ComponentData.Verticies, joinedVertices);
            Owner.SetComponentInfo(ComponentData.Texture, joinedTextures);
        }

        private void SetBodyAnimation (string name) {
            int index = 0;
            for (int i = 0; i < bodyAnimations.Length; i++) {
                if (bodyAnimations[i].Name == name) {
                    index = i;
                    break;
                }
            }
            (currentBodyAnimation = bodyAnimations[index]).Reset( );
            (currentSecondaryWeaponAnimation = secondaryWeaponAnimations[index]).Reset( );
        }

        private void SetWeaponAnimation (string name) {
            int index = 0;
            for (int i = 0; i < primaryWeaponAnimations.Length; i++) {
                if (primaryWeaponAnimations[i].Name == name) {
                    index = i;
                    break;
                }
            }
            (currentPrimaryWeaponAnimation = primaryWeaponAnimations[index]).Reset( );
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new PlayerAnimationComponent(owner);
            }
        }
    }
}
