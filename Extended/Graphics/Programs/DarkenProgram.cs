using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;
using System;

namespace mapKnight.Extended.Graphics.Programs {
    public class DarkenProgram : TextureProgram {
        public static DarkenProgram Program;

        public static void Init ( ) {
            Program = new DarkenProgram( );
        }

        public static void Destroy ( ) {
            Program.Dispose( );
        }
        
        private UniformMatrixHandle mvpMatrixHandle;
        private UniformVec1Handle brightnessHandle;

        public DarkenProgram ( ) : base(Assets.GetVertexShader("matrix"), Assets.GetFragmentShader("darken")) {
            mvpMatrixHandle = new UniformMatrixHandle(glProgram, "u_mvpmatrix");
            brightnessHandle = new UniformVec1Handle(glProgram, "u_brightness");
        }

        public void Draw (IndexBuffer indexbuffer, IAttributeBuffer vertexbuffer, IAttributeBuffer texturebuffer, float darkness, Texture2D texture, Matrix matrix, int count, int offset, bool alphablending = true) {
            Apply(texture.ID, indexbuffer, vertexbuffer, texturebuffer, alphablending);
            mvpMatrixHandle.Set(matrix.MVP);
            brightnessHandle.Set(darkness);

            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, new IntPtr(offset));
        }
    }
}
