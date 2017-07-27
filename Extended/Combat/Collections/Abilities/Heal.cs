using mapKnight.Core;
using mapKnight.Core.World.Components;
using System;

namespace mapKnight.Extended.Combat.Collections.Abilities {
    public class Heal : Ability {
        private const float HEAL_PER_SECOND = 3;
        private const int HEAL_DURATION = 2000;

        private static readonly Vector2[ ] PREVIEW = new Vector2[ ] {
            new Vector2(0.4f, 0.0f),
            new Vector2(0.6f, 0.0f),

            new Vector2(0.6f, 0.4f),

            new Vector2(1.0f, 0.4f),
            new Vector2(1.0f, 0.6f),

            new Vector2(0.6f, 0.6f),

            new Vector2(0.6f, 1.0f),
            new Vector2(0.4f, 1.0f),

            new Vector2(0.4f, 0.6f),

            new Vector2(0.0f, 0.6f),
            new Vector2(0.0f, 0.4f),

            new Vector2(0.4f, 0.4f),

            new Vector2(0.4f, 0.0f),
        };

        private int healEndTime = 0;
        private bool wasHealing = false;

        public Heal (SecondaryWeapon Weapon) : base(Weapon, "Heal", 5000, "abil_heal", PREVIEW) {
        }

        protected override void OnCast (float gestureSuccess) {
            wasHealing = true;
            healEndTime = Environment.TickCount + HEAL_DURATION;
        }

        public override void Update (DeltaTime dt) {
            if (healEndTime > Environment.TickCount) {
                Weapon.Owner.SetComponentInfo(ComponentData.Heal, HEAL_PER_SECOND * Manager.FrameTime.TotalSeconds);
                RequestUpdate( );
            } else if (wasHealing) {
                wasHealing = false;
                EndCast( );
            }
            base.Update(dt);
        }
    }
}
