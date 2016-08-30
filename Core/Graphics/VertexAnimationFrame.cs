using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
#if WINDOWS
using System.Collections.ObjectModel;
#endif

namespace mapKnight.Core.Graphics {
    public class VertexAnimationFrame {
#if __ANDROID__
        [JsonIgnore]
        public VertexBone[ ] Bones;
        [JsonProperty]
        private Dictionary<string, VertexBone> State {
            set {
                List<string> bones = new List<string>(value.Keys);
                bones.Sort( );
                Bones = bones.Select(bone => value[bone]).ToArray( );
            }
        }
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
