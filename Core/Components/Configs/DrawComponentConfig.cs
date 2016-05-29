namespace mapKnight.Core.Components.Configs {
    public class DrawComponentConfig : ComponentConfig {
        public override int Priority { get { return 0; } }

        public override Component Create (Entity owner) {
            return new DrawComponent (owner);
        }
    }
}