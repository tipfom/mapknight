using mapKnight.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.Particles
{
    public interface IVelocityProvider
    {
        Vector2 GetVelocity ( );
    }
}
