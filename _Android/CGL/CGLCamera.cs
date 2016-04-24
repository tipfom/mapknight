using Android.Opengl;
using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public class CGLCamera {
        public Point CurrentMapTile;

        private float[] DefaultProjectionMatrix;
        private float[] DefaultViewMatrix;
        public float[] DefaultMVPMatrix { get; private set; }

        private float[] MapViewMatrix;
        public float[] MapMVPMatrix { get; private set; }

        private float[] CharacterViewMatrix;
        public float[] CharacterMVPMatrix { get; private set; }

        private float characterOffset;

        public CGLCamera (float characteroffset) {
            CurrentMapTile = new Point ();
            characterOffset = characteroffset;

            DefaultProjectionMatrix = new float[16];
            DefaultViewMatrix = new float[16];
            DefaultMVPMatrix = new float[16];
            UpdateDefaultMatrix ();

            MapViewMatrix = new float[16];
            MapMVPMatrix = new float[16];

            CharacterViewMatrix = new float[16];
            CharacterMVPMatrix = new float[16];

            Content.OnUpdate += () => {
                UpdateDefaultMatrix ();
            };
        }

        private void UpdateDefaultMatrix () {
            float ratio = Content.ScreenRatio;
            Matrix.OrthoM (DefaultProjectionMatrix, 0, -Content.ScreenRatio, Content.ScreenRatio, -1, 1, 3, 7);
            Matrix.SetLookAtM (DefaultViewMatrix, 0, 0, 0, 3, 0f, 0f, 0f, 0f, 1f, 0f);
            Matrix.MultiplyMM (DefaultMVPMatrix, 0, DefaultProjectionMatrix, 0, DefaultViewMatrix, 0);
        }

        public void Update () {
            fVector2D characterTile = Content.Character.AABB.Centre;
            fPoint nextMapTile = new fPoint (
                (characterTile.X - Content.Map.RealDrawSize.Width / 2f).FitBounds (0f, (Content.Map.Size.Width - Content.Map.DrawSize.Width)),
                (characterTile.Y - (1 - characterOffset) * Content.Map.DrawSize.Height / 2f).FitBounds (0f, Content.Map.Size.Height - Content.Map.DrawSize.Height));

            CharacterViewMatrix = (float[])DefaultViewMatrix.Clone ();
            MapViewMatrix = (float[])DefaultViewMatrix.Clone ();

            float mapOffsetY = 0f;
            float mapOffsetX = 0f;
            float charOffsetY = 0f;
            float charOffsetX = 0f;

            if (nextMapTile.X > 0f && nextMapTile.X < Content.Map.Size.Width - Content.Map.DrawSize.Width) {
                mapOffsetX = characterTile.X % 1;
                mapOffsetX = -2f * mapOffsetX * Content.ScreenRatio / (Content.Map.DrawSize.Width);
            } else if (nextMapTile.X > 0) {
                // on the right side
                charOffsetX = Content.ScreenRatio - 2f * ((Content.Map.Size.Width - characterTile.X - 2) / Content.Map.DrawSize.Width) * Content.ScreenRatio;
            } else {
                // on the left side
                mapOffsetX = Content.Map.VertexSize;
                charOffsetX = -2f * ((Content.Map.RealDrawSize.Width / 2 - characterTile.X) / Content.Map.DrawSize.Width) * Content.ScreenRatio;
            }

            if (nextMapTile.Y > 0 && nextMapTile.Y < Content.Map.Size.Height - Content.Map.DrawSize.Height) {
                mapOffsetY = (characterTile.Y - (1 - characterOffset) * Content.Map.DrawSize.Height / 2f);
                mapOffsetY -= (int)mapOffsetY;
                mapOffsetY = -2f * mapOffsetY / (Content.Map.RealDrawSize.Height);

                charOffsetY = -characterOffset;
            } else if (nextMapTile.Y > 0) {
                charOffsetY = 1 - 2f * (Content.Map.Size.Height - characterTile.Y - 1) / ((float)Content.Map.DrawSize.Height);
                mapOffsetY = Content.Map.DrawSize.Height - Content.Map.RealDrawSize.Height;
            } else {
                charOffsetY = -1 + 2f * characterTile.Y / Content.Map.RealDrawSize.Height;
            }

            Matrix.TranslateM (CharacterViewMatrix, 0, charOffsetX, charOffsetY, 0f);
            Matrix.TranslateM (MapViewMatrix, 0, mapOffsetX, mapOffsetY, 0f);

            Matrix.MultiplyMM (CharacterMVPMatrix, 0, DefaultProjectionMatrix, 0, CharacterViewMatrix, 0);
            Matrix.MultiplyMM (MapMVPMatrix, 0, DefaultProjectionMatrix, 0, MapViewMatrix, 0);

            CurrentMapTile.X = (int)nextMapTile.X;
            CurrentMapTile.Y = (int)nextMapTile.Y;
            Content.Map.updateTextureBuffer ();
        }
    }
}

