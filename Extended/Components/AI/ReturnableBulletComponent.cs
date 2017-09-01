using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Combat;

namespace mapKnight.Extended.Components.AI {
    class ReturnableBulletComponent : Component {
        public bool Returned;
        private float returnSpeed;
        private float damage;
        private int counter = 0;
        private MotionComponent motionComponent;

        public ReturnableBulletComponent (Entity owner, float damage, float returnSpeed) : base(owner) {
            owner.Domain = EntityDomain.Enemy;

            this.returnSpeed = returnSpeed;
            this.damage = damage;
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.Player) {
                if (counter == 0) collidingEntity.SetComponentInfo(ComponentData.Damage, Owner, damage, DamageType.Magical);
                counter = 2;
                Returned = true;
                Vector2 dir = (Owner.Transform.Center - collidingEntity.Transform.Center).Normalize( );
                Vector2 speed = collidingEntity.GetComponent<MotionComponent>( ).Velocity + dir * returnSpeed + (motionComponent.AimedVelocity - motionComponent.Velocity) * .7f;
                motionComponent.AimedVelocity = speed;
            }
        }

        public override void Update (DeltaTime dt) {
            if (counter > 0) counter--;
            if (motionComponent.IsAtWall || motionComponent.IsOnGround) Owner.Destroy( );
            base.Update(dt);
        }

        public new class Configuration : Component.Configuration {
            public float ReturnSpeed;
            public float Damage;

            public override Component Create (Entity owner) {
                return new ReturnableBulletComponent(owner, Damage, ReturnSpeed);
            }
        }
    }
}
