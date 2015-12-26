using System;

using mapKnight.Values;

namespace mapKnight.Entity
{
	public class Entity
	{
		public event Action<Entity> OnDeath;

		public ChangingProperty Health{ get; private set; }

		public Point Position{ get; private set; }
		// oben links = (0, 0)

		public bool Alive{ get; private set; }

		public readonly string Name;

		public Entity (int health, Point position, string name)
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
			Position = new Point (-1, -1);
			OnDeath (this);
		}

		public virtual void Move (int newx, int newy)
		{
			if (Alive)
				Position = new Point (newx, newy);
		}
	}
}

