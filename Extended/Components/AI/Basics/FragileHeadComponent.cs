using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI.Basics {

    [ComponentRequirement(typeof(DamageComponent))]
    public class FragileHeadComponent: Component {
        private DamageComponent damageComponent;

        public FragileHeadComponent (Entity owner) : base(owner) {
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Info.IsPlayer) {
                if (collidingEntity.Transform.BL.Y > Owner.Transform.Center.Y) {
                    Owner.Destroy( );
                } else {
                    collidingEntity.SetComponentInfo(ComponentData.Damage, damageComponent.OnTouch);
                }
            }
        }

        public override void Prepare ( ) {
            damageComponent = Owner.GetComponent<DamageComponent>( );
        }

        public new class Configuration : Component.Configuration {

            public override Component Create (Entity owner) {
                return new FragileHeadComponent(owner);
            }
        }
    }
}