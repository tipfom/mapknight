using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
#if __ANDROID__
#else
using System.Collections.ObjectModel;
#endif

namespace mapKnight.Core.Graphics {
    public class VertexAnimationFrame {
        public VertexBone[ ] State;

        public int Time;
    }
}
