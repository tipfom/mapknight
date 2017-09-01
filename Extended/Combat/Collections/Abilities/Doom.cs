using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Core.World;
using System.Linq;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Graphics.Particles;
using mapKnight.Extended.Graphics.Particles.VelocityProvider;

namespace mapKnight.Extended.Combat.Collections.Abilities {
    public class Doom : Ability {
        private const int TIME_TO_KILL = 10000;
        private const float LIFEGAIN_MULTIPLIER = .4f;
        private const float RETURN_DAMAGE_MULTIPLIER = .8f;

        private static readonly Vector2[ ] PREVIEW = new Vector2[ ] {
            new Vector2(0.5f, 0.0f),
            new Vector2(0.8148f, 1.0f),
            new Vector2(0.0f, 0.3774f),
            new Vector2(1.0f, 0.3774f),
            new Vector2(0.1852f, 1.0f),
            new Vector2(0.5f, 0.0f),
        };

        public Doom (SecondaryWeapon Weapon) : base(Weapon, "Doom", 20000, "abil_dash", PREVIEW) {
        }

        protected override void OnCast (float gestureSuccess) {
            Entity targetEntity = FindEntityToDoom( );
            if (targetEntity != null) {
                List<Component> componentsAsList = targetEntity.Components.ToList( );
                componentsAsList.Add(new DoomComponent(targetEntity, DoomSuccessCallback));
                targetEntity.Components = componentsAsList.ToArray( );
            }
        }

        private Entity FindEntityToDoom ( ) {
            float currentClosest = float.PositiveInfinity;
            Entity currentEntity = null;

            foreach (Entity entity in Weapon.Owner.World.Entities) {
                if (entity.HasComponent<HealthComponent>( )) {
                    float dist = (Weapon.Owner.Transform.Center - entity.Transform.Center).MagnitudeSqr( );
                    if (dist < currentClosest) {
                        currentClosest = dist;
                        currentEntity = entity;
                    }
                }
            }

            return currentEntity;
        }

        private void DoomSuccessCallback (bool success, float hp) {
            if (success) {
                Weapon.Owner.SetComponentInfo(ComponentData.Heal, LIFEGAIN_MULTIPLIER * hp);
            } else {
                Weapon.Owner.SetComponentInfo(ComponentData.Damage, Weapon.Owner, RETURN_DAMAGE_MULTIPLIER * hp, DamageType.Pure);
            }
            EndCast( );
        }

        private class DoomComponent : Component {
            private int timeToKillLeft = TIME_TO_KILL;
            private Action<bool, float> successCallback;
            private float entityHp;
            private Emitter emitter;
            private bool disabled;

            public DoomComponent (Entity owner, Action<bool, float> successCallback) : base(owner) {
                this.successCallback = successCallback;
                entityHp = Owner.GetComponent<HealthComponent>( ).Initial;

                emitter = new Emitter( ) {
                    Color = new Range<Color>(Color.Red, Color.Red),
                    Count = 40,
                    Lifetime = new Range<int>(300, 700),
                    Gravity = Vector2.Zero,
                    Position = GetEmitterPosition( ),
                    RespawnParticles = true,
                    Size = new Range<float>(10f, 20f),
                    VelocityProvider = new CartesianVelocityProvider(0f, 0f, 3f, 7f)
                };
                emitter.Setup( );
                Owner.World.Renderer.AddParticles(emitter);
            }

            public override void Update (DeltaTime dt) {
                if (!disabled) {
                    timeToKillLeft -= (int)dt.TotalMilliseconds;
                    emitter.Position = GetEmitterPosition( );
                    if (timeToKillLeft < 0) {
                        successCallback(false, entityHp);
                        disabled = true;
                        Owner.World.Renderer.RemoveParticles(emitter);
                        emitter.Dispose( );
                    }
                }
            }

            public override void Destroy ( ) {
                if (!disabled) {
                    successCallback(true, entityHp);
                    Owner.World.Renderer.RemoveParticles(emitter);
                    emitter.Dispose( );
                }
            }

            private Vector2 GetEmitterPosition ( ) {
                return new Vector2(Owner.Transform.Center.X, Owner.Transform.TR.Y + 1);
            }
        }
    }
}
