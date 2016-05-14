namespace mapKnight.Android.Entity.Components {
    public class GravityComponent : Component {
        private float gravityInfluence;

        public GravityComponent (Entity owner, float gravityinfluence) : base (owner) {
            this.gravityInfluence = gravityinfluence;
        }

        public override void Update (float dt) {
            // say to the collisioncomponent to use a part of the global gravity when calculating movement
            // that the motioncomponent is contained by the entity needs to be handled by dependencies
            Owner.SetComponentInfo (ComponentType.Motion, ComponentType.Gravity, ComponentAction.Acceleration, Owner.Owner.Gravity * gravityInfluence);
        }
    }
}