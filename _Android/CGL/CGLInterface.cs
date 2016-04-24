using System;
using Java.Nio;
using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public class CGLInterface {
        private static Size jumpButtonSize = new Size ((int)(Content.ScreenSize.Height * 0.45f));
        private static Size moveButtonSize = new Size ((int)(Content.ScreenSize.Height * 0.325f));

        delegate void Test ();

        delegate int TestIntVoid (int test222);

        FloatBuffer VertexBuffer;
        ShortBuffer IndexBuffer;
        FloatBuffer TextureBuffer;
        float[] TextureCoords = new float[48];

        CGLSprite<int> sprite;

        public Button JumpButton;
        public Button LeftButton;
        public Button RightButton;

        public CGLInterface (XMLElemental configfile) {
            sprite = new CGLSprite<int> (Content.Context.Assets.Open (configfile["images"].Find ("name", "interface").Attributes["src"]), configfile["images"].Find ("name", "interface").GetAll ());

            short[] Indicies = new short[] {
                0, 1, 2,
                0, 2, 3,
                4, 5, 6,
                4, 6, 7,
                8, 9, 10,
                8, 10, 11,
                12, 13, 14,
                12, 14, 15,
                16, 17, 18,
                16, 18, 19,
                20, 21, 22,
                20, 22, 23
            };

            ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (Indicies.Length * sizeof (short));
            bytebuffer.Order (ByteOrder.NativeOrder ());
            IndexBuffer = bytebuffer.AsShortBuffer ();
            IndexBuffer.Put (Indicies);
            IndexBuffer.Position (0);

            updateVertexBuffer ();
            initTextureBuffer ();
            initButtons ();

            Content.OnUpdate += OnUpdate;
        }

        private void updateVertexBuffer () {
            fSize movebuttonsize = new fSize (.65f, .65f);
            fSize jumpbuttonsize = new fSize (.9f, .9f);

            float[] verticies = new float[] {
                //button 1
                Content.ScreenRatio - movebuttonsize.Width, -1f + movebuttonsize.Height, 
                Content.ScreenRatio, -1f + movebuttonsize.Height, 
                Content.ScreenRatio, -1f, 
                Content.ScreenRatio - movebuttonsize.Width, -1f, 
                //button 2
                Content.ScreenRatio - movebuttonsize.Width, -1f, 
                Content.ScreenRatio - movebuttonsize.Width - movebuttonsize.Width, -1f, 
                Content.ScreenRatio - movebuttonsize.Width - movebuttonsize.Width, -1f + movebuttonsize.Height, 
                Content.ScreenRatio - movebuttonsize.Width, -1f + movebuttonsize.Height, 
                //jump button
                -Content.ScreenRatio, -1f, 
                -Content.ScreenRatio, -1f + jumpbuttonsize.Height, 
                -Content.ScreenRatio + jumpbuttonsize.Width, -1f + jumpbuttonsize.Height, 
                -Content.ScreenRatio + jumpbuttonsize.Width, -1f, 
                //health bar
                -Content.ScreenRatio, 1f, 
                -Content.ScreenRatio + Content.ScreenRatio, 1f,  // * Content.Character.Health.Current / Content.Character.Health.Max
                -Content.ScreenRatio + Content.ScreenRatio, 0.97f,
                -Content.ScreenRatio, 0.97f, 
                //energie bar
                // hat zur zeit die weerte der healtbar
                -Content.ScreenRatio, 0.97f, 
                -Content.ScreenRatio + Content.ScreenRatio, 0.97f, 
                -Content.ScreenRatio + Content.ScreenRatio, 0.94f, 
                -Content.ScreenRatio, 0.94f, 
                // back button
                Content.ScreenRatio, 0.8f, 
                Content.ScreenRatio, 1f, 
                Content.ScreenRatio - 0.2f, 1f, 
                Content.ScreenRatio - 0.2f, 0.8f,
            };
            ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (verticies.Length * sizeof (float));
            bytebuffer.Order (ByteOrder.NativeOrder ());
            VertexBuffer = bytebuffer.AsFloatBuffer ();
            VertexBuffer.Put (verticies);
            VertexBuffer.Position (0);
        }

        private void initButtons () {
            JumpButton = Content.TouchManager.Create (new Point (Content.ScreenSize.Width - jumpButtonSize.Width, 0), jumpButtonSize);
            RightButton = Content.TouchManager.Create (new Point (moveButtonSize.Width, 0), moveButtonSize);
            LeftButton = Content.TouchManager.Create (new Point (0, 0), moveButtonSize);

            JumpButton.OnClick += handleJumpButtonClick;
            RightButton.OnClick += handleRightButtonClick;
            LeftButton.OnClick += handleLeftButtonClick;

            JumpButton.OnLeave += handleJumpButtonLeave;
            RightButton.OnLeave += handleRightButtonLeave;
            LeftButton.OnLeave += handleLeftButtonLeave;
        }

        private void handleJumpButtonClick () {
            Content.Character.Jump ();
            Array.Copy (sprite.Sprites[1].Verticies, 0, TextureCoords, 16, 8);
            updateTextureBuffer ();
        }

        private void handleJumpButtonLeave () {
            Array.Copy (sprite.Sprites[0].Verticies, 0, TextureCoords, 16, 8);
            updateTextureBuffer ();
        }

        private void handleLeftButtonClick () {
            Array.Copy (sprite.Sprites[1].Verticies, 0, TextureCoords, 0, 8);
            updateTextureBuffer ();
        }

        private void handleLeftButtonLeave () {
            Array.Copy (sprite.Sprites[0].Verticies, 0, TextureCoords, 0, 8);
            updateTextureBuffer ();
        }

        private void handleRightButtonClick () {
            Array.Copy (sprite.Sprites[1].Verticies, 0, TextureCoords, 8, 8);
            updateTextureBuffer ();
        }

        private void handleRightButtonLeave () {
            Array.Copy (sprite.Sprites[0].Verticies, 0, TextureCoords, 8, 8);
            updateTextureBuffer ();
        }

        private void initTextureBuffer () {
            // set textures of buttons first time
            Array.Copy (sprite.Sprites[0].Verticies, 0, TextureCoords, 0, 8);
            Array.Copy (sprite.Sprites[0].Verticies, 0, TextureCoords, 8, 8);
            Array.Copy (sprite.Sprites[0].Verticies, 0, TextureCoords, 16, 8);
            Array.Copy (sprite.Sprites[2].Verticies, 0, TextureCoords, 24, 8);
            Array.Copy (sprite.Sprites[3].Verticies, 0, TextureCoords, 32, 8);
            Array.Copy (sprite.Sprites[4].Verticies, 0, TextureCoords, 40, 8);

            ByteBuffer bytebuffer = ByteBuffer.AllocateDirect (TextureCoords.Length * sizeof (float));
            bytebuffer.Order (ByteOrder.NativeOrder ());
            TextureBuffer = bytebuffer.AsFloatBuffer ();
            TextureBuffer.Put (TextureCoords);
            TextureBuffer.Position (0);
        }

        private void updateTextureBuffer () {
            TextureBuffer.Put (TextureCoords);
            TextureBuffer.Position (0);
        }

        private void updateStatBars () {

        }

        public void Draw () {
            Content.MatrixProgram.Begin ();
            Content.MatrixProgram.EnableAlphaBlending ();

            Content.MatrixProgram.SetMVPMatrix (Content.Camera.DefaultMVPMatrix);
            Content.MatrixProgram.SetTexture (this.sprite.Texture);
            Content.MatrixProgram.SetTextureBuffer (TextureBuffer);
            Content.MatrixProgram.SetVertexBuffer (VertexBuffer);
            Content.MatrixProgram.Draw (IndexBuffer);

            Content.MatrixProgram.End ();
        }

        private void OnUpdate () {
            JumpButton.Dispose ();
            RightButton.Dispose ();
            LeftButton.Dispose ();

            jumpButtonSize = new Size ((int)(Content.ScreenSize.Height * 0.45f));
            moveButtonSize = new Size ((int)(Content.ScreenSize.Height * 0.325f));

            JumpButton = Content.TouchManager.Create (new Point (Content.ScreenSize.Width - jumpButtonSize.Width, 0), jumpButtonSize);
            RightButton = Content.TouchManager.Create (new Point (moveButtonSize.Width, 0), moveButtonSize);
            LeftButton = Content.TouchManager.Create (new Point (0, 0), moveButtonSize);

            JumpButton.OnClick += handleJumpButtonClick;
            RightButton.OnClick += handleRightButtonClick;
            LeftButton.OnClick += handleLeftButtonClick;

            JumpButton.OnLeave += handleJumpButtonLeave;
            RightButton.OnLeave += handleRightButtonLeave;
            LeftButton.OnLeave += handleLeftButtonLeave;

            updateVertexBuffer ();
        }
    }
}