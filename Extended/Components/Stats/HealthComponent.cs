using System;
using mapKnight.Core;

namespace mapKnight.Extended.Components.Stats {

    public class HealthComponent : Component {
        public readonly int Initial;
        public float Current;
        public Func<Entity, bool> IsHit; 

        private ArmorComponent armorComponent;

        public HealthComponent (Entity owner, int health) : base(owner) {
            Initial = health;
            Current = health;
        }

        public override void Prepare ( ) {
            if (Owner.HasComponent<ArmorComponent>())
                armorComponent = Owner.GetComponent<ArmorComponent>( );
        }

        public override void Update (DeltaTime dt) {
            while (Owner.HasComponentInfo(ComponentData.Damage)) {
                object[] data = Owner.GetComponentInfo(ComponentData.Damage);
                if (IsHit?.Invoke((Entity)data[0]) ?? true) {
                    Current -= ((float)data[1] * ((armorComponent != null) ? armorComponent.PhysicalMultiplier : 1f));
                    if (Current < 0)
                        Owner.Destroy( );
                }
            }
        }

        public new class Configuration : Component.Configuration {
            public int Value;

            public override Component Create (Entity owner) {
                return new HealthComponent(owner, Value);
            }
        }
    }
}