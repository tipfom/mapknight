using mapKnight.Core;

namespace mapKnight.Extended.Combat.Collections.Abilities {
    public class Buff : Ability {
        public Buff (SecondaryWeapon Weapon) : base(Weapon, "Buff", 5000, "abil_buff", new Vector2[] { Vector2.One, Vector2.Zero }) {
        }

        protected override void OnCast (float gestureSuccess) {
            EndCast( );
        }
    }
}
