using mapKnight.Core;

namespace mapKnight.Extended.Combat.Collections.Abilities {
    public class Buff : Ability {
        public Buff (SecondaryWeapon Weapon) : base(Weapon, "Buff", 5000, "abil_buff", new Vector2[0]) {
        }

        protected override void OnCast (float gestureSuccess) {
            EndCast( );
        }
    }
}
