using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using mapKnight.Core;
using Microsoft.Xna.Framework.Graphics;

namespace mapKnight.ToolKit {
    public static class Extensions {
        private const int TILE_LIGHT_RESOLUTION = 10;
        private const float TILE_LIGHT_DISTANCE = 2 * TILE_LIGHT_RESOLUTION - .5f;

        public static void AddTile (this Map map, Tile tile) {
            List<Tile> tiles = map.Tiles.ToList( );
            tiles.Add(tile);
            map.Tiles = tiles.ToArray( );
        }

        public static void RemoveTile (this Map map, int index) {
            for (int y = 0; y < map.Height; y++) {
                for (int x = 0; x < map.Width; x++) {
                    for (int z = 0; z < 3; z++) {
                        if (map.Data[x, y, z] == index) {
                            map.Data[x, y, z] = 0;
                        } else if (map.Data[x, y, z] > index) {
                            map.Data[x, y, z]--;
                        }
                    }
                }
            }
            List<Tile> tiles = map.Tiles.ToList( );
            tiles.RemoveAt(index);
            map.Tiles = tiles.ToArray( );
        }

        public static unsafe void PrerenderShadowMap (this Map map, Stream stream) {
            WriteableBitmap bitmap = new WriteableBitmap(map.Width * 10, map.Height * 10, 30, 30, System.Windows.Media.PixelFormats.Bgra32, null);

            bitmap.Lock( );
            for (int y = 0; y < map.Height; y++) {
                for (int x = 0; x < map.Width; x++) {

                    for (int ty = 0; ty < TILE_LIGHT_RESOLUTION; ty++) {
                        for (int tx = 0; tx < TILE_LIGHT_RESOLUTION; tx++) {

                            float lightLvl = 1f;
                            if (map.GetTile(x, y).HasFlag(TileAttribute.Collision)) {

                                for (int cy = 1; cy <= 2; cy++) {
                                    for (int cx = 1; cx <= 2; cx++) {
                                        int px = x + cx, py = y + cy;
                                        if (px >= 0 && px < map.Width && py >= 0 && py < map.Height && !map.GetTile(px, py).HasFlag(TileAttribute.Collision)) {
                                            float dx = cx * TILE_LIGHT_RESOLUTION - tx - .5f;
                                            float dy = cy * TILE_LIGHT_RESOLUTION - ty - .5f;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }

                                        py = y - cy;
                                        if (px >= 0 && px < map.Width && py >= 0 && py < map.Height && !map.GetTile(px, py).HasFlag(TileAttribute.Collision)) {
                                            float dx = cx * TILE_LIGHT_RESOLUTION - tx - .5f;
                                            float dy = (cy - 1) * TILE_LIGHT_RESOLUTION + ty + .5f;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }

                                        px = x - cx;
                                        if (px >= 0 && px < map.Width && py >= 0 && py < map.Height && !map.GetTile(px, py).HasFlag(TileAttribute.Collision)) {
                                            float dx = (cx - 1) * TILE_LIGHT_RESOLUTION + tx + .5f;
                                            float dy = (cy - 1) * TILE_LIGHT_RESOLUTION + ty + .5f;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }

                                        py = y + cy;
                                        if (px >= 0 && px < map.Width && py >= 0 && py < map.Height && !map.GetTile(px, py).HasFlag(TileAttribute.Collision)) {
                                            float dx = (cx - 1) * TILE_LIGHT_RESOLUTION + tx + .5f;
                                            float dy = cy * TILE_LIGHT_RESOLUTION - ty - .5f;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }
                                    }
                                }

                                for (int d = 1; d <= 2; d++) {
                                    int temp = x + d;
                                    if (temp < map.Width && !map.GetTile(temp, y).HasFlag(TileAttribute.Collision)) {
                                        lightLvl = Math.Min(lightLvl, (d * TILE_LIGHT_RESOLUTION - tx - .5f) / TILE_LIGHT_DISTANCE);
                                    }

                                    temp = x - d;
                                    if (temp > 0 && !map.GetTile(temp, y).HasFlag(TileAttribute.Collision)) {
                                        lightLvl = Math.Min(lightLvl, ((d - 1) * TILE_LIGHT_RESOLUTION + tx + .5f) / TILE_LIGHT_DISTANCE);
                                    }

                                    temp = y + d;
                                    if (temp < map.Height && !map.GetTile(x, temp).HasFlag(TileAttribute.Collision)) {
                                        lightLvl = Math.Min(lightLvl, (d * TILE_LIGHT_RESOLUTION - ty - .5f) / TILE_LIGHT_DISTANCE);
                                    }

                                    temp = y - d;
                                    if (temp > 0 && !map.GetTile(x, temp).HasFlag(TileAttribute.Collision)) {
                                        lightLvl = Math.Min(lightLvl, ((d - 1) * TILE_LIGHT_RESOLUTION + ty + .5f) / TILE_LIGHT_DISTANCE);
                                    }
                                }

                                lightLvl = 1f - lightLvl;
                            }

                            int backBuffer = (int)bitmap.BackBuffer + (TILE_LIGHT_RESOLUTION * y + ty) * bitmap.BackBufferStride + (TILE_LIGHT_RESOLUTION * x + tx) * 4;
                            *((uint*)backBuffer) = (uint)((byte)(lightLvl * 255)  << 24);
                        }
                    }
                }
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            bitmap.Unlock( );

            PngBitmapEncoder encoder = new PngBitmapEncoder( );
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
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

        public static void SaveToStream (this BitmapImage image, Stream stream) {
            PngBitmapEncoder encoder = new PngBitmapEncoder( );
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
        }

        public static Texture2D ToTexture2D (this BitmapImage image, GraphicsDevice g) {
            using (MemoryStream ms = new MemoryStream( )) {
                image.SaveToStream(ms);

                return Texture2D.FromStream(g, ms);
            }
        }

        public static Map MergeRotations (this Map map, float[ , , ] rotations) {
            Map result = new Map(map.Size, map.Creator, map.Name) { Texture = map.Texture, SpawnPoint = map.SpawnPoint, Gravity = map.Gravity, Entities = map.Entities };
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
            Dictionary<int, Dictionary<int, int>> startPositions = new Dictionary<int, Dictionary<int, int>>( );
            for (int tile = 0; tile < map.Tiles.Length; tile++) {
                if (hasRotation.ContainsKey(tile)) {
                    startPositions.Add(tile, new Dictionary<int, int>( ));
                    if (hasRotation[tile][0])
                        startPositions[tile].Add(0, mapTiles.Count);
                    mapTiles.Add(map.Tiles[tile]);
                    for (int i = 1; i < 4; i++) {
                        if (hasRotation[tile][i]) {
                            startPositions[tile].Add(i, mapTiles.Count);
                            mapTiles.Add(new Tile( ) { Name = map.Tiles[tile].Name + i + "~", Attributes = map.Tiles[tile].Attributes, Texture = ShiftTextureCoordinates(i / 2f, map.Tiles[tile].Texture) });
                        }
                    }
                } else {
                    mapTiles.Add(map.Tiles[tile]);
                }
            }
            result.Tiles = mapTiles.ToArray( );
            // switch indicies
            for (int l = 0; l < 3; l++) {
                for (int x = 0; x < map.Width; x++) {
                    for (int y = 0; y < map.Height; y++) {
                        result.Data[x, y, l] = startPositions[map.Data[x, y, l]][(int)(rotations[x, y, l] * 2)];
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

        public static IEnumerable<T> FindDescendants<T> (this DependencyObject parent, Func<T, bool> predicate, bool deepSearch = false) where T : DependencyObject {
            var children = LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>( ).ToList( );

            foreach (var child in children) {
                var typedChild = child as T;
                if ((typedChild != null) && (predicate == null || predicate.Invoke(typedChild))) {
                    yield return typedChild;
                    if (deepSearch) foreach (var foundDescendant in FindDescendants(child, predicate, true)) yield return foundDescendant;
                } else {
                    foreach (var foundDescendant in FindDescendants(child, predicate, deepSearch)) yield return foundDescendant;
                }
            }

            yield break;
        }

        public static TreeViewItem FindContainer (this TreeView treeview, object item) {
            return (TreeViewItem)treeview.ItemContainerGenerator.FindContainer(item);
        }

        private static TreeViewItem FindContainer (this ItemContainerGenerator containerGenerator, object item) {
            TreeViewItem container = (TreeViewItem)containerGenerator.ContainerFromItem(item);
            if (container != null)
                return container;

            foreach (object childItem in containerGenerator.Items) {
                TreeViewItem parent = containerGenerator.ContainerFromItem(childItem) as TreeViewItem;
                if (parent == null)
                    continue;

                container = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (container != null)
                    return container;

                container = parent.ItemContainerGenerator.FindContainer(item);
                if (container != null)
                    return container;
            }
            return null;
        }

        public static void AddRange<T> (this ObservableCollection<T> collection, IEnumerable<T> items) {
            foreach (T item in items) collection.Add(item);
        }

        public static Point ToPoint (this Vector vector) {
            return new Point(vector.X, vector.Y);
        }
    }
}
