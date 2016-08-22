using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Core.Graphics {
    public struct VertexBone {
        public bool Mirrored
#if __ANDROID__
            ;
#else
            { get; set; }
#endif
        public Vector2 Position
#if __ANDROID__
            ;
#else
            { get; set; }
#endif
        public float Rotation
#if __ANDROID__
        ;
#else
            { get; set; }
#endif
        public Vector2 Size
#if __ANDROID__
    ;
#else
            { get; set; }
#endif
    }
}
