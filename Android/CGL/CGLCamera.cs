using Android.Opengl;
using mapKnight.Basic;
using System;

namespace mapKnight.Android.CGL {
    public class CGLCamera {
        public Vector2 CurrentMapTile;

        public CGLMatrix MapMatrix { get; private set; } = new CGLMatrix ();

        public Vector2 ScreenCentre { get; private set; }
        public Vector2 DrawRange { get; private set; }

        public CGLCamera (float characteroffset) {
            CurrentMapTile = new Vector2 (-1, -1);

            ScreenCentre = new Vector2 ();
            DrawRange = new Vector2 (10, 10);
        }

        public void Update (Vector2 focusPoint, CGLMap map) {
            Vector2 nextMapTile = new Vector2 (
                (focusPoint.X - map.DrawSize.Width / 2f).FitBounds (-1, map.Size.Width - map.DrawSize.Width),
                (focusPoint.Y - map.DrawSize.Height / 2f).FitBounds (-1, map.Size.Height - map.DrawSize.Height));

            MapMatrix.ResetView ();

            float mapOffsetX = nextMapTile.X > 0 ? -((nextMapTile.X) % 1) * map.VertexSize : (-nextMapTile.X) * map.VertexSize;
            float mapOffsetY = nextMapTile.Y > 0 ? -((nextMapTile.Y) % 1) * map.VertexSize : (-nextMapTile.Y) * map.VertexSize;

            Matrix.TranslateM (MapMatrix.View, 0, mapOffsetX, mapOffsetY, 0f);

            MapMatrix.CalculateMVP ();

            Vector2 nextMapTileINT = new Vector2 ((int)Math.Max (0, nextMapTile.X), (int)Math.Max (0, nextMapTile.Y));
            if (CurrentMapTile != nextMapTileINT) {
                CurrentMapTile = nextMapTileINT;
                map.UpdateTextureBuffer ();
            }

            ScreenCentre = nextMapTile + (Vector2)map.DrawSize / 2;
        }
    }
}

