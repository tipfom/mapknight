using System;
using System.Collections;
using System.Collections.Generic;
#if __ANDROID__
using System.Linq;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Programs;
using OpenTK.Graphics.ES20;
#endif

namespace mapKnight.Core.Graphics {
    public class Spritebatch2D : Texture2D{
        private Dictionary<string, float[ ]> sprites = new Dictionary<string, float[ ]>( );

        public float[] this[string name] {
            get {
                return sprites[name];
            }
            set {
                sprites[name] = value;
            }
        }

        public Spritebatch2D(Texture2D texture) : base(texture.ID, texture.Size, texture.Name) {
        }

        public Spritebatch2D(Dictionary<string, int[ ]> content, Texture2D texture) : this(content, texture.ID, texture.Size, texture.Name) {
        }

        public Spritebatch2D(Dictionary<string, int[ ]> Content, int ID, Size Size, string Name = "%") : base(ID, Size, Name) {
            if (Content != null) {
                foreach (KeyValuePair<string, int[ ]> sprite in Content) {
                    Add(sprite.Key, sprite.Value);
                }
            }
        }

        public void Add(string name, int[] data) {
            float top = (float)(data[1]) / Height;
            float bottom = (float)(data[1] + data[3]) / Height;
            float left = (float)data[0] / Width;
            float right = (float)(data[0] + data[2]) / Width;

            Add(name, new float[ ] { left, top, left, bottom, right, bottom, right, top });
        }

        public void Add(string name, float[] uvs) {
            sprites.Add(name, uvs);
        }
        
        public IEnumerable<string> Sprites( ) {
            foreach (string s in sprites.Keys)
                yield return s;
        }

#if __ANDROID__
        public static Spritebatch2D Combine (bool diposeChildren, List<Spritebatch2D> children) {
            children.Sort((a, b) => { return -a.Size.Height.CompareTo(b.Size.Height); });
            Size size = new Size(children.Sum(batch => batch.Width), children[0].Height);
            Framebuffer buffer = new Framebuffer(size.Width, size.Height, false);
            Spritebatch2D result = new Spritebatch2D(new Texture2D(buffer.Texture, size, "csprite"));

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
                foreach (KeyValuePair<string, float[ ]> entry in children[i].sprites) {
                    // sprites einfügen
                    result.Add(entry.Key, new[ ] {
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
#endif
    }
}
