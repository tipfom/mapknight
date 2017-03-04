using mapKnight.Core;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Extended.Graphics.Programs;
using mapKnight.Extended.Graphics.Buffer;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics {
    public class SpriteBatch : Texture2D {
        public Dictionary<string, float[ ]> Sprites { get; private set; } = new Dictionary<string, float[ ]>( );

        public SpriteBatch (Texture2D texture) : base(texture.ID, texture.Size, texture.Name) { }

        public SpriteBatch (Dictionary<string, int[ ]> content, Texture2D texture) :
            this(content, texture.ID, texture.Name, texture.Size) {

        }

        public SpriteBatch (Dictionary<string, int[ ]> content, int id, string name, Size size) : base(id, size, name) {
            if (content != null) {
                foreach (var sprite in content) {
                    Add(sprite.Key, sprite.Value);
                }
            }
        }

        public float[ ] Get (string name) {
            return Sprites[name];
        }

        public void Add (string name, int[ ] data) {
            float top = (float)(data[1]) / Height;
            float bottom = (float)(data[1] + data[3]) / Height;
            float left = (float)data[0] / Width;
            float right = (float)(data[0] + data[2]) / Width;

            Sprites.Add(name, new float[ ] { left, top, left, bottom, right, bottom, right, top });
        }

        public static SpriteBatch Combine (bool diposeChildren, List<SpriteBatch> children) {
            children.Sort((a, b) => { return -a.Size.Height.CompareTo(b.Size.Height); });
            Size size = new Size(children.Sum(batch => batch.Width), children[0].Height);
            Framebuffer buffer = new Framebuffer(size.Width, size.Height, false);
            SpriteBatch result = new SpriteBatch(null, buffer.Texture, "csprite", size);

            buffer.Bind( );
            FBOProgram.Program.Begin( );
            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            IndexBuffer indexBuffer = new IndexBuffer(1);
            ClientBuffer vertexBuffer = new ClientBuffer(2, 1, PrimitiveType.Quad);
            ClientBuffer textureBuffer = new ClientBuffer(2, 1, PrimitiveType.Quad);
            textureBuffer.Data = new[ ] { 0f, 1f, 0f, 0f, 1f, 0f, 1f, 1f };
            int position = 0;

            for (int i = 0; i < children.Count; i++) {
                // vertex buffer füllen
                float vleft = -1f + 2 * position / (float)size.Width;
                float vright = -1f + 2 * (position + children[i].Width) / (float)size.Width;
                float vtop = -1f + 2f * (children[i].Height / (float)size.Height);
                vertexBuffer.Data = new[ ] { vleft, vtop, vleft, -1f, vright, -1f, vright, vtop };
                FBOProgram.Program.Draw(indexBuffer, vertexBuffer, textureBuffer, children[i], 6, true);

                float tleft = position / (float)size.Width;
                float theight = children[i].Height / (float)size.Height;
                float twidth = children[i].Width / (float)size.Width;
                foreach (KeyValuePair<string, float[ ]> entry in children[i].Sprites) {
                    // sprites einfügen
                    result.Sprites.Add(entry.Key, new[ ] {
                        tleft + entry.Value[0] * twidth, entry.Value[1] * theight,
                        tleft + entry.Value[2] * twidth, entry.Value[3] * theight,
                        tleft + entry.Value[4] * twidth, entry.Value[5] * theight,
                        tleft + entry.Value[6] * twidth, entry.Value[7] * theight});
                }
                position += children[i].Width;
            }
            FBOProgram.Program.End( );
            buffer.Unbind( );
            GL.ClearColor(Window.Background.R / 255f, Window.Background.G / 255f, Window.Background.B / 255f, Window.Background.A / 255f);

            if (diposeChildren) {
                for (int i = 0; i < children.Count; i++)
                    children[i].Dispose( );
            }

            return result;
        }
    }
}
