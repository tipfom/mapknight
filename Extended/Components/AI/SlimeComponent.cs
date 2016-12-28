using System;
using mapKnight.Core;

namespace mapKnight.Extended.Components.AI {
    public class SlimeComponent : Component {
        byte mode = 0;
        byte r = 255, g = 0, b = 0;

        public SlimeComponent (Entity owner) : base(owner) {

        }

        public override void Update (DeltaTime dt) {
            switch (mode) {
                case 0:
                    r--;
                    g++;
                    if(g == 255) {
                        mode++;
                    }
                    break;
                case 1:
                    g--;
                    b++;
                    if(b == 255) {
                        mode++;
                    }
                    break;
                case 2:
                    b--;
                    r++;
                    if(r == 255) {
                        mode = 0;
                    }
                    break;
            }
            Owner.SetComponentInfo(ComponentData.Color, new Color(r,g,b));
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new SlimeComponent(owner);
            }
        }
    }
}
