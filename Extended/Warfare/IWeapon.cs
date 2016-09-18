using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Warfare {
    public interface IWeapon {
        string Name { get; }
        string[ ] SpecialGestures { get; }

        void Attack ( );
        void Special (string move);
        void Prepare ( );
    }
}
