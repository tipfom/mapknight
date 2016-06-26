using mapKnight.Extended.Graphics;
using System.Collections.Generic;
using static mapKnight.Extended.Components.SkeletComponent;

namespace mapKnight.Extended.Components.Configs {
    public class SkeletComponentConfig : ComponentConfig {
        public override int Priority { get { return 1; } }

        public Dictionary<string, Bone> Bones;

        public override Component Create (Entity owner) {
            return new SkeletComponent(owner, Bones);
        }
    }
}