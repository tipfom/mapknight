using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace mapKnight.Core.Graphics {
    public class SpriteAnimationFrame {
        public string[ ] Bones;
        public Dictionary<string, string> State {
            set {
                List<string> bones = new List<string>(value.Keys);
                bones.Sort( );
                Bones = bones.Select(bone => value[bone]).ToArray();
            }
        }
        public int Time;
    }
}
