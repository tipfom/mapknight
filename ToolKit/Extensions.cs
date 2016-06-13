using mapKnight.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit {
    public static class Extensions {
        public static void AddTile (this Map map, Tile tile) {
            List<Tile> tiles = map.Tiles.ToList( );
            tiles.Add(tile);
            map.Tiles = tiles.ToArray( );
        }

        public static BitmapImage ToBitmapImage (this Texture2D texture) {
            using (MemoryStream ms = new MemoryStream( )) {
                texture.SaveAsPng(ms, texture.Width, texture.Height);

                BitmapImage result = new BitmapImage( );
                result.BeginInit( );
                result.StreamSource = ms;
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.EndInit( );
                return result;
            }
        }

        public static Texture2D ToTexture2D (this BitmapImage image, GraphicsDevice g) {
            using (MemoryStream ms = new MemoryStream( )) {
                PngBitmapEncoder encoder = new PngBitmapEncoder( );
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(ms);

                return Texture2D.FromStream(g, ms);
            }
        }

        public static float[ , , ] GetRotations (this Map map) {
            return App.Project.GetRotations(map);
        }

        public static float GetRotation (this Map map, int x, int y, int layer) {
            return App.Project.GetRotations(map)[x, y, layer];
        }

        public static void SetRotation (this Map map, int x, int y, int layer, float value) {
            App.Project.SetRotation(map, x, y, layer, value);
        }
    }
}
