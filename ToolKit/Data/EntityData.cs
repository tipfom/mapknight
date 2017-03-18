using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit.Data {
    public struct EntityData {
        public string Name { get; }
        public BitmapImage Texture { get; }

        public EntityData(string Name, string Texture) {
            this.Name = Name;
            this.Texture = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly( ).GetName( ).Name + ";component/Resources/Images/Entities/" + Texture + ".png", UriKind.Absolute)); 
        }
    }
}
