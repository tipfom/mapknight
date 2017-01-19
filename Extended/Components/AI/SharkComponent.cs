using System;
using mapKnight.Core;
using mapKnight.Extended.Components.AI.Basics;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {
    [UpdateAfter(typeof(MotionComponent))]
    [UpdateAfter(typeof(SpeedComponent))]
    [ComponentRequirement(typeof(TriggerComponent))]
    [UpdateBefore(typeof(SpriteComponent))]
    public class SharkComponent : Component {
        private SpeedComponent speedComponent;
        private MotionComponent motionComponent;
        private Entity player;
        private float escapeDistance;
        private bool isBouncing;

        public SharkComponent (Entity owner, float escapeDistance) : base(owner) {
            owner.Domain = EntityDomain.Enemy;
            this.escapeDistance = escapeDistance;
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            Owner.GetComponent<TriggerComponent>( ).Triggered += SharkComponent_Triggered;
        }

        private void SharkComponent_Triggered (Entity entity) {
            if (entity.Domain == EntityDomain.Player) {
                player = entity;
            }
        }

        public override void Update (DeltaTime dt) {
            if (motionComponent.IsOnGround && !isBouncing) {
                isBouncing = true;
                motionComponent.AimedVelocity.X = 0;
                motionComponent.AimedVelocity.Y = 0;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "bounce", true, (SpriteComponent.AnimationCallback)BounceFinishedAnimationCallback);
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "fly", false);
            }
        }

        private void BounceFinishedAnimationCallback (bool success) {
            motionComponent.AimedVelocity.Y = speedComponent.Speed.Y;
            isBouncing = false;

            if (player != null) {
                float c = player.Transform.Center.Y - Owner.Transform.Y; // distance y axis
                float d = player.Transform.Center.X - Owner.Transform.X; // distance x axis
                if (Math.Abs(d) > escapeDistance) {
                    player = null;
                    return;
                }
                float a = (motionComponent.GravityInfluence * Owner.World.Gravity.Y);
                float va = speedComponent.Speed.Y / a;
                float t = -va + Mathf.Sqrt(va * va - c / a); // time
                float vx = d / t;
                motionComponent.AimedVelocity.X = Mathf.Clamp(vx, -speedComponent.Speed.X, speedComponent.Speed.X);
            }
        }

        public new class Configuration : Component.Configuration {
            public float EscapeDistance;

            public override Component Create (Entity owner) {
                return new SharkComponent(owner, EscapeDistance);
            }
        }
    }
}