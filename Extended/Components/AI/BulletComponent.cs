using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Combat;
using mapKnight.Extended.Components.Movement;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(MotionComponent))]
    public class BulletComponent : Component {
        protected MotionComponent motionComponent;
        private float damage;
        private DamageType damageType;

        public BulletComponent (Entity owner, float damage, DamageType damageType) : base(owner) {
            owner.Domain = EntityDomain.Temporary;

            this.damage = damage;
            this.damageType = damageType;
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.Player) {
                collidingEntity.SetComponentInfo(ComponentData.Damage, Owner, damage, damageType);
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
            public DamageType DamageType;

            public override Component Create (Entity owner) {
                return new BulletComponent(owner, Damage, DamageType);
            }
        }
    }
}