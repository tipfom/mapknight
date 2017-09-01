using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components.Player;

namespace mapKnight.Extended.Combat.Collections.Abilities {
    public class Terrorfield : Ability {
        private const int DURATION = 10000;

        private static readonly Vector2[ ] PREVIEW = new Vector2[ ] {
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.5f, 0.0f)
        };

        private int fieldEndTime = 0;
        private PlayerComponent playerComponent;

        public Terrorfield (SecondaryWeapon Weapon) : base(Weapon, "Terrorfield", 10000, "abil_dash", PREVIEW) {
        }

        public override void Prepare ( ) {
            playerComponent = Weapon.Owner.GetComponent<PlayerComponent>( );
        }

        protected override void OnCast (float gestureSuccess) {
            fieldEndTime = DURATION;
            playerComponent.Immunity |= DamageType.Physical;
        }

        public override void Update (DeltaTime dt) {
            if (fieldEndTime > 0) {
                fieldEndTime -= (int)dt.TotalMilliseconds;
                if (fieldEndTime <= 0) {
                    playerComponent.Immunity &= ~DamageType.Physical;
                    EndCast( );
                }
                RequestUpdate( );
            }
            base.Update(dt);
        }
    }
}
