using System;
using System.Timers;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI { 

    [ComponentRequirement(typeof(SpeedComponent))]
    [ComponentRequirement(typeof(TriggerComponent))]
    public class HastorComponent : Component {
        private float frenzySpeedPercent;
        private bool hasting;
        private float hastingDirection;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private bool stunned;
        private Entity target;

        public HastorComponent (Entity owner, float frenzyspeedpercent) : base(owner) {
            frenzySpeedPercent = frenzyspeedpercent;

            Owner.SetComponentInfo(ComponentData.BoneTexture, "shell");
            Owner.SetComponentInfo(ComponentData.BoneOffset, Tuple.Create("shell", new Vector2(9.5f, 12.5f)));
        }

        public override void Prepare ( ) {
            Owner.GetComponent<TriggerComponent>( ).Triggered += Trigger_Triggered;
            speedComponent = Owner.GetComponent<SpeedComponent>( );
            motionComponent = Owner.GetComponent<MotionComponent>( );
            hasting = false;
            stunned = false;
        }

        public override void Update (DeltaTime dt) {
            if (motionComponent.IsAtWall && !stunned) {
                hasting = false;
                stunned = true;
                motionComponent.AimedVelocity.X = -hastingDirection * frenzySpeedPercent * speedComponent.Speed.X;
                Owner.SetComponentInfo(ComponentData.VertexAnimation, "frenzy", true, (AnimationComponent.AnimationCallback)AnimationCallbackFrenzy);
            }
        }

        private void Trigger_Triggered (Entity entity) {
            if (!(hasting || stunned) && entity.Info.IsPlayer) {
                target = entity;
                Owner.SetComponentInfo(ComponentData.VertexAnimation, "prepare", true, (AnimationComponent.AnimationCallback)AnimationCallbackPrepare);
            }
        }

        private void AnimationCallbackPrepare(bool success) {
            hastingDirection = (target.Transform.BL.X > Owner.Transform.TR.X) ? 1 : -1;
            motionComponent.AimedVelocity.X = speedComponent.Speed.X * hastingDirection;
            hasting = true;
            Owner.SetComponentInfo(ComponentData.VertexAnimation, "attack", true);
        }

        private void AnimationCallbackFrenzy(bool success) {
            stunned = false;
            motionComponent.AimedVelocity.X = 0;
            Owner.SetComponentInfo(ComponentData.VertexAnimation, "idle", true);
        }

        public new class Configuration : Component.Configuration {
            public float FrenzySpeedPercent;

            public override Component Create (Entity owner) {
                return new HastorComponent(owner, FrenzySpeedPercent);
            }
        }
    }
}