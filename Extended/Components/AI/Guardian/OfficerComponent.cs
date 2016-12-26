using System;

namespace mapKnight.Extended.Components.AI.Guardian {
    public class OfficerComponent : Component {
        private Entity tent;

        public OfficerComponent (Entity owner, Entity tent) : base(owner) {
            this.tent = tent;
        }

        public void ReturnHome ( ) {

        }

        public override void Destroy ( ) {
            
        }

        public new class Configuration : Component.Configuration {
            public Entity Tent;

            public override Component Create (Entity owner) {
                return new OfficerComponent(owner, Tent);
            }
        }
    }
}
