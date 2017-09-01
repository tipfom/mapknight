using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Combat {
    [Flags]
    public enum DamageType {
        Physical = 1 << 1,
        Magical = 1 << 2,
        Pure = 1 << 3
    }
}
