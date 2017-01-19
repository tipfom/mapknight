using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI.Basics {

    public class FragileHeadComponent: Component {
        private float damage;

        public FragileHeadComponent (Entity owner, float damage) : base(owner) {
            this.damage = damage;
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.Player) {
                if (collidingEntity.Transform.BL.Y > Owner.Transform.Center.Y) {
                    Owner.Destroy( );
                } else {
                    collidingEntity.SetComponentInfo(ComponentData.Damage, damage);
                }
            }
        }

        public override void Prepare ( ) {
        }

        public new class Configuration : Component.Configuration {
            public float Damage;

            public override Component Create (Entity owner) {
                return new FragileHeadComponent(owner, Damage);
            }
        }
    }
}