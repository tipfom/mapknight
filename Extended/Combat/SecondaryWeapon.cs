using mapKnight.Core;
using mapKnight.Core.World;
using System.Collections.Generic;

namespace mapKnight.Extended.Combat {
    public abstract class SecondaryWeapon {
        public bool Lock;
        public Entity Owner;

        public SecondaryWeapon(Entity Owner) {
            this.Owner = Owner;
        }

        public abstract IEnumerable<Ability> Abilities ( );

        public virtual void Prepare ( ) {
        }

        public virtual void Update (DeltaTime dt) {
        }
    }
}
