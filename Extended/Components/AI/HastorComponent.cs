using System;
using System.Timers;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;

namespace mapKnight.Extended.Components.AI {

    [ComponentRequirement(typeof(GravityComponent))]
    [ComponentRequirement(typeof(SpeedComponent))]
    [ComponentRequirement(typeof(TriggerComponent))]
    public class HastorComponent : Component {
        private float frenzySpeedPercent;
        private bool hasting;
        private float hastingDirection;
        private MotionComponent motionComponent;
        private SpeedComponent speedComponent;
        private bool stunned;
        private Timer stunnedTimer;

        public HastorComponent (Entity owner, int stunduration, float frenzyspeedpercent) : base(owner) {
            stunnedTimer = new Timer(stunduration);
            stunnedTimer.Elapsed += StunnedTimer_Elapsed;
            frenzySpeedPercent = frenzyspeedpercent;
        }

        public override void Destroy ( ) {
            stunnedTimer.Dispose( );
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
                motionComponent.Velocity.X = -hastingDirection * frenzySpeedPercent * speedComponent.Speed.X;
                stunnedTimer.Start( );
            }
        }

        private void StunnedTimer_Elapsed (object sender, ElapsedEventArgs e) {
            stunnedTimer.Stop( );
            stunned = false;
            motionComponent.Velocity.X = 0;
        }

        private void Trigger_Triggered (Entity entity) {
            if (!(hasting || stunned) && entity.Info.IsPlayer) {
                hastingDirection = (entity.Transform.BL.X > Owner.Transform.TR.X) ? 1 : -1;
                motionComponent.Velocity.X = speedComponent.Speed.X * hastingDirection;
                hasting = true;
            }
        }

        public new class Configuration : Component.Configuration {
            public float FrenzySpeedPercent;
            public int StunDuration;

            public override Component Create (Entity owner) {
                return new HastorComponent(owner, StunDuration, FrenzySpeedPercent);
            }
        }
    }
}