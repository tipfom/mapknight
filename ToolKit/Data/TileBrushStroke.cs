using mapKnight.Core;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit.Data {
    public class TileBrushStroke {
        public Tile Tile { get; set; }
        public float Rotation { get; set; }
        public int Possibility { get; set; }

        [JsonIgnore]
        public double RotationInDegree { get { return Rotation * 180d; } }
        [JsonIgnore]
        public BitmapImage Preview { get; set; }

        public TileBrushStroke(Tile Tile, float Rotation, int Possibility) {
            this.Tile = Tile;
            this.Rotation = Rotation;
            this.Possibility = Possibility;
        }

        public void GeneratePreviewImage (EditorMap map) {
            Preview = map.WpfTextures[Tile.Name];
        }
    }
}
