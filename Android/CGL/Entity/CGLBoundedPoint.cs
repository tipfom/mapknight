using mapKnight.Basic;

namespace mapKnight.Android.CGL.Entity {
    public class CGLBoundedPoint {
        public fRectangle TextureRectangle;
        public fSize Size;
        public string Name;

        public CGLBoundedPoint (fRectangle textureRectangle, string name, fSize size) {
            TextureRectangle = textureRectangle;
            Size = size;
            Name = name;
        }
    }
}

