using System;
using System.Collections.Generic;
using System.IO;
using mapKnight.Core;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace mapKnight.ToolKit.Serializer {
    public static class TileSerializer {
        public static void Serialize (Stream stream, Tile[ ] tiles, Dictionary<string, Texture2D> textures, GraphicsDevice g) {
            Tile[ ] tilesClone = new Tile[tiles.Length];
            for (int i = 0; i < tiles.Length; i++)
                tilesClone[i] = new Tile( ) { Name = tiles[i].Name, Attributes = tiles[i].Attributes };

            Texture2D texture = BuildTexture(tilesClone, textures, g);
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                byte[ ] tilesSerialized = JsonConvert.SerializeObject(tilesClone).Encode( );
                writer.Write(tilesSerialized.Length);
                writer.Write(tilesSerialized);
                texture.SaveAsPng(stream, texture.Width, texture.Height);
            }
        }

        public static Tuple<Tile[ ], Dictionary<string, Texture2D>> Deserialize (Stream stream, GraphicsDevice g) {
            Tile[ ] tiles;
            Texture2D packedTexture;
            using (BinaryReader reader = new BinaryReader(stream)) {
                tiles = JsonConvert.DeserializeObject<Tile[ ]>(reader.ReadBytes(reader.ReadInt32( )).Decode( ));
                using (MemoryStream textureStream = new MemoryStream(reader.ReadBytes((int)(stream.Length - stream.Position)))) {
                    packedTexture = Texture2D.FromStream(g, textureStream);
                }
            }
            return new Tuple<Tile[ ], Dictionary<string, Texture2D>>(tiles, ExtractTextures(packedTexture, tiles, g));
        }

        public static Texture2D BuildTexture (Tile[ ] tiles, Dictionary<string, Texture2D> textures, GraphicsDevice g) {
            int textureTileSize = Map.TILE_PXL_SIZE + 2;
            int textureSizeTL = (int)Math.Sqrt(tiles.Length) + 1;
            int textureSizePXL = textureSizeTL * textureTileSize;
            RenderTarget2D renderTarget = new RenderTarget2D(g, textureSizePXL, textureSizePXL);
            g.SetRenderTarget(renderTarget);

            g.Clear(Color.Transparent);
            using (SpriteBatch batch = new SpriteBatch(g)) {
                batch.Begin(samplerState: SamplerState.PointClamp);
                for (int y = 0; y < textureSizeTL; y++) {
                    for (int x = 0; x < Math.Min(textureSizeTL, tiles.Length - y * textureSizeTL); x++) {
                        int currentIndex = y * textureSizeTL + x;
                        Texture2D tileTexture = textures[tiles[currentIndex].Name];
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
                        tiles[currentIndex].Texture = new float[ ] {
                            startX ,startY + vertexSize,
                            startX ,startY,
                            startX + vertexSize, startY,
                            startX + vertexSize, startY + vertexSize
                        };
                    }
                }
                batch.End( );
            }

            g.SetRenderTarget(null);
            return renderTarget;
        }

        public static Dictionary<string, Texture2D> ExtractTextures (Texture2D original, Tile[ ] tiles, GraphicsDevice g) {
            Dictionary<string, Texture2D> result = new Dictionary<string, Texture2D>( );
            foreach (Tile tile in tiles) {
                RenderTarget2D renderTarget = new RenderTarget2D(g, Map.TILE_PXL_SIZE, Map.TILE_PXL_SIZE);
                g.SetRenderTarget(renderTarget);
                g.Clear(Color.Transparent);
                using (SpriteBatch batch = new SpriteBatch(g)) {
                    batch.Begin(samplerState: SamplerState.PointWrap);
                    Rectangle sourceRectangle = new Rectangle((int)Math.Round(original.Width * tile.Texture[0]), (int)Math.Round(original.Height * tile.Texture[3]), Map.TILE_PXL_SIZE, Map.TILE_PXL_SIZE);
                    batch.Draw(original, new Rectangle(0, 0, Map.TILE_PXL_SIZE, Map.TILE_PXL_SIZE), sourceRectangle, Color.White);
                    batch.End( );
                }
                g.SetRenderTarget(null);
                result.Add(tile.Name, renderTarget);
            }
            return result;

        }
    }
}
