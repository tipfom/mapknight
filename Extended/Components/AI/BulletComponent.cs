using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(MotionComponent))]
    [ComponentRequirement(typeof(SpeedComponent))]
    public class BulletComponent : Component {
        private MotionComponent motionComponent;
        private Entity target;

        public BulletComponent (Entity owner, Entity target) : base(owner) {
            this.target = target;
        }

        public event Func<Entity, bool> Hit;

        public override void Collision (Entity collidingEntity) {
            if (!collidingEntity.Info.IsTemporary && (Hit?.Invoke(collidingEntity) ?? true)) {
                DamageComponent damageComponent = Owner.GetComponent<DamageComponent>( );
                if (damageComponent != null) collidingEntity.SetComponentInfo(ComponentEnum.Stats_Health, damageComponent.OnTouch);
                Owner.Destroy( );
            }
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            SpeedComponent speedComponent = Owner.GetComponent<SpeedComponent>( );
            float c = target.Transform.Center.Y - Owner.Transform.Center.Y; // distance y axis
            float d = target.Transform.Center.X - Owner.Transform.Center.X; // distance x axis
            if (float.IsNaN(c) || float.IsNaN(d)) {
                Owner.Destroy( );
                return;
            }
            float t = Math.Abs(d) / speedComponent.Speed.X; // time
            float vx = speedComponent.Speed.X * Math.Sign(d);
            float vy = 0;
            if (Owner.HasComponent<GravityComponent>( ))
                vy = (c - 0.5f * Owner.World.Gravity.Y * Owner.GetComponent<GravityComponent>( ).Influence * t * t) / t;
            motionComponent.Velocity = new Vector2(vx, vy);
        }

        public override void Update (DeltaTime dt) {
            if (motionComponent.IsAtWall || motionComponent.IsOnGround)
                Owner.Destroy( );
        }

        public new class Configuration : Component.Configuration {
            public Entity Target;

            public override Component Create (Entity owner) {
                return new BulletComponent(owner, Target);
            }
        }
    }
}