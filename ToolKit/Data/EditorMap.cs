using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.ToolKit.Serializer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Size = mapKnight.Core.Size;

namespace mapKnight.ToolKit.Data {
    public class EditorMap : Map, IEntityWorld {
        private const int TILE_LIGHT_RESOLUTION = 10;
        private const float TILE_LIGHT_DISTANCE = 2 * TILE_LIGHT_RESOLUTION - .5f;

        public string Description {
            get {
                return $"{Name}, Width: {Width}, Height: {Height}";
            }
        }

        public float[ , , ] Rotations;
        public byte[ ] ImageData;
        public Dictionary<string, Texture2D> XnaTextures = new Dictionary<string, Texture2D>( );
        public Dictionary<string, BitmapImage> WpfTextures = new Dictionary<string, BitmapImage>( );
        public List<TileBrush> Brushes = new List<TileBrush>( );

        public EditorMap (Size size, string creator, string name) :
                base(size, creator, name) {
            Rotations = new float[Width, Height, 3];
        }

        public EditorMap (Stream input) :
            base(input, new WindowsEntitySerializer( )) {
            ExtractRotations( );
        }

        public IEntityRenderer Renderer => null;

        public float VertexSize => 0;

        public void Init (GraphicsDevice g) {
            if (ImageData != null) {
                using (Stream imageStream = new MemoryStream(ImageData))
                    XnaTextures = TileSerializer.ExtractTextures(Texture2D.FromStream(g, imageStream), Tiles, g);
                ImageData = null;
                foreach (var entry in XnaTextures)
                    WpfTextures.Add(entry.Key, entry.Value.ToBitmapImage( ));
            } else if (Tiles == null) {
                MemoryStream memoryStream = new MemoryStream( );
                memoryStream.Position = 0;
                new Bitmap(1, 1).Save(memoryStream, ImageFormat.Png);
                BitmapImage emptyImage = new BitmapImage( );
                emptyImage.BeginInit( );
                emptyImage.StreamSource = memoryStream;
                emptyImage.EndInit( );

                LoadTexture("None", emptyImage, g);
                Tiles = new Tile[ ] { new Tile( ) { Name = "None", Attributes = new Dictionary<TileAttribute, string>( ) } };
            } else if (WpfTextures.Count > 0 && XnaTextures.Count == 0) {
                foreach (var entry in WpfTextures) {
                    XnaTextures.Add(entry.Key, entry.Value.ToTexture2D(g));
                }
            }
        }

        public bool HasCollider (int x, int y) {
            return GetTile(x, y).HasFlag(TileAttribute.Collision);
        }

        public void SaveTo (Project project) {
            // build texture
            Texture2D packedTexture = TileSerializer.BuildTexture(Tiles, XnaTextures, project.GraphicsDevice);
            using (Stream stream = project.GetOrCreateStream(false, "maps", Name, Name + ".png"))
                packedTexture.SaveAsPng(stream, packedTexture.Width, packedTexture.Height);

            Texture = Path.GetFileNameWithoutExtension(Name + ".png");
            using (Stream stream = project.GetOrCreateStream(false, "maps", Name, Name + ".map"))
                CreateCompileVersion( ).Serialize(stream, new WindowsEntitySerializer( ));
        }

        public void LoadTexture (string name, BitmapImage image, GraphicsDevice g) {
            WpfTextures.Add(name, image);
            XnaTextures.Add(name, image.ToTexture2D(g));
        }

        public void AddTile (Tile tile) {
            List<Tile> tiles = Tiles.ToList( );
            tiles.Add(tile);
            Tiles = tiles.ToArray( );
        }

