using mapKnight.Core;
using System;
using System.Collections.Generic;

namespace mapKnight.Extended.Components.AI.Guardian {
    public class TentComponent : Component {
        private Entity.Configuration officer;
        private Entity.Configuration[ ] privates;
        public float PatrolRangeSqr;
        private int maxPrivateCount;
        private int timeBetweenPrivates;
        private int nextPrivateSpawnTime;
        private Entity enemy;
        private OfficerComponent activeOfficer;
        private List<Entity> activePrivates;

        public TentComponent (Entity owner, Entity.Configuration officer, Entity.Configuration[ ] privates, float patrolRange, int maxPrivateCount, int timeBetweenPrivates) : base(owner) {
            this.officer = officer;
            this.privates = privates;
            this.PatrolRangeSqr = patrolRange * patrolRange;
            this.maxPrivateCount = maxPrivateCount;
            this.timeBetweenPrivates = timeBetweenPrivates;
            this.activePrivates = new List<Entity>(maxPrivateCount);

            this.officer.Components.GetConfiguration<OfficerComponent.Configuration>( ).Tent = this;
            foreach (Entity.Configuration c in this.privates) {
                c.Components.GetConfiguration<PrivateComponent.Configuration>( ).Tent = this;
            }
        }

        public override void Prepare ( ) {
            nextPrivateSpawnTime = Environment.TickCount + timeBetweenPrivates;
            base.Prepare( );
        }

        public override void Tick ( ) {
            if (enemy != null && (Owner.Transform.Center - enemy.Transform.Center).MagnitudeSqr( ) > PatrolRangeSqr) {
                enemy = null;
                activeOfficer.ReturnHome( );
            }
            if (Environment.TickCount > nextPrivateSpawnTime) {
                activePrivates.Add(RandomPrivate( ));
                if (activePrivates.Count >= maxPrivateCount) {
                    nextPrivateSpawnTime = int.MaxValue;
                } else {
                    nextPrivateSpawnTime += timeBetweenPrivates;
                }
            }
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Info.IsPlayer) {
                if (activeOfficer == null) {
                    activeOfficer = officer.Create(Owner.Transform.Center, Owner.World).GetComponent<OfficerComponent>( );
                    activeOfficer.Target = collidingEntity;
                }
            }
        }

        public void PrivateDied (Entity e) {
            activePrivates.Remove(e);
            nextPrivateSpawnTime = Environment.TickCount + timeBetweenPrivates;
        }

        public void OfficerDied ( ) {
            if (activeOfficer != null)
                Owner.Destroy( );
        }

        private Entity RandomPrivate ( ) {
            Entity.Configuration randomedConfig = privates[Mathi.Random(0, privates.Length)];
            return randomedConfig.Create(Owner.Transform.Center - new Vector2(0, Owner.Transform.HalfSize.Y - randomedConfig.Transform.HalfSize.Y), Owner.World);
        }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration Officer;
            public Entity.Configuration[ ] Privates;
            public float PatrolRange;
            public int PrivateCount;
            public int TimeBetweenPrivates;

            public override Component Create (Entity owner) {
                return new TentComponent(owner, Officer, Privates, PatrolRange, PrivateCount, TimeBetweenPrivates);
            }
        }
    }
}
