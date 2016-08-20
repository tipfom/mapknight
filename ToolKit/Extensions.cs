using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using mapKnight.Core;
using Microsoft.Xna.Framework.Graphics;

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

        public static Map MergeRotations (this Map map, float[ , , ] rotations) {
            Map result = new Map(map.Size, map.Creator, map.Name) { Texture = map.Texture };
            Dictionary<int, bool[ ]> hasRotation = new Dictionary<int, bool[ ]>( );
            // get all available rotations
            for (int l = 0; l < 3; l++) {
                for (int x = 0; x < map.Width; x++) {
                    for (int y = 0; y < map.Height; y++) {
                        int currentTile = map.Data[x, y, l];
                        if (!hasRotation.ContainsKey(currentTile))
                            hasRotation.Add(currentTile, new bool[4]);
                        hasRotation[currentTile][(int)(rotations[x, y, l] * 2)] = true;
                    }
                }
            }
            // create new tiles
            List<Tile> mapTiles = new List<Tile>( );
            Dictionary<int, int> startPositions = new Dictionary<int, int>( );
            for (int tile = 0; tile < map.Tiles.Length; tile++) {
                startPositions.Add(tile, mapTiles.Count);
                mapTiles.Add(map.Tiles[tile]);
                if (hasRotation.ContainsKey(tile)) {
                    for (int i = 1; i < 4; i++) {
                        if (hasRotation[tile][i]) {
                            mapTiles.Add(new Tile( ) { Name = map.Tiles[tile].Name + i + "~", Attributes = map.Tiles[tile].Attributes, Texture = ShiftTextureCoordinates(i / 2f, map.Tiles[tile].Texture) });
                        }
                    }
                }
            }
            result.Tiles = mapTiles.ToArray( );
            // switch indicies
            for (int l = 0; l < 3; l++) {
                for (int x = 0; x < map.Width; x++) {
                    for (int y = 0; y < map.Height; y++) {
                        result.Data[x, y, l] = startPositions[map.Data[x, y, l]] + (int)(rotations[x, y, l] * 2);
                    }
                }
            }
            return result;
        }

        public static float[ , , ] ExtractRotations (this Map map) {
            float[ , , ] rotations = new float[map.Width, map.Height, 3];

            Dictionary<string, int> startPositions = new Dictionary<string, int>( );
            foreach (Tile tile in map.Tiles) {
                if (!tile.Name.EndsWith("~")) {
                    startPositions.Add(tile.Name, startPositions.Count);
                }
            }

            for (int l = 0; l < 3; l++) {
                for (int x = 0; x < map.Width; x++) {
                    for (int y = 0; y < map.Height; y++) {
                        Tile tile = map.Tiles[map.Data[x, y, l]];
                        if (tile.Name.EndsWith("~")) {
                            map.Data[x, y, l] = startPositions[tile.Name.Substring(0, tile.Name.Length - 2)];
                            rotations[x, y, l] = float.Parse(tile.Name[tile.Name.Length - 2].ToString( )) / 2f;
                        } else
                            map.Data[x, y, l] = startPositions[tile.Name];
                    }
                }
            }

            List<Tile> filteredTiles = new List<Tile>( );
            foreach (Tile tile in map.Tiles) {
                if (!tile.Name.EndsWith("~"))
                    filteredTiles.Add(tile);
            }
            map.Tiles = filteredTiles.ToArray( );

            return rotations;
        }

        private static float[ ] ShiftTextureCoordinates (float rotation, float[ ] texturecoords) {
            if (rotation == 0.5f) {
                return new float[ ] { texturecoords[6], texturecoords[7], texturecoords[0], texturecoords[1], texturecoords[2], texturecoords[3], texturecoords[4], texturecoords[5] };
            } else if (rotation == 1f) {
                return new float[ ] { texturecoords[4], texturecoords[5], texturecoords[6], texturecoords[7], texturecoords[0], texturecoords[1], texturecoords[2], texturecoords[3] };
            } else if (rotation == 1.5f) {
                return new float[ ] { texturecoords[2], texturecoords[3], texturecoords[4], texturecoords[5], texturecoords[6], texturecoords[7], texturecoords[0], texturecoords[1] };
            } else {
                return null;
            }
        }
    }
}
