using mapKnight.Core;
using mapKnight.Core.World;

namespace mapKnight.Extended.Combat {
    public abstract class SecondaryWeapon {
        public readonly string Gestures;

        public bool Lock;
        public Entity Owner;

        public SecondaryWeapon(Entity Owner, string Gestures) {
            this.Gestures = Gestures;
            this.Owner = Owner;
        }

        public abstract void OnGesture (string gesture);

        public virtual void Prepare ( ) {
        }

        public virtual void Update (DeltaTime dt) {
        }
    }
}
