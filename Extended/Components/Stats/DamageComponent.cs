using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components.Stats {

    public class DamageComponent : Component {
        public float OnTouch;

        public DamageComponent (Entity owner, float ontouch) : base(owner) {
            OnTouch = ontouch;
        }

        public new class Configuration : Component.Configuration {
            public float OnTouch;

            public override Component Create (Entity owner) {
                return new DamageComponent(owner, OnTouch);
            }
        }
    }
}