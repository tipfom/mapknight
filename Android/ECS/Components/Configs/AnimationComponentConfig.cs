using mapKnight.Basic.Components;
using System.Collections.Generic;

namespace mapKnight.Android.ECS.Components.Configs {
    public class AnimationComponentConfig : ComponentConfig {
        public override int Priority { get { return 2; } }
        public List<Animation> Animations;

        public override Component Create (Entity owner) {
            return new AnimationComponent (owner, Animations);
        }
    }
}