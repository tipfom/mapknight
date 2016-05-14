namespace mapKnight.Android.Entity.Components.Configs {
    public class GravityComponentConfig : Component.Config {
        public override int Priority { get { return 2; } }

        public float Influence;

        public override Component Create (Entity owner) {
            return new GravityComponent (owner, Influence);
        }
    }
}