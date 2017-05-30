using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mapKnight.ToolKit.Data {
    public class VertexAnimationFrame : INotifyPropertyChanged {
        public ObservableCollection<VertexBone> Bones { get; set; }
        public int Time { get; set; }
        public bool Featured { get; set; }
       
        // boiler-plate
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged (string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public VertexAnimationFrame Clone ( ) {
            return new VertexAnimationFrame( ) { Time = Time, Bones = new ObservableCollection<VertexBone>(Bones.Select(item => item.Clone( ))) };
        }
    }
}
