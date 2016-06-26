using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;

namespace mapKnight.Extended.Components.Configs {
    public class SpeedComponentConfig : ComponentConfig {
        public override int Priority { get { return 3; } }

        public float X;
        public float Y;

        public override Component Create (Entity owner) {
            return new SpeedComponent(owner, new Vector2(X, Y));
        }
    }
}
