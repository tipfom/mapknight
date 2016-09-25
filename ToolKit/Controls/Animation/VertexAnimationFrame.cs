using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mapKnight.ToolKit.Controls.Components.Animation {
    public class VertexAnimationFrame {
        public ObservableDictionary<string, VertexBone> State { get; set; }
        public int Time { get; set; }

    }
}
