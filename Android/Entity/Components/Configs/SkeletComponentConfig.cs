using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.Entity.Components.Configs {
    public class SkeletComponentConfig : Component.Config {
        public override int Priority { get { return 1; } }

        public Dictionary<string, Rectangle> Bones;

        public override Component Create (Entity owner) {
            return new SkeletComponent (owner, Bones);
        }
    }
}