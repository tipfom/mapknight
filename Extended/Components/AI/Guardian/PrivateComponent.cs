using System;
using mapKnight.Core;
using mapKnight.Extended.Components.AI.Basics;

namespace mapKnight.Extended.Components.AI.Guardian {
    public class PrivateComponent : BishopComponent {
        private TentComponent tent;
        private float patrolDistanceSqr;

        public PrivateComponent (Entity owner, TentComponent tent) : base(owner, true) {
            owner.Domain = EntityDomain.Enemy;

            this.tent = tent;
            this.patrolDistanceSqr = tent.PatrolRangeSqr;
        }

        public override void Update (DeltaTime dt) {
            if ((Owner.Transform.Center - tent.Owner.Transform.Center).MagnitudeSqr( ) > patrolDistanceSqr) {
                Turn( );
            }
            base.Update(dt);
        }

        public override void Destroy ( ) {
            tent.PrivateDied(Owner);
            base.Destroy( );
        }

        public new class Configuration : Component.Configuration {
            public TentComponent Tent;

            public override Component Create (Entity owner) {
                return new PrivateComponent(owner, Tent);
            }
        }
    }
}
