using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Components.Graphics;

namespace mapKnight.Extended.Components.AI.Guardian {
    [ComponentRequirement(typeof(MotionComponent))]
    [ComponentRequirement(typeof(SpeedComponent))]
    public class OfficerComponent : Component {
        public Entity Target;
        private TentComponent tent;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private int lastWalkingTime;
        private int turnTime;
        private bool walking;

        public OfficerComponent(Entity owner, TentComponent tent, int turnTime) : base(owner) {
            owner.Domain = EntityDomain.Enemy;

            this.tent = tent;
            this.turnTime = turnTime;
        }

        public override void Prepare( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            Owner.GetComponent<HealthComponent>( ).IsHit = (entity) => {
                // check for the sign
                return (entity.Transform.X - Owner.Transform.X) * motionComponent.ScaleX < 0;
            };
            walking = false;
        }

        public override void Update(DeltaTime dt) {
            if (Owner.Transform.Center.X >= Target.Transform.Center.X) {
                // walk left
                if (Owner.Transform.BL.X >= Target.Transform.TR.X &&
                    (Owner.World.HasCollider(Mathi.Floor(Owner.Transform.BL.X), Mathi.Floor(Owner.Transform.BL.Y) - 1) ||
                     Owner.World.HasCollider(Mathi.Floor(Owner.Transform.BL.X), Mathi.Floor(Owner.Transform.BL.Y) - 2))) {
                    if (motionComponent.AimedVelocity.X < 0 || Environment.TickCount - lastWalkingTime > turnTime) {
                        motionComponent.AimedVelocity.X = -speedComponent.Speed.X;
                        lastWalkingTime = Environment.TickCount;
                        if (!walking) {
                            Owner.SetComponentInfo(ComponentData.SpriteAnimation, "walk", true);
                            walking = true;
                        }
                    }
                } else {
                    motionComponent.AimedVelocity.X = 0;
                    if (motionComponent.ScaleX < 0)
                        lastWalkingTime = Environment.TickCount;
                    else if (Environment.TickCount - lastWalkingTime > turnTime)
                        motionComponent.ScaleX = -1;
                    walking = false;
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "def", true);
                }
            } else {
                // walk right
                if (Owner.Transform.TR.X <= Target.Transform.BL.X &&
                    (Owner.World.HasCollider(Mathi.Floor(Owner.Transform.TR.X), Mathi.Floor(Owner.Transform.BL.Y) - 1) ||
                     Owner.World.HasCollider(Mathi.Floor(Owner.Transform.TR.X), Mathi.Floor(Owner.Transform.BL.Y) - 2))) {
                    if (motionComponent.AimedVelocity.X > 0 || Environment.TickCount - lastWalkingTime > turnTime) {
                        motionComponent.AimedVelocity.X = speedComponent.Speed.X;
                        lastWalkingTime = Environment.TickCount;
                        if (!walking) {
                            Owner.SetComponentInfo(ComponentData.SpriteAnimation, "walk", true);
                            walking = true;
                        }
                    }
                } else {
                    motionComponent.AimedVelocity.X = 0;
                    if (motionComponent.ScaleX > 0)
                        lastWalkingTime = Environment.TickCount;
                    else if (Environment.TickCount - lastWalkingTime > turnTime)
                        motionComponent.ScaleX = 1;
                    walking = false;
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "def", true);
                }
            }

            if (motionComponent.IsOnGround) {
                motionComponent.AimedVelocity.Y = 0;
            }
            if (motionComponent.IsAtWall) {
                // v = g * t = g * sqrt(2*h / g) = sqrt(2*h*g); h=1.5
                motionComponent.AimedVelocity.Y = Mathf.Sqrt(3f * -Owner.World.Gravity.Y);
            }
        }

        public void ReturnHome( ) {
            Target = tent.Owner;
        }

        public override void Destroy( ) {
            tent.OfficerDied( );
        }

        public new class Configuration : Component.Configuration {
            public TentComponent Tent;
            public int TurnTime;

            public override Component Create(Entity owner) {
                return new OfficerComponent(owner, Tent, TurnTime);
            }
        }
    }
}
