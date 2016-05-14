namespace mapKnight.Android.Entity.Components.Configs {
    class CollisionComponentConfig : Component.Config {
        public override int Priority { get { return 0; } }

        public override Component Create (Entity owner) {
            return new CollisionComponent (owner);
        }
    }
}