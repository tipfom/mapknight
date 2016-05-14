namespace mapKnight.Android.ECS.Components.Configs {
    class CollisionComponentConfig : ComponentConfig {
        public override int Priority { get { return 0; } }

        public override Component Create (Entity owner) {
            return new CollisionComponent (owner);
        }
    }
}