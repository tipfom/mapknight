using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using System;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(MotionComponent))]
    public class MoonballComponent : Component {
        private float boostVelocity;
        private float damagePerSecond;
        private MotionComponent motionComponent;
        private Entity damagingEntity;

        public MoonballComponent (Entity owner, float boostVelocity, float damagePerSecond) : base(owner) {
            this.boostVelocity = boostVelocity;
            this.damagePerSecond = damagePerSecond;
        }

        public override void Collision(Entity collidingEntity) {
            if(collidingEntity.Domain == EntityDomain.Player) {
                damagingEntity = collidingEntity;
            }
        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            motionComponent.AimedVelocity.X = -boostVelocity;
        }

        public override void Update (DeltaTime dt) {
            if (motionComponent.IsAtWall)
                Owner.Destroy( );

            if(damagingEntity != null) {
                damagingEntity.SetComponentInfo(ComponentData.Damage, damagePerSecond * dt.TotalSeconds);
                damagingEntity = null;
            }
        }

        public new class Configuration : Component.Configuration {
            public float BoostVelocity;
            public float DamagePerSecond;

            public override Component Create (Entity owner) {
                return new MoonballComponent(owner, BoostVelocity, DamagePerSecond);
            }
        }
    }
}
