using mapKnight.Core;

namespace mapKnight.Extended.Combat.Collections.Abilities {
    public class Heal : Ability {
        public Heal (SecondaryWeapon Weapon) : base(Weapon, "Heal", 5000, "abil_heal", new Vector2[0]) {
        }

        protected override void OnCast (float gestureSuccess) {
            EndCast( );
        }
    }
}
