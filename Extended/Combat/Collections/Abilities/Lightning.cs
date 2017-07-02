using mapKnight.Core;

namespace mapKnight.Extended.Combat.Collections.Abilities {
    public class Lightning : Ability {
        public Lightning (SecondaryWeapon Weapon) : base(Weapon, "Lightning", 700, "abil_lightning", new Vector2[0]) {
        }

        public override void OnCast ( ) {
        }
    }
}
