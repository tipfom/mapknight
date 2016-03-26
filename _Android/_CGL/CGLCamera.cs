using System;
using Android.Opengl;
using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public class CGLCamera {
        public Point CurrentMapTile;

        public float[] DefaultProjectionMatrix { get; private set; }

        public float[] DefaultViewMatrix { get; private set; }

        public float[] DefaultMVPMatrix { get; private set; }


        public float[] MapViewMatrix { get; private set; }

        public float[] MapMVPMatrix { get; private set; }


        public float[] CharacterViewMatrix { get; private set; }

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
            Matrix.FrustumM (DefaultProjectionMatrix, 0, -Content.ScreenRatio, Content.ScreenRatio, -1, 1, 3, 7);
            Matrix.SetLookAtM (DefaultViewMatrix, 0, 0, 0, -3, 0f, 0f, 0f, 0f, 1f, 0f);
            Matrix.MultiplyMM (DefaultMVPMatrix, 0, DefaultProjectionMatrix, 0, DefaultViewMatrix, 0);
        }

        public void Update () {
            fPoint CharacterCenter = new fPoint (Content.Character.Position.X + Content.Character.Bounds.Width * 0.5f,
                                         Content.Character.Position.Y + Content.Character.Bounds.Height * 0.5f);
            fPoint CharacterTile = new fPoint (CharacterCenter.X / PhysX.PhysXMap.TILE_BOX_SIZE, CharacterCenter.Y / PhysX.PhysXMap.TILE_BOX_SIZE);
            // berechnet die werte für die map abhängig von der Character position

            Matrix.SetLookAtM (CharacterViewMatrix, 0, 0, 0, -3f, 0, 0f, 0, 0f, 1f, 0f);
            Matrix.SetLookAtM (MapViewMatrix, 0, 0, 0, -3f, 0, 0, 0, 0, 1f, 0);

            float mapOffsetY = 0f;
            float mapOffsetX = 0f;
            float charOffsetY = 0f;
            float charOffsetX = 0f;

            CurrentMapTile.X = Math.Min (Math.Max ((int)CharacterTile.X - (int)(Content.Map.DrawSize.Width / 2), 0), (int)(Content.Map.Size.Width - Content.Map.DrawSize.Width));
            if (CharacterTile.X > Content.Map.DrawSize.Width / 2f && CharacterTile.X < (Content.Map.Size.Width - Content.Map.RealDrawSize.Width / 2f)) {
                mapOffsetX = CharacterTile.X;
                mapOffsetX -= (int)mapOffsetX;
                //mapOffsetX -= Content.Map.DrawSize.Width / Content.Map.RealDrawSize.Width;
                mapOffsetX = 2f * mapOffsetX * Content.ScreenRatio / (Content.Map.DrawSize.Width);
                //mapOffsetX -= Content.Character.Bounds.Width / PhysX.PhysXMap.TILE_BOX_SIZE / Content.Map.DrawSize.Width;
            } else if (CharacterTile.X > Content.Map.DrawSize.Width / 2) {
                // on the right side
                charOffsetX = -Content.ScreenRatio + 2f * ((Content.Map.Size.Width - CharacterTile.X) / Content.Map.DrawSize.Width) * Content.ScreenRatio;
            } else {
                // on the left side
                charOffsetX = 2f * ((Content.Map.DrawSize.Width / 2 - CharacterTile.X) / Content.Map.RealDrawSize.Width) * Content.ScreenRatio;
            }

            if (CharacterTile.Y > (1 - characterOffset) * Content.Map.RealDrawSize.Height / 2 && CharacterTile.Y < (Content.Map.Size.Height - (1 + characterOffset) * Content.Map.DrawSize.Height / 2)) {
                mapOffsetY = (CharacterTile.Y - (1 - characterOffset) * Content.Map.RealDrawSize.Height / 2f);//CharacterTile.Y;// - characterOffset * Content.Map.DrawSize.Height / 2;
                mapOffsetY -= (int)mapOffsetY;
                mapOffsetY = -2f * mapOffsetY / (Content.Map.RealDrawSize.Height);

                charOffsetY = -characterOffset;

                CurrentMapTile.Y = (int)(CharacterTile.Y - (1 - characterOffset) * Content.Map.RealDrawSize.Height / 2);

            } else if (CharacterTile.Y > Content.Map.DrawSize.Height / 2) {
                charOffsetY = 1 - 2f * (Content.Map.Size.Height - (CharacterCenter.Y / PhysX.PhysXMap.TILE_BOX_SIZE)) / ((float)Content.Map.DrawSize.Height);
                mapOffsetY = Content.Map.DrawSize.Height - Content.Map.RealDrawSize.Height - 2;
            } else {
                charOffsetY = -1 + 2f * CharacterTile.Y / Content.Map.RealDrawSize.Height;
                CurrentMapTile.Y = 0;
            }

            Matrix.TranslateM (CharacterViewMatrix, 0, charOffsetX, charOffsetY, 0f);
            Matrix.TranslateM (MapViewMatrix, 0, mapOffsetX, mapOffsetY, 0f);

            Matrix.MultiplyMM (CharacterMVPMatrix, 0, DefaultProjectionMatrix, 0, CharacterViewMatrix, 0);
            Matrix.MultiplyMM (MapMVPMatrix, 0, DefaultProjectionMatrix, 0, MapViewMatrix, 0);
            Log.All (this, mapOffsetX.ToString (), MessageType.Debug);
            Content.Map.updateTextureBuffer ();
        }
    }
}

