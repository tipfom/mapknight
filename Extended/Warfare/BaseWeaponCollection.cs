using mapKnight.Core;
using mapKnight.Core.World;

namespace mapKnight.Extended.Warfare {
    public static class BaseWeaponCollection {
        public static BaseWeapon DiamondSword (Entity owner) {
            return new BaseWeapon("Diamond Sword", 0, 3f, new Transform(new Vector2(owner.Transform.Width, 0f), new Vector2(owner.Transform.Width, owner.Transform.Height)), owner);
        }
    }
}
