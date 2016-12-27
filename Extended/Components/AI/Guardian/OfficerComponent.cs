using System;

namespace mapKnight.Extended.Components.AI.Guardian {
    public class OfficerComponent : Component {
        private TentComponent tent;

        public OfficerComponent (Entity owner, TentComponent tent) : base(owner) {
            this.tent = tent;
        }

        public void ReturnHome ( ) {

        }

        public override void Destroy ( ) {
            tent.OfficerDied( );
        }

        public new class Configuration : Component.Configuration {
            public TentComponent Tent;

            public override Component Create (Entity owner) {
                return new OfficerComponent(owner, Tent);
            }
        }
    }
}
