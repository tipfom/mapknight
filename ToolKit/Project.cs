using mapKnight.Core;
using mapKnight.ToolKit.Xna;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit {
    public class Project {
        public string Home { get; private set; } = null;
        public bool IsLocated { get { return Home != null; } }
        public bool HasChanged { get; set; } = false;

        public event Action<Map> MapAdded;
        private List<Map> maps = new List<Map>( );
        private Dictionary<Map, float[ , , ]> mapRotations = new Dictionary<Map, float[ , , ]>( );
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
                LoadMap(map);
            }
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

                Texture2D packedTexture = TileSerializer.BuildTexture(map.Tiles, xnaTextures[map], GraphicsDevice);
                using (Stream stream = File.OpenWrite(texturePath))
                    packedTexture.SaveAsPng(stream, packedTexture.Width, packedTexture.Height);
                map.Texture = map.Name + ".png";
                map.Serialize(File.OpenWrite(mapPath));
            }

            HasChanged = false;
        }

        public void AddMap (Map map) {
            HasChanged = true;
            if (!maps.Contains(map)) {
                mapRotations.Add(map, new float[map.Width, map.Height, 3]);
                maps.Add(map);
                if (!xnaTextures.ContainsKey(map)) {
                    xnaTextures.Add(map, new Dictionary<string, Texture2D>( ));
                    wpfTextures.Add(map, new Dictionary<string, BitmapImage>( ));
                }
            }
            MapAdded?.Invoke(map);
        }

        public void LoadMap (string path) {
            using (Stream mapStream = File.OpenRead(path)) {
                Map loadedMap = Map.FromStream(mapStream);
                using (Stream imageStream = File.OpenRead(Path.Combine(Path.GetDirectoryName(path), loadedMap.Texture)))
                    xnaTextures.Add(loadedMap, TileSerializer.ExtractTextures(Texture2D.FromStream(GraphicsDevice, imageStream), loadedMap.Tiles, GraphicsDevice));
                wpfTextures.Add(loadedMap, new Dictionary<string, BitmapImage>( ));
                foreach (var entry in xnaTextures[loadedMap])
                    wpfTextures[loadedMap].Add(entry.Key, entry.Value.ToBitmapImage( ));
                AddMap(loadedMap);
            }
        }

        public List<Map> GetMaps ( ) {
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

        public void ChangeTextureName (Map map, string oldname, string newname) {
            Texture2D texture = xnaTextures[map][oldname];
            xnaTextures[map].Remove(oldname);
            xnaTextures[map].Add(newname, texture);

            BitmapImage image = wpfTextures[map][oldname];
            wpfTextures[map].Remove(oldname);
            wpfTextures[map].Add(newname, image);
        }

        public Dictionary<string, Texture2D> GetMapXNATextures (Map map) {
            return xnaTextures[map];
        }

        public Dictionary<string, BitmapImage> GetMapWPFTextures (Map map) {
            return wpfTextures[map];
        }

        public float[ , , ] GetRotations (Map map) {
            return mapRotations[map];
        }

        public void SetRotation (Map map, int x, int y, int layer, float value) {
            float[ , , ] data = mapRotations[map];
            data[x, y, layer] = value;
            mapRotations[map] = data;
        }

        public static bool Validate (string path) {
            return false;
        }
    }
}
