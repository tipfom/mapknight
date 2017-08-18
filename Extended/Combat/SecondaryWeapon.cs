using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics.Animation;
using System.Collections.Generic;

namespace mapKnight.Extended.Combat {
    public abstract class SecondaryWeapon {
        public readonly string Texture; 
        public readonly VertexAnimationData AnimationData;

        public bool Lock;
        public Entity Owner;

        public SecondaryWeapon(Entity Owner, string Texture, VertexAnimationData AnimationData) {
            this.Owner = Owner;
            this.Texture = Texture;
            this.AnimationData = AnimationData;
        }

        public abstract IEnumerable<Ability> Abilities ( );

        public virtual void Prepare ( ) {
        }

        public virtual void Update (DeltaTime dt) {
        }
    }
}
