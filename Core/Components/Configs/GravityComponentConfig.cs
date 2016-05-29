namespace mapKnight.Core.Components.Configs {
    public class GravityComponentConfig : ComponentConfig {
        public override int Priority { get { return 2; } }

        public float Influence;

        public override Component Create (Entity owner) {
            return new GravityComponent (owner, Influence);
        }
    }
}