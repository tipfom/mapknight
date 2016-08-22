using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS
using System.Collections.ObjectModel;
#endif

namespace mapKnight.Core.Graphics {
    public class VertexAnimationFrame {
#if __ANDROID__
                   public Dictionary<string, VertexBone> State ;
#else 
            public ObservableDictionary<string, VertexBone> State { get; set; }
#endif
        public int Time
#if __ANDROID__
            ;
#else
            { get; set; }
#endif
    }
}
