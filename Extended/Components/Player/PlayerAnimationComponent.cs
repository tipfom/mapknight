using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Graphics.Animation;

namespace mapKnight.Extended.Components.Player {
    [UpdateBefore(typeof(DrawComponent))]
    public class PlayerAnimationComponent : Component {
        public const int BODY_ANIMATION = 0;
        public const int WEAPON_ANIMATION = 1;

        public delegate string AnimationCallback (bool completed);

        private VertexAnimation[ ] bodyAnimations;
        private VertexAnimation[ ] weaponAnimations;
        private float[ ][ ] baseVertices;
        private float[ ][ ] joinedVertices;
        private string[ ] joinedTextures;
        private int currentBodyAnimationIndex;
        private AnimationCallback currentBodyAnimationCallback;
        private VertexAnimation currentBodyAnimation { get { return bodyAnimations[currentBodyAnimationIndex]; } }
        private int currentWeaponAnimationIndex;
        private AnimationCallback currentWeaponAnimationCallback;
        private VertexAnimation currentWeaponAnimation { get { return weaponAnimations[currentWeaponAnimationIndex]; } }

        public PlayerAnimationComponent (Entity owner) : base(owner) {
        }

        public void LoadAnimations (VertexAnimationData bodyData, VertexAnimationData weaponData, params string[ ] textures) {
            bodyAnimations = bodyData.Animations;
            weaponAnimations = weaponData.Animations;

            int joinedLength = bodyData.Scales.Length + weaponData.Scales.Length;
            joinedVertices = new float[joinedLength][ ];
            joinedTextures = new string[joinedLength];

            float[ ] joinedScales = new float[joinedLength];
            string[ ] joinedInitialTextures = new string[joinedLength];
            Vector2[ ] joinedOffsets = new Vector2[joinedLength];

            for (int i = 0; i < bodyData.Scales.Length; i++) {
                joinedScales[i] = bodyData.Scales[i];
                joinedInitialTextures[i] = bodyData.Animations[0].Frames[0].State[i].Texture;
                joinedOffsets[i] = bodyData.Offsets[i];
            }
            for (int i = 0; i < weaponData.Scales.Length; i++) {
                joinedScales[i + bodyData.Scales.Length] = weaponData.Scales[i];
                joinedInitialTextures[i + bodyData.Scales.Length] = weaponData.Animations[0].Frames[0].State[i].Texture;
                joinedOffsets[i + bodyData.Scales.Length] = weaponData.Offsets[i];
            }

            Spritebatch2D sprite;
            Compiler.Compile(joinedInitialTextures, joinedScales, joinedOffsets, textures, Owner, out baseVertices, out sprite);
            Owner.World.Renderer.AddTexture(Owner.Species, sprite);

            currentBodyAnimation.Reset( );
            currentWeaponAnimation.Reset( );
        }

        public override void Update (DeltaTime dt) {
            if (!Owner.IsOnScreen) return;

            if (!currentBodyAnimation.IsRunning) {
                string callbackResult = currentBodyAnimationCallback?.Invoke(true);
                if (callbackResult != null) {
                    SetBodyAnimation(callbackResult);
                } else if (currentBodyAnimation.CanRepeat) {
                    currentBodyAnimation.IsRunning = true;
                } else {
                    currentBodyAnimationIndex = 0;
                    currentBodyAnimation.Reset( );
                }
            }

            if (!currentWeaponAnimation.IsRunning) {
                string callbackResult = currentWeaponAnimationCallback?.Invoke(true);
                if (callbackResult != null) {
                    SetWeaponAnimation(callbackResult);
                    currentWeaponAnimationCallback = null;
                } else if (currentWeaponAnimation.CanRepeat) {
                    currentWeaponAnimation.IsRunning = true;
                } else {
                    currentWeaponAnimationCallback = null;
                    currentWeaponAnimationIndex = 0;
                    currentWeaponAnimation.Reset( );
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
                        currentWeaponAnimationCallback?.Invoke(false);
                        SetWeaponAnimation((string)data[1]);
                        currentWeaponAnimationCallback = callback;
                        break;
                }
            }

            currentBodyAnimation.Update(dt.TotalMilliseconds, Owner.Transform, Owner.World.VertexSize, baseVertices, 0);
            currentWeaponAnimation.Update(dt.TotalMilliseconds, Owner.Transform, Owner.World.VertexSize, baseVertices, currentBodyAnimation.Verticies.Length);
        }

        public override void Draw ( ) {
            if (!Owner.IsOnScreen) return;
            for (int i = 0; i < currentBodyAnimation.Verticies.Length; i++) {
                joinedVertices[i] = currentBodyAnimation.Verticies[i];
                joinedTextures[i] = currentBodyAnimation.Textures[i];
            }
            for (int i = 0; i < currentWeaponAnimation.Verticies.Length; i++) {
                joinedVertices[i + currentBodyAnimation.Verticies.Length] = currentWeaponAnimation.Verticies[i];
                joinedTextures[i + currentBodyAnimation.Verticies.Length] = currentWeaponAnimation.Textures[i];
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
            currentBodyAnimationIndex = index;
            currentBodyAnimation.Reset( );
        }

        private void SetWeaponAnimation (string name) {
            int index = 0;
            for (int i = 0; i < weaponAnimations.Length; i++) {
                if (weaponAnimations[i].Name == name) {
                    index = i;
                    break;
                }
            }
            currentWeaponAnimationIndex = index;
            currentWeaponAnimation.Reset( );
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new PlayerAnimationComponent(owner);
            }
        }
    }
}
