using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {
    [UpdateAfter(typeof(MotionComponent))]
    [UpdateAfter(typeof(SpeedComponent))]
    [UpdateBefore(typeof(SpriteComponent))]
    public class SharkComponent : Component {
        private SpeedComponent speedComponent;
        private MotionComponent motionComponent;
        private bool isBouncing;

        public SharkComponent (Entity owner) : base(owner) {

        }

        public override void Prepare ( ) {
            motionComponent = Owner.GetComponent<MotionComponent>( );
            speedComponent = Owner.GetComponent<SpeedComponent>( );
        }

        public override void Update (DeltaTime dt) {
            if(motionComponent.IsOnGround && !isBouncing) {
                isBouncing = true;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "bounce", true, (SpriteComponent.AnimationCallback)BounceFinishedAnimationCallback);
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "fly", false);
            }
        }

        private void BounceFinishedAnimationCallback(bool success) {
            motionComponent.AimedVelocity.Y = speedComponent.Speed.Y;
            isBouncing = false;
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new SharkComponent(owner);
            }
        }
    }
}