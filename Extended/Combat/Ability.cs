using System;
using mapKnight.Core;

namespace mapKnight.Extended.Combat {
    public abstract class Ability {
        public readonly string Name;
        public readonly int Cooldown;
        public readonly string Texture;
        public readonly SecondaryWeapon Weapon;

        public bool Available { get { return Availability == 1f; } }
        public float Availability = 1f;
        public event Action<Ability> AvailabilityChanged;

        public Ability (SecondaryWeapon Weapon, string Name, int Cooldown, string Texture) {
            this.Name = Name;
            this.Cooldown = Cooldown;
            this.Texture = Texture;
            this.Weapon = Weapon;
        }

        public virtual void Prepare ( ) {
        }

        public abstract void OnCast ( );

        public virtual void Update (DeltaTime dt) {
            if (Availability < 1f) {
                Availability = Mathf.Clamp01(Availability + dt.TotalMilliseconds / Cooldown);
                AvailabilityChanged?.Invoke(this);
            }
        }
    }
}
