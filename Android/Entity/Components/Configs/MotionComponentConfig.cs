namespace mapKnight.Android.Entity.Components.Configs {
    public class MotionComponentConfig : Component.Config {
        public override int Priority { get { return 1; } }

        public override Component Create (Entity owner) {
            return new MotionComponent (owner);
        }
    }
}