using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.Movement;

namespace mapKnight.Extended.Warfare {
    public abstract class Sword : IWeapon {
        public abstract string Name { get; }
        public abstract string[ ] SpecialGestures { get; }

        protected Entity Holder;

        private Vector2 hitboxOffset;
        private Vector2 hitboxSize;
        private MotionComponent motionComponent;
        private float damage;

        public Sword (Entity holder, Vector2 hitboxoffset, Vector2 hitboxsize, float swordDamage) {
            Holder = holder;
            hitboxOffset = hitboxoffset;
            hitboxSize = hitboxsize;
            damage = swordDamage;
        }

        public virtual void Prepare ( ) {
            motionComponent = Holder.GetComponent<MotionComponent>( );
        }

        public void Attack ( ) {
            Transform hitbox = new Transform(new Vector2(Holder.Transform.Center.X + hitboxOffset.X * motionComponent.ScaleX, Holder.Transform.Center.Y + hitboxOffset.Y), hitboxSize);
            for (int i = 0; i < Holder.World.Entities.Count; i++) {
                Entity checkingEntity = Holder.World.Entities[i];
                if (checkingEntity != Holder && checkingEntity.Transform.Intersects(hitbox)) {
                    if (checkingEntity.Domain == EntityDomain.Enemy) checkingEntity.SetComponentInfo(ComponentData.Damage, Holder, damage);
                }
            }
        }

        public abstract void Special (string move);
    }
}
