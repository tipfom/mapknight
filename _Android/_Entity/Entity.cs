using System;

using mapKnight.Basic;

namespace mapKnight.Android {
    public class Entity {
        public event Action<Entity> OnDeath;

        public ChangingProperty Health { get; private set; }

        public bool Alive { get; private set; }

        public readonly string Name;

        public Entity (int health, string name) {
            Alive = true;
            Health = new ChangingProperty (health);
            Name = name;
        }

        public virtual void TakeDamage (int damage) {
            Health.Current -= damage;
            if (Health.Current <= 0) {
                Die ();
            }
        }

        public virtual void Die () {
            Alive = false;
            OnDeath (this);
        }
    }
}

