using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Combat.Collections.Abilities;
using System.Collections.Generic;

namespace mapKnight.Extended.Combat.Collections.Secondaries {
    public class Shield : SecondaryWeapon {
        private Buff buffAbility;
        private Dash dashAbility;
        private Heal healAbility;

        public Shield (Entity Owner) : base(Owner) {
            buffAbility = new Buff(this);
            dashAbility = new Dash(this);
            healAbility = new Heal(this);
        }
        
        public override IEnumerable<Ability> Abilities ( ) {
            //yield return buffAbility;
            yield return dashAbility;
            //yield return healAbility;
        }

        public override void Prepare ( ) {
            foreach (Ability ability in Abilities( ))
                ability.Prepare( );
        }

        public override void Update (DeltaTime dt) {
            buffAbility.Update(dt);
            dashAbility.Update(dt);
            healAbility.Update(dt);
        }
    }
}
