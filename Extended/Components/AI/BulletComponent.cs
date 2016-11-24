using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(MotionComponent))]
    public class BulletComponent : Component {
        private MotionComponent motionComponent;

        public BulletComponent (Entity owner) : base(owner) {
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Info.IsPlayer) {
                DamageComponent damageComponent = Owner.GetComponent<DamageComponent>( );
                if (damageComponent != null) collidingEntity.SetComponentInfo(ComponentData.Damage, damageComponent.OnTouch);
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
            public override Component Create (Entity owner) {
                return new BulletComponent(owner);
            }
        }
    }
}