using mapKnight.Basic.Components;
using System.Collections.Generic;

namespace mapKnight.Android.Entity.Components.Configs {
    public class AnimationComponentConfig : Component.Config {
        public override int Priority { get { return 2; } }
        public List<Animation> Animations;

        public override Component Create (Entity owner) {
            return new AnimationComponent (owner, Animations);
        }
    }
}