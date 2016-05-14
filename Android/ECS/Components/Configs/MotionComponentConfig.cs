namespace mapKnight.Android.ECS.Components.Configs {
    public class MotionComponentConfig : ComponentConfig {
        public override int Priority { get { return 1; } }

        public override Component Create (Entity owner) {
            return new MotionComponent (owner);
        }
    }
}