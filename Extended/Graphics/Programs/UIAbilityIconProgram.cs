using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;
using System;

namespace mapKnight.Extended.Graphics.Programs {
    public class UIAbilityIconProgram : Program {
        public static UIAbilityIconProgram Program;

        public static void Init ( ) {
            Program = new UIAbilityIconProgram( );
        }

        public static void Destroy ( ) {
            Program.Dispose( );
        }

        private UniformMatrixHandle mvpMatrixHandle;
        private TextureHandle baseTextureHandle;
        private AttributeHandle baseTextureCoordsHandle;
        private TextureHandle ampTextureHandle;
        private AttributeHandle ampTextureCoordsHandle;

        public UIAbilityIconProgram ( ) : base("ui_abilityicon.vert", "ui_abilityicon.frag") {
            mvpMatrixHandle = new UniformMatrixHandle(glProgram, "u_mvpMatrix");
            baseTextureCoordsHandle = new AttributeHandle(glProgram, "a_baseTexcoord");
            ampTextureCoordsHandle = new AttributeHandle(glProgram, "a_ampTexcoord");
            baseTextureHandle = new TextureHandle(glProgram, "u_baseTexture");
            ampTextureHandle = new TextureHandle(glProgram, "u_ampTexture");
        }

        public override void Begin ( ) {
            base.Begin( );
            baseTextureCoordsHandle.Enable( );
            ampTextureCoordsHandle.Enable( );
        }

        public override void End ( ) {
            baseTextureCoordsHandle.Disable( );
            ampTextureCoordsHandle.Disable( );
            base.End( );
        }

        public void Draw (IndexBuffer indexBuffer, IAttributeBuffer vertexBuffer, IAttributeBuffer baseTextureBuffer, IAttributeBuffer ampTextureBuffer, Texture2D baseTexture, Texture2D ampTexture, Matrix matrix, int count, int offset, bool alphaBlending = true) {
            indexBuffer.Bind( );
            Apply(vertexBuffer, alphaBlending);

            baseTextureHandle.Set(baseTexture.ID, 0);
            ampTextureHandle.Set(ampTexture.ID, 1);
            baseTextureBuffer.Bind(baseTextureCoordsHandle);
            ampTextureBuffer.Bind(ampTextureCoordsHandle);
            mvpMatrixHandle.Set(matrix.MVP);

            GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedShort, new IntPtr(offset));
        }
    }
}