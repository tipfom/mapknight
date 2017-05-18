using System;

namespace mapKnight.Extended.Combat {
    public class Ability {
        public readonly string Name = "a";
        public readonly int Cooldown = 100;
        public readonly string Texture = "abil_1";
        public int CooldownLeft = 50;
        public event Action<Ability> CooldownChanged;

        public void WontBeHereLong ( ) {
            CooldownChanged?.Invoke(this);
        }
    }
}
