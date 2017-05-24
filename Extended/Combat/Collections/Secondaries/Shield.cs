using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Combat.Collections.Abilities;

namespace mapKnight.Extended.Combat.Collections.Secondaries {
    public class Shield : SecondaryWeapon {

        private enum Action {
            None,
            Charge,
        }

        private Action currentAction;
        public Ability testChargeAbility;

        public Shield (Entity Owner) : base(Owner, "gestures") {
            testChargeAbility = new ChargeAbility(this);
        }

        public override void Prepare ( ) {
            testChargeAbility.Prepare( );
        }
        
        public override void Update (DeltaTime dt) {
            testChargeAbility.Update(dt);
        }

        public override void OnGesture (string gesture) {
        }
    }
}
