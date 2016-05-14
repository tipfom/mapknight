using System;

namespace mapKnight.Basic {
    public static class MathHelper {
        public static float[ ] Rotate (float[ ] verticies, float centerX, float centerY, float angle) {
            if (verticies.Length % 2 != 0)
                return null; // no verticies with format 0 = x, 1 = y available

            angle *= (float)Math.PI / 180f; // convert to radians

            float[ ] rotatedVerticies = new float[verticies.Length];
            for (int i = 0; i < verticies.Length / 2; i++) {
                rotatedVerticies[i * 2 + 0] = centerX + (verticies[i * 2 + 0] - centerX) * (float)Math.Cos (angle) - (verticies[i * 2 + 1] - centerY) * (float)Math.Sin (angle);
                rotatedVerticies[i * 2 + 1] = centerX + (verticies[i * 2 + 0] - centerX) * (float)Math.Sin (angle) + (verticies[i * 2 + 1] - centerY) * (float)Math.Cos (angle);
            }
            return rotatedVerticies;
        }

        public static float[ ] Translate (float[ ] verticies, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY) {
            if (verticies.Length % 2 != 0)
                return null;

            float[ ] transformedVerticies = new float[verticies.Length];
            float shiftingX = newCenterX - oldCenterX;
            float shiftingY = newCenterY - oldCenterY;
            for (int i = 0; i < verticies.Length / 2; i++) {
                transformedVerticies[i * 2 + 0] = verticies[i * 2 + 0] - shiftingX;
                transformedVerticies[i * 2 + 1] = verticies[i * 2 + 1] - shiftingY;
            }
            return transformedVerticies;
        }

        public static float[ ] TranslateRotate (float[ ] verticies, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY, float angle) {
            if (verticies.Length % 2 != 0)
                return null;

            angle *= (float)Math.PI / 180f; // convert to radians

            float[ ] transformedRotatedVerticies = new float[verticies.Length];
            for (int i = 0; i < verticies.Length / 2; i++) {
                transformedRotatedVerticies[i * 2 + 0] = newCenterX + (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Cos (angle) - (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Sin (angle);
                transformedRotatedVerticies[i * 2 + 1] = newCenterY + (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Sin (angle) + (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Cos (angle);
            }
            return transformedRotatedVerticies;
        }

        public static float[ ] TranslateRotateMirror (float[ ] verticies, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY, float angle, bool mirrored) {
            if (verticies.Length % 2 != 0)
                return null;

            angle *= (float)Math.PI / 180f; // convert to radians

            float[ ] transformedRotatedVerticies = new float[verticies.Length];
            for (int i = 0; i < verticies.Length / 2; i++) {
                if (mirrored) {
                    transformedRotatedVerticies[i * 2 + 0] = newCenterX - (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Cos (angle) + (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Sin (angle);
                    transformedRotatedVerticies[i * 2 + 1] = newCenterY + (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Sin (angle) + (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Cos (angle);
                } else {
                    transformedRotatedVerticies[i * 2 + 0] = newCenterX + (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Cos (angle) - (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Sin (angle);
                    transformedRotatedVerticies[i * 2 + 1] = newCenterY + (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Sin (angle) + (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Cos (angle);
                }
            }
            return transformedRotatedVerticies;
        }

        public static float Clamp (float value, float min, float max) {
            return Math.Min (Math.Max (value, min), max);
        }

        public static float Interpolate (float value1, float value2, float percent) {
            return value1 + (value2 - value1) * percent;
        }

        public static Vector2 Interpolate (Vector2 vec1, Vector2 vec2, float percent) {
            return new Vector2 (vec1.X + (vec1.X - vec2.X) * percent, vec1.Y + (vec1.Y - vec2.Y) * percent);
        }

        public static float[ ] GetVerticies (Vector2 size) {
            Vector2 bottomleft = -size / 2;
            Vector2 topright = size / 2;

            return new float[ ] {
                bottomleft.X, bottomleft.Y,
                bottomleft.X, topright.Y,
                topright.X, topright.Y,
                topright.X, bottomleft.Y
            };
        }
    }
}