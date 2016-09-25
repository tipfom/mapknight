using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mapKnight.ToolKit.Controls.Components.Animation {
    public class VertexAnimation {
        public string Name { get; set; }
        public bool CanRepeat { get; set; }
        public ObservableCollection<VertexAnimationFrame> Frames { get; set; }
    }
}
