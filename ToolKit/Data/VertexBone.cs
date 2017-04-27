using System;
using System.IO;
using System.Windows.Media.Imaging;
using mapKnight.Core;
using mapKnight.ToolKit.Controls;
using mapKnight.ToolKit.Controls.Animation;
using Newtonsoft.Json;
using System.ComponentModel;

namespace mapKnight.ToolKit.Data {
    public class VertexBone : INotifyPropertyChanged {
        public BitmapImage BitmapImage { get; private set; }

        public bool Mirrored { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public string Image { get; set; }
        public float Scale { get; set; }

        public bool Export { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetBitmapImage(AnimationControl control) {
            BitmapImage = BoneImage.Data[control][Path.GetFileNameWithoutExtension(Image)].Image;
        }

        public VertexBone Clone ( ) {
            return new VertexBone( ) { Mirrored = Mirrored, Position = Position, Rotation = Rotation, Image = Image, Scale = Scale };
        }

        public void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
