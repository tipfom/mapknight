using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(MotionComponent))]
    public class BulletComponent : Component {
        private MotionComponent motionComponent;
        private float damage;

        public BulletComponent (Entity owner, float damage) : base(owner) {
            this.damage = damage;
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Info.IsPlayer) {
                collidingEntity.SetComponentInfo(ComponentData.Damage, damage);
                Owner.Destroy( );
            }
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
        }

        public override void Update (DeltaTime dt) {
            if (motionComponent.IsAtWall || motionComponent.IsOnGround)
                Owner.Destroy( );
        }

        public new class Configuration : Component.Configuration {
            public float Damage;

            public override Component Create (Entity owner) {
                return new BulletComponent(owner, Damage);
            }
        }
    }
}