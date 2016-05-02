using mapKnight.Basic;

namespace mapKnight.Android.Physics {
    public class Entity : Android.Entity.Entity {
        public AABB AABB;
        // centre
        public fVector2D Position { get { return AABB.Centre; } }
        public fVector2D Velocity;
        public fVector2D Acceleration;
        public readonly int Weight;
        public Flag CollisionMask;

        public Entity (int weight, fSize bounds, int health, fPoint position, string name) : base (health, name) {
            AABB = new AABB (position.X, position.Y, position.X + bounds.Width, position.Y + bounds.Height);
            Weight = weight;

            Velocity = new fVector2D (0, 0);
            Acceleration = new fVector2D (0, 0);
            CollisionMask = Flag.None;
        }
    }
}

