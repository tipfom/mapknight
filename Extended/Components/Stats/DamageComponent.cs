using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components.Stats {
    public class DamageComponent : Component {
        public int OnTouch;

        public DamageComponent (Entity owner, int ontouch) : base(owner) {
            OnTouch = ontouch;
        }

        public new class Configuration : Component.Configuration {
            public int OnTouch;

            public override Component Create (Entity owner) {
                return new DamageComponent(owner, OnTouch);
            }
        }
    }
}
