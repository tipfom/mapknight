using Android.Opengl;
using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public class CGLCamera {
        public Point CurrentMapTile;

        public CGLMatrix MapMatrix { get; private set; }

        public fVector2D ScreenCentre { get; private set; }
        public fVector2D CharacterCentreOnScreen { get; private set; }

        private float characterOffset;

        public CGLCamera (float characteroffset) {
            CurrentMapTile = new Point ( );
            characterOffset = characteroffset;

            MapMatrix = new CGLMatrix ( );
            ScreenCentre = new fVector2D ( );
            CharacterCentreOnScreen = new fVector2D ( );
        }

        public void Update (fVector2D characterPosition, CGLMap map) {
            fPoint nextMapTile = new fPoint (
                (characterPosition.X - map.RealDrawSize.Width / 2f).FitBounds (0f, (map.Size.Width - map.DrawSize.Width)),
                (characterPosition.Y - (1 - characterOffset) * map.DrawSize.Height / 2f).FitBounds (0f, map.Size.Height - map.DrawSize.Height));

            MapMatrix.ResetView ( );

            float mapOffsetY = 0f;
            float mapOffsetX = 0f;
            float charOffsetY = 0f;
            float charOffsetX = 0f;

            if (nextMapTile.X > 0f && nextMapTile.X < map.Size.Width - map.DrawSize.Width) {
                mapOffsetX = characterPosition.X % 1;
                mapOffsetX = -2f * mapOffsetX * Screen.ScreenRatio / (map.DrawSize.Width);
            } else if (nextMapTile.X > 0) {
                // on the right side
                charOffsetX = Screen.ScreenRatio - 2f * ((map.Size.Width - characterPosition.X - 2) / map.DrawSize.Width) * Screen.ScreenRatio;
            } else {
                // on the left side
                mapOffsetX = map.VertexSize;
                charOffsetX = -2f * ((map.RealDrawSize.Width / 2 - characterPosition.X) / map.DrawSize.Width) * Screen.ScreenRatio;
            }

            if (nextMapTile.Y > 0 && nextMapTile.Y < map.Size.Height - map.DrawSize.Height) {
                mapOffsetY = (characterPosition.Y - (1 - characterOffset) * map.DrawSize.Height / 2f);
                mapOffsetY -= (int)mapOffsetY;
                mapOffsetY = -2f * mapOffsetY / (map.RealDrawSize.Height);

                charOffsetY = -characterOffset;
            } else if (nextMapTile.Y > 0) {
                charOffsetY = 1 - 2f * (map.Size.Height - characterPosition.Y - 1) / ((float)map.DrawSize.Height);
                mapOffsetY = map.DrawSize.Height - map.RealDrawSize.Height;
            } else {
                charOffsetY = -1 + 2f * characterPosition.Y / map.RealDrawSize.Height;
            }



            Matrix.TranslateM (CharacterMatrix.View, 0, charOffsetX, charOffsetY, 0f);
            Matrix.TranslateM (MapMatrix.View, 0, mapOffsetX, mapOffsetY, 0f);

            CharacterMatrix.CalculateMVP ( );
            MapMatrix.CalculateMVP ( );

            CurrentMapTile.X = (int)nextMapTile.X;
            CurrentMapTile.Y = (int)nextMapTile.Y;
        }
    }
}

