using mapKnight.Core;
using mapKnight.ToolKit.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace mapKnight.ToolKit {
    public class Project {
        public string Home { get; private set; } = null;
        public bool IsLocated { get { return Home != null; } }

        public event Action<Map> MapAdded;
        private List<Map> maps = new List<Map>( );
        private Dictionary<Map, Dictionary<string, Texture2D>> xnaTextures = new Dictionary<Map, Dictionary<string, Texture2D>>( );
        private Dictionary<Map, Dictionary<string, BitmapImage>> wpfTextures = new Dictionary<Map, Dictionary<string, BitmapImage>>( );

        private GraphicsDeviceService graphicsService;
        private GraphicsDevice GraphicsDevice { get { return graphicsService.GraphicsDevice; } }

        public Project (Control parent) {
            graphicsService = GraphicsDeviceService.AddRef((PresentationSource.FromVisual(parent) as HwndSource).Handle);
        }

        public Project (Control parent, string path) : this(parent) {
            Home = path;

            string mapPath = Path.Combine(Home, "maps");

            foreach (string map in Directory.GetFiles(mapPath).Where(file => Path.GetExtension(file) == ".map")) {
                using (Stream mapStream = File.OpenRead(map)) {
                    Map loadedMap = Map.FromStream(mapStream);
                    xnaTextures.Add(loadedMap, ExtractTextures(loadedMap, Path.Combine(mapPath, loadedMap.Texture)));
                    wpfTextures.Add(loadedMap, new Dictionary<string, BitmapImage>( ));
                    foreach (var entry in xnaTextures[loadedMap])
                        wpfTextures[loadedMap].Add(entry.Key, entry.Value.ToBitmapImage( ));
                    AddMap(loadedMap);
                }
            }
        }

        private Dictionary<string, Texture2D> ExtractTextures (Map map, string texturePath) {
            Texture2D originalTexture = Texture2D.FromStream(GraphicsDevice, File.OpenRead(texturePath));
            Dictionary<string, Texture2D> result = new Dictionary<string, Texture2D>( );
            foreach (Tile tile in map.Tiles) {
                RenderTarget2D renderTarget = new RenderTarget2D(GraphicsDevice, Map.TILE_PXL_SIZE, Map.TILE_PXL_SIZE);
                GraphicsDevice.SetRenderTarget(renderTarget);
                GraphicsDevice.Clear(Color.Transparent);
                using (SpriteBatch batch = new SpriteBatch(GraphicsDevice)) {
                    batch.Begin(samplerState: SamplerState.PointWrap);
                    Rectangle sourceRectangle = new Rectangle((int)Math.Round(originalTexture.Width * tile.Texture[0]), (int)Math.Round(originalTexture.Height * tile.Texture[3]), Map.TILE_PXL_SIZE, Map.TILE_PXL_SIZE);
                    batch.Draw(originalTexture, new Rectangle(0, 0, Map.TILE_PXL_SIZE, Map.TILE_PXL_SIZE), sourceRectangle, Color.White);
                    batch.End( );
                }
                GraphicsDevice.SetRenderTarget(null);
                result.Add(tile.Name, renderTarget);
            }
            return result;
        }

        public void Save ( ) {
            Save(Home);
        }

        public void Save (string directory) {
            Home = directory;
            string mapDirectory = Path.Combine(directory, "maps");
            if (!Directory.Exists(mapDirectory))
                Directory.CreateDirectory(mapDirectory);

            foreach (Map map in maps) {
                // build texture
                string texturePath = Path.ChangeExtension(Path.Combine(mapDirectory, map.Name), "png");
                string mapPath = Path.ChangeExtension(Path.Combine(mapDirectory, map.Name), "map");
                BuildTexture(map, xnaTextures[map], texturePath);
                map.Texture = map.Name + ".png";
                map.Serialize(File.OpenWrite(mapPath));
            }

            MessageBox.Show("Completed!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BuildTexture (Map map, Dictionary<string, Texture2D> textures, string path) {
            int textureTileSize = Map.TILE_PXL_SIZE + 2;
            int textureSizeTL = (int)Math.Sqrt(map.Tiles.Length) + 1;
            int textureSizePXL = textureSizeTL * textureTileSize;
            RenderTarget2D renderTarget = new RenderTarget2D(graphicsService.GraphicsDevice, textureSizePXL, textureSizePXL);
            graphicsService.GraphicsDevice.SetRenderTarget(renderTarget);

            graphicsService.GraphicsDevice.Clear(Color.Transparent);
            using (SpriteBatch batch = new SpriteBatch(graphicsService.GraphicsDevice)) {
                batch.Begin(samplerState: SamplerState.PointClamp);
                for (int y = 0; y < textureSizeTL; y++) {
                    for (int x = 0; x < Math.Min(textureSizeTL, map.Tiles.Length - y * textureSizeTL); x++) {
                        int currentIndex = y * textureSizeTL + x;
                        Texture2D tileTexture = textures[map.Tiles[currentIndex].Name];
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // draw to texture
                        // tile
                        Rectangle drawRectangle = new Rectangle(x * textureTileSize + 1, y * textureTileSize + 1, Map.TILE_PXL_SIZE, Map.TILE_PXL_SIZE);
                        batch.Draw(tileTexture, drawRectangle, Color.White);
                        // expanding border
                        Rectangle blDrawRectangle = new Rectangle(x * textureTileSize, y * textureTileSize + 1, 1, Map.TILE_PXL_SIZE);
                        Rectangle blSourceRectangle = new Rectangle(0, 0, 1, Map.TILE_PXL_SIZE);
                        Rectangle brDrawRectangle = new Rectangle((x + 1) * textureTileSize - 1, y * textureTileSize + 1, 1, Map.TILE_PXL_SIZE);
                        Rectangle brSourceRectangle = new Rectangle(Map.TILE_PXL_SIZE - 1, 0, 1, Map.TILE_PXL_SIZE);
                        Rectangle btDrawRectangle = new Rectangle(x * textureTileSize + 1, y * textureTileSize, Map.TILE_PXL_SIZE, 1);
                        Rectangle btSourceRectangle = new Rectangle(0, 0, Map.TILE_PXL_SIZE, 1);
                        Rectangle bbDrawRectangle = new Rectangle(x * textureTileSize + 1, (y + 1) * textureTileSize - 1, Map.TILE_PXL_SIZE, 1);
                        Rectangle bbSourceRectangle = new Rectangle(0, Map.TILE_PXL_SIZE - 1, Map.TILE_PXL_SIZE, 1);
                        batch.Draw(tileTexture, blDrawRectangle, blSourceRectangle, Color.White);
                        batch.Draw(tileTexture, brDrawRectangle, brSourceRectangle, Color.White);
                        batch.Draw(tileTexture, btDrawRectangle, btSourceRectangle, Color.White);
                        batch.Draw(tileTexture, bbDrawRectangle, bbSourceRectangle, Color.White);
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // save texture coords
                        float vertexSize = (float)Map.TILE_PXL_SIZE / textureSizePXL;
                        float startX = (float)drawRectangle.X / textureSizePXL;
                        float startY = (float)drawRectangle.Y / textureSizePXL;
                        map.Tiles[currentIndex].Texture = new float[ ] {
                            startX ,startY + vertexSize,
                            startX ,startY,
                            startX + vertexSize, startY,
                            startX + vertexSize, startY + vertexSize
                        };
                    }
                }
                batch.End( );
            }

            graphicsService.GraphicsDevice.SetRenderTarget(null);
            using (Stream imageStream = File.OpenWrite(path))
                renderTarget.SaveAsPng(imageStream, renderTarget.Width, renderTarget.Height);
        }

        public void AddMap (Map map) {
            maps.Add(map);
            if (!xnaTextures.ContainsKey(map)) {
                xnaTextures.Add(map, new Dictionary<string, Texture2D>( ));
                wpfTextures.Add(map, new Dictionary<string, BitmapImage>( ));
            }
            MapAdded?.Invoke(map);
        }

        public IEnumerable<Map> GetMaps ( ) {
            return maps;
        }

        public void AddTexture (Map map, string name, Texture2D texture) {
            if (xnaTextures[map].ContainsKey(name))
                return;
            xnaTextures[map].Add(name, texture);
            wpfTextures[map].Add(name, texture.ToBitmapImage( ));
        }

        public void AddTexture (Map map, string name, BitmapImage image) {
            if (xnaTextures[map].ContainsKey(name))
                return;
            xnaTextures[map].Add(name, image.ToTexture2D(GraphicsDevice));
            wpfTextures[map].Add(name, image);
        }

        public Dictionary<string, Texture2D> GetMapXNATextures (Map map) {
            return xnaTextures[map];
        }

        public Dictionary<string, BitmapImage> GetMapWPFTextures (Map map) {
            return wpfTextures[map];
        }

        public static bool Validate (string path) {
            return false;
        }
    }
}
