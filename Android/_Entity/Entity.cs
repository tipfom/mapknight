using System;

using mapKnight.Values;

namespace mapKnight.Entity
{
	public class Entity
	{
		public event Action<Entity> OnDeath;

		public ChangingProperty Health{ get; private set; }

		public fPoint Position{ get; set; }
		// oben links = (0, 0)

		public bool Alive{ get; private set; }

		public readonly string Name;

		public Entity (int health, fPoint position, string name)
		{
			Alive = true;
			Health = new ChangingProperty (health);
			Position = position;
			Name = name;
		}

		public virtual void TakeDamage (int damage)
		{
			Health.Current -= damage;
			if (Health.Current <= 0) {
				Die ();
			}
		}

		public virtual void Die ()
		{
			Alive = false;
			Position = new fPoint (-1, -1);
			OnDeath (this);
		}

		public virtual void Move (int newx, int newy)
		{
			if (Alive)
				Position = new fPoint (newx, newy);
		}
	}
}

