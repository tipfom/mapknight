using System;
using mapKnight.Core;

namespace mapKnight.Extended.Components.AI {

    public class TriggerComponent : Component {
        private static Entity.Configuration triggerEntityConfiguration;

        private Entity triggerEntity;

        static TriggerComponent ( ) {
            triggerEntityConfiguration = new Entity.Configuration( ) { Components = new ComponentList( ), Name = "triggerEntity" };
            triggerEntityConfiguration.Components.Add(new InternalTriggerComponent.Configuration( ));
        }

        public TriggerComponent (Entity owner, Vector2 triggerZone, float offset) : base(owner) {
            ((InternalTriggerComponent.Configuration)triggerEntityConfiguration.Components[0]).FollowEntity = Owner;
            ((InternalTriggerComponent.Configuration)triggerEntityConfiguration.Components[0]).TriggerOffset = offset;
            triggerEntityConfiguration.Transform = new Transform(new Vector2(0, 0), triggerZone);
            triggerEntity = triggerEntityConfiguration.Create(new Vector2(0, 0), Owner.World);
            triggerEntity.GetComponent<InternalTriggerComponent>( ).Triggered += (Entity obj) => { Triggered?.Invoke(obj); };
        }

        public event Action<Entity> Triggered;

        public override void Destroy ( ) {
            triggerEntity.Destroy( );
            base.Destroy( );
        }

        public new class Configuration : Component.Configuration {
            public float Offset;
            public Vector2 TriggerZone;

            public override Component Create (Entity owner) {
                return new TriggerComponent(owner, TriggerZone, Offset);
            }
        }

        private class InternalTriggerComponent : Component {
            private Entity followingEntity;
            private float triggerOffset;

            public InternalTriggerComponent (Entity owner, Entity followingentity, float triggeroffset) : base(owner) {
                followingEntity = followingentity;
                triggerOffset = triggeroffset;
            }

            public event Action<Entity> Triggered;

            public override void Collision (Entity collidingEntity) {
                if (collidingEntity != followingEntity)
                    Triggered?.Invoke(collidingEntity);
            }

            public override void Update (DeltaTime dt) {
                Owner.Transform.Center = new Vector2(followingEntity.Transform.Center.X, followingEntity.Transform.Center.Y + triggerOffset);
            }

            public new class Configuration : Component.Configuration {
                public Entity FollowEntity;
                public float TriggerOffset;

                public override Component Create (Entity owner) {
                    return new InternalTriggerComponent(owner, FollowEntity, TriggerOffset);
                }
            }
        }
    }
}