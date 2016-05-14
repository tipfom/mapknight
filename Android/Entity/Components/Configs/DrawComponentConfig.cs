namespace mapKnight.Android.Entity.Components.Configs {
    public class DrawComponentConfig : Component.Config {
        public override int Priority { get { return 0; } }

        public override Component Create (Entity owner) {
            return new DrawComponent (owner);
        }
    }
}