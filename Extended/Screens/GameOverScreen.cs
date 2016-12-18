using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using Map = mapKnight.Extended.Graphics.Map;
using static mapKnight.Extended.Graphics.Programs.MatrixProgram;
using mapKnight.Extended.Graphics.Buffer;
using System;
using System.Linq;

namespace mapKnight.Extended.Screens {

    public class GameOverScreen : Screen {
        const float HEAD_HEIGHT = 0.5f;
        const float HEAD_ROTATION = 30f;
        const float TIP_WIDTH = 0.15f;
        const float HEAD_X = -1f;
        const float HEAD_Y = .1f;

        private Map map;
        private SpriteBatch poleTexture;
        private SpriteBatch playerTexture;

        private BufferBatch poleBuffer;
        private BufferBatch headBuffer;

        public GameOverScreen (Map map, SpriteBatch playerTexture) {
            this.map = map;
            this.playerTexture = playerTexture;
        }

        public override void Load ( ) {
            new UIDim(this, 0.15f, UIDepths.BACKGROUND).Release += ( ) => {
                Screen.Active = Screen.MainMenu;
                headBuffer.Dispose( );
            };
            new UILabel(this, new UIHorizontalCenterMargin(0.7f), new UIVerticalCenterMargin(0f), 0.1f, "You Died!\nSince there are no Stats,\nthis is a sample text\nto embarrass you!", UITextAlignment.Center);

            {
                // setup head
                float[ ] textureUVs = playerTexture.Get("head");
                int headWidthPXL = (int)(Math.Abs(textureUVs[0] - textureUVs[4]) * playerTexture.Width);
                int headHeightPXL = (int)(Math.Abs(textureUVs[3] - textureUVs[1]) * playerTexture.Height);
                float vertexWidth = HEAD_HEIGHT / headHeightPXL * headWidthPXL;
                GPUBuffer vertexBuffer = new GPUBuffer(2, 1, PrimitiveType.Quad);
                vertexBuffer.Put(Mathf.TransformAtOrigin(new float[ ] { -vertexWidth, HEAD_HEIGHT, -vertexWidth, -HEAD_HEIGHT, vertexWidth, -HEAD_HEIGHT, vertexWidth, HEAD_HEIGHT }, HEAD_X, HEAD_Y, HEAD_ROTATION, false));
                GPUBuffer textureBuffer = new GPUBuffer(2, 1, PrimitiveType.Quad);
                textureBuffer.Put(textureUVs);
                headBuffer = new BufferBatch(new IndexBuffer(1), vertexBuffer, textureBuffer);
            }
            {
                // setup pole
                poleTexture = Assets.Load<SpriteBatch>("pole");
                float[ ] tipUVs = poleTexture.Get("tip");
                int tipWidthPXL = (int)(Math.Abs(tipUVs[0] - tipUVs[4]) * poleTexture.Width);
                int tipHeightPXL = (int)(Math.Abs(tipUVs[3] - tipUVs[1]) * poleTexture.Height);
                float vertexHeight = TIP_WIDTH / tipWidthPXL * tipHeightPXL;
                GPUBuffer vertexBuffer = new GPUBuffer(2, 2, PrimitiveType.Quad);
                float l = HEAD_X - TIP_WIDTH / 2, r = HEAD_X + TIP_WIDTH / 2, t = HEAD_Y + HEAD_HEIGHT, pt = t + vertexHeight;
                vertexBuffer.Put(new float[ ]{
                    l, t, l, -1f, r, -1f, r, t, // pole
                    l, pt, l, t, r, t, r, pt // tip
                });
                GPUBuffer textureBuffer = new GPUBuffer(2, 2, PrimitiveType.Quad);
                textureBuffer.Put(poleTexture.Get("pole").Concat(poleTexture.Get("tip")).ToArray( ));
                poleBuffer = new BufferBatch(new IndexBuffer(2), vertexBuffer, textureBuffer);
            }
        }

        public override void Update (DeltaTime dt) {
            map.Update(dt);
            base.Update(dt);
        }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );

            Program.Begin( );
            Program.Draw(poleBuffer, poleTexture, Matrix.Default, true);
            Program.Draw(headBuffer, playerTexture, Matrix.Default, true);
            Program.End( );
        }
    }
}