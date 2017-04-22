using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Graphics.UI;

namespace mapKnight.Extended.Components.Stats {
    public class HealthComponent : Component {
        public readonly int Initial;
        private float _Current;
        public float Current { get { return _Current; } set { _Current = value; Changed?.Invoke( ); } }
        public event Action Changed;
        public Func<Entity, bool> IsHit;

        private ArmorComponent armorComponent;
        private UIHealthBar healthBar;

        public HealthComponent(Entity owner, int health) : base(owner) {
            Initial = health;
            Current = health;
            healthBar = new UIHealthBar(Screen.Gameplay, this);
        }

        public override void Prepare( ) {
            armorComponent = Owner.GetComponent<ArmorComponent>( );
        }

        public override void Update(DeltaTime dt) {
            while (Owner.HasComponentInfo(ComponentData.Damage)) {
                object[ ] data = Owner.GetComponentInfo(ComponentData.Damage);
                if (IsHit?.Invoke((Entity)data[0]) ?? true) {
                    Current -= ((float)data[1] * ((armorComponent != null) ? armorComponent.PhysicalMultiplier : 1f));
                    if (Current <= 0)
                        Owner.Destroy( );
                }
            }
        }

        public new class Configuration : Component.Configuration {
            public int Value;

            public override Component Create(Entity owner) {
                return new HealthComponent(owner, Value);
            }
        }
    }
}