using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(DamageComponent))]
    public class GrinderComponent : Component {
        private DamageComponent damageComponent;

        public GrinderComponent (Entity owner) : base(owner) {
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Info.IsPlayer) {
                if (collidingEntity.Transform.BL.Y > Owner.Transform.Center.Y) {
                    Owner.Destroy( );
                } else {
                    collidingEntity.SetComponentInfo(ComponentEnum.Stats_Health, damageComponent.OnTouch);
                }
            }
        }

        public override void Prepare ( ) {
            damageComponent = Owner.GetComponent<DamageComponent>( );
        }

        public new class Configuration : Component.Configuration {

            public override Component Create (Entity owner) {
                return new GrinderComponent(owner);
            }
        }
    }
}