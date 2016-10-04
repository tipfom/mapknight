using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mapKnight.ToolKit.Data {
    public class VertexAnimationFrame {
        public ObservableCollection<VertexBone> Bones { get; set; }
        public int Time { get; set; }

        public VertexAnimationFrame Clone ( ) {
            return new VertexAnimationFrame( ) { Time = Time, Bones = new ObservableCollection<VertexBone>(Bones.Select(item => item.Clone( ))) };
        }
    }
}
