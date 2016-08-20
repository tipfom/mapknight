using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(DamageComponent))]
    public class SpikeComponent : Component {
        private DamageComponent damageComponent;

        public SpikeComponent (Entity owner) : base(owner) {
        }

        public override void Collision (Entity collidingEntity) {
            collidingEntity.SetComponentInfo(ComponentEnum.Stats_Health, damageComponent.OnTouch);
        }

        public override void Prepare ( ) {
            damageComponent = Owner.GetComponent<DamageComponent>( );
        }

        public new class Configuration : Component.Configuration {

            public override Component Create (Entity owner) {
                return new SpikeComponent(owner);
            }
        }
    }
}