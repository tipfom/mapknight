using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;
using OpenTK;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.Programs {
    public class ColorProgram : Program {
        private UniformMatrixHandle mvpMatrixHandle;
        private AttributeHandle colorHandle;

        public ColorProgram ( ) : base(Assets.GetVertexShader("color"), Assets.GetFragmentShader("color")) {
            mvpMatrixHandle = new UniformMatrixHandle(glProgram, "u_mvpmatrix");
            colorHandle = new AttributeHandle(glProgram, "a_color");
        }

        public override void Begin ( ) {
            colorHandle.Enable( );
            base.Begin( );
        }

        public override void End ( ) {
            colorHandle.Disable( );
            base.End( );
        }

        public void Draw (ColorBufferBatch buffer, Texture2D texture, Matrix4 matrix, bool alphaBlending = true) {
            Draw(buffer, texture, matrix, buffer.QuadCount * 6, alphaBlending);
        }

        public void Draw (ColorBufferBatch buffer, Texture2D texture, Matrix4 matrix, int count, bool alphaBlending = true) {
            Apply(texture.ID, buffer.Verticies, buffer.Dimesions, buffer.Texture, alphaBlending);
            colorHandle.Set(buffer.Color, 4);
            mvpMatrixHandle.Set(matrix);

            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, buffer.Indicies);
        }
    
        public static ColorProgram Program { get; set; }
    }
}