        public void RemoveTile (int index) {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    for (int z = 0; z < 3; z++) {
                        if (Data[x, y, z] == index) {
                            Data[x, y, z] = 0;
                        } else if (Data[x, y, z] > index) {
                            Data[x, y, z]--;
                        }
                    }
                }
            }
            List<Tile> tiles = Tiles.ToList( );
            tiles.RemoveAt(index);
            Tiles = tiles.ToArray( );
        }

        public Map CreateCompileVersion ( ) {
            Map result = new Map(Size, Creator, Name) { Texture = Texture, SpawnPoint = SpawnPoint, Gravity = Gravity, Entities = Entities };
            Dictionary<int, bool[ ]> hasRotation = new Dictionary<int, bool[ ]>( );
            // get all available rotations
            for (int l = 0; l < 3; l++) {
                for (int x = 0; x < Width; x++) {
                    for (int y = 0; y < Height; y++) {
                        int currentTile = Data[x, y, l];
                        if (!hasRotation.ContainsKey(currentTile))
                            hasRotation.Add(currentTile, new bool[4]);
                        hasRotation[currentTile][(int)(Rotations[x, y, l] * 2)] = true;
                    }
                }
            }
            // create new tiles
            List<Tile> mapTiles = new List<Tile>( );
            Dictionary<int, Dictionary<int, int>> startPositions = new Dictionary<int, Dictionary<int, int>>( );
            for (int tile = 0; tile < Tiles.Length; tile++) {
                if (hasRotation.ContainsKey(tile)) {
                    startPositions.Add(tile, new Dictionary<int, int>( ));
                    if (hasRotation[tile][0])
                        startPositions[tile].Add(0, mapTiles.Count);
                    mapTiles.Add(Tiles[tile]);
                    for (int i = 1; i < 4; i++) {
                        if (hasRotation[tile][i]) {
                            startPositions[tile].Add(i, mapTiles.Count);
                            mapTiles.Add(new Tile( ) { Name = Tiles[tile].Name + i + "~", Attributes = Tiles[tile].Attributes, Texture = ShiftTextureCoordinates(i / 2f, Tiles[tile].Texture) });
                        }
                    }
                } else {
                    mapTiles.Add(Tiles[tile]);
                }
            }
            result.Tiles = mapTiles.ToArray( );
            // switch indicies
            for (int l = 0; l < 3; l++) {
                for (int x = 0; x < Width; x++) {
                    for (int y = 0; y < Height; y++) {
                        result.Data[x, y, l] = startPositions[Data[x, y, l]][(int)(Rotations[x, y, l] * 2)];
                    }
                }
            }
            return result;
        }

        public void Compile (string path, Project project) {
            string basedirectory = Path.Combine(path, "maps", Name);
            if (!Directory.Exists(basedirectory))
                Directory.CreateDirectory(basedirectory);
            // build texture
            Texture2D packedTexture = TileSerializer.BuildTexture(Tiles, XnaTextures, project.GraphicsDevice);
            using (Stream stream = File.Open(Path.Combine(basedirectory, Name + ".png"), FileMode.Create))
                packedTexture.SaveAsPng(stream, packedTexture.Width, packedTexture.Height);

            Texture = Path.GetFileNameWithoutExtension(Name + ".png");
            using (Stream stream = File.Open(Path.Combine(basedirectory, Name + ".map"), FileMode.Create))
                CreateCompileVersion( ).Serialize(stream, new WindowsEntitySerializer( ));

            using (Stream stream = File.Open(Path.Combine(basedirectory, Name + "_shadow.png"), FileMode.Create))
                PrerenderShadowMap(stream);
        }

        private float[ ] ShiftTextureCoordinates (float rotation, float[ ] texturecoords) {
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

        private void ExtractRotations ( ) {
            Rotations = new float[Width, Height, 3];

            Dictionary<string, int> startPositions = new Dictionary<string, int>( );
            foreach (Tile tile in Tiles) {
                if (!tile.Name.EndsWith("~")) {
                    startPositions.Add(tile.Name, startPositions.Count);
                }
            }

            for (int l = 0; l < 3; l++) {
                for (int x = 0; x < Width; x++) {
                    for (int y = 0; y < Height; y++) {
                        Tile tile = Tiles[Data[x, y, l]];
                        if (tile.Name.EndsWith("~")) {
                            Data[x, y, l] = startPositions[tile.Name.Substring(0, tile.Name.Length - 2)];
                            Rotations[x, y, l] = float.Parse(tile.Name[tile.Name.Length - 2].ToString( )) / 2f;
                        } else
                            Data[x, y, l] = startPositions[tile.Name];
                    }
                }
            }

            List<Tile> filteredTiles = new List<Tile>( );
            foreach (Tile tile in Tiles) {
                if (!tile.Name.EndsWith("~"))
                    filteredTiles.Add(tile);
            }
            Tiles = filteredTiles.ToArray( );
        }

        public unsafe void PrerenderShadowMap (Stream stream) {
            WriteableBitmap bitmap = new WriteableBitmap(Width * 10, Height * 10, 30, 30, System.Windows.Media.PixelFormats.Bgra32, null);

            bitmap.Lock( );
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {

                    for (int ty = 0; ty < TILE_LIGHT_RESOLUTION; ty++) {
                        for (int tx = 0; tx < TILE_LIGHT_RESOLUTION; tx++) {

                            float lightLvl = 1f;
                            if (HasCollider(x, y)) {

                                for (int cy = 1; cy <= 2; cy++) {
                                    for (int cx = 1; cx <= 2; cx++) {
                                        int px = x + cx, py = y + cy;
                                        if (px >= 0 && px < Width && py >= 0 && py < Height && !HasCollider(px, py)) {
                                            float dx = cx * TILE_LIGHT_RESOLUTION - tx - .5f;
                                            float dy = cy * TILE_LIGHT_RESOLUTION - ty - .5f;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }

                                        py = y - cy;
                                        if (px >= 0 && px < Width && py >= 0 && py < Height && !HasCollider(px, py)) {
                                            float dx = cx * TILE_LIGHT_RESOLUTION - tx - .5f;
                                            float dy = (cy - 1) * TILE_LIGHT_RESOLUTION + ty + .5f;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }

                                        px = x - cx;
                                        if (px >= 0 && px < Width && py >= 0 && py < Height && !HasCollider(px, py)) {
                                            float dx = (cx - 1) * TILE_LIGHT_RESOLUTION + tx + .5f;
                                            float dy = (cy - 1) * TILE_LIGHT_RESOLUTION + ty + .5f;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }

                                        py = y + cy;
                                        if (px >= 0 && px < Width && py >= 0 && py < Height && !HasCollider(px, py)) {
                                            float dx = (cx - 1) * TILE_LIGHT_RESOLUTION + tx + .5f;
                                            float dy = cy * TILE_LIGHT_RESOLUTION - ty - .5f;
                                            lightLvl = Math.Min(lightLvl, (float)Math.Sqrt(dx * dx + dy * dy) / TILE_LIGHT_DISTANCE);
                                        }
                                    }
                                }

                                for (int d = 1; d <= 2; d++) {
                                    int temp = x + d;
                                    if (temp < Width && !HasCollider(temp, y)) {
                                        lightLvl = Math.Min(lightLvl, (d * TILE_LIGHT_RESOLUTION - tx - .5f) / TILE_LIGHT_DISTANCE);
                                    }

                                    temp = x - d;
                                    if (temp > 0 && !HasCollider(temp, y)) {
                                        lightLvl = Math.Min(lightLvl, ((d - 1) * TILE_LIGHT_RESOLUTION + tx + .5f) / TILE_LIGHT_DISTANCE);
                                    }

                                    temp = y + d;
                                    if (temp < Height && !HasCollider(x, temp)) {
                                        lightLvl = Math.Min(lightLvl, (d * TILE_LIGHT_RESOLUTION - ty - .5f) / TILE_LIGHT_DISTANCE);
                                    }

                                    temp = y - d;
                                    if (temp > 0 && !HasCollider(x, temp)) {
                                        lightLvl = Math.Min(lightLvl, ((d - 1) * TILE_LIGHT_RESOLUTION + ty + .5f) / TILE_LIGHT_DISTANCE);
                                    }
                                }

                                lightLvl = 1f - lightLvl;
                            }

                            int backBuffer = (int)bitmap.BackBuffer + (TILE_LIGHT_RESOLUTION * y + ty) * bitmap.BackBufferStride + (TILE_LIGHT_RESOLUTION * x + tx) * 4;
                            *((uint*)backBuffer) = (uint)((byte)(lightLvl * 255) << 24);
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
    }
}
