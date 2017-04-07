using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit.Data {
    public struct EntityData {
        public string Name { get; }
        public BitmapImage Bitmap { get; }
        public Texture2D Texture { get; }

        public EntityData(string Name, string Texture, GraphicsDevice g) {
            this.Name = Name;
            this.Bitmap = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly( ).GetName( ).Name + ";component/Resources/Images/Entities/" + Texture + ".png", UriKind.Absolute));
            this.Texture = Bitmap.ToTexture2D(g);
        }
    }
}
