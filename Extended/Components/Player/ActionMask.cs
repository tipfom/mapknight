using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components.Player {
    [Flags]
    public enum ActionMask {
        None = 0,
    
        // BASE
        Jump = 1 << 0,
        Left = 1 << 1,
        Right = 1 << 2,

        // KNEIGHT
        DashLeft = 1 << 3,
        DashRight = 1  << 4
    }
}
