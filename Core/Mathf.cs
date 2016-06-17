using System;

namespace mapKnight.Core {
    public static class Mathf {
        public const float PI = (float)Math.PI;
        public const float E = (float)Math.E;

        public static float[ ] Translate (float[ ] verticies, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY) {
            if (verticies.Length % 2 != 0)
                return null;

            float[ ] transformedVerticies = new float[verticies.Length];
            float shiftingX = newCenterX - oldCenterX;
            float shiftingY = newCenterY - oldCenterY;
            for (int i = 0; i < verticies.Length / 2; i++) {
                transformedVerticies[i * 2 + 0] = verticies[i * 2 + 0] + shiftingX;
                transformedVerticies[i * 2 + 1] = verticies[i * 2 + 1] + shiftingY;
            }
            return transformedVerticies;
        }

        public static float[ ] Transform (float[ ] verticies, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY, float angle, bool mirrored =  false) {
            if (verticies.Length % 2 != 0)
                return null;

            angle *= (float)Math.PI / 180f; // convert to radians

            float[ ] transformedRotatedVerticies = new float[verticies.Length];
            for (int i = 0; i < verticies.Length / 2; i++) {
                if (mirrored) {
                    transformedRotatedVerticies[i * 2 + 0] = newCenterX - (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Cos(angle) + (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Sin(angle);
                    transformedRotatedVerticies[i * 2 + 1] = newCenterY + (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Sin(angle) + (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Cos(angle);
                } else {
                    transformedRotatedVerticies[i * 2 + 0] = newCenterX + (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Cos(angle) - (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Sin(angle);
                    transformedRotatedVerticies[i * 2 + 1] = newCenterY + (verticies[i * 2 + 0] - oldCenterX) * (float)Math.Sin(angle) + (verticies[i * 2 + 1] - oldCenterY) * (float)Math.Cos(angle);
                }
            }
            return transformedRotatedVerticies;
        }

        public static float Clamp (float value, float min, float max) {
            return Math.Min(Math.Max(value, min), max);
        }

        public static float Clamp01 (float value) {
            return Clamp(value, 0, 1);
        }

        public static float Interpolate (float v1, float v2, float percent) {
            return Interpolate(v1, v2, percent, Interpolation.Linear);
        }

        public static float Interpolate (float v1, float v2, float percent, Interpolation mode) {
            switch (mode) {
                case Interpolation.Linear:
                    return v1 + (v2 - v1) * percent;
                case Interpolation.Cosine:
                    double x = Math.PI * percent;
                    if (v1 < v2)
                        x += Math.PI; // interpolate on the raising part of the cosine function
                    return (float)(v1 + (Math.Cos(x) + 1) / 2 * (v1 - v2));
                case Interpolation.Jump:
                    return (percent > 0.5) ? v1 : v2;
                default:
                    return Interpolate(v1, v2, percent, Interpolation.Linear);
            }
        }

        public static Vector2 Interpolate (Vector2 vec1, Vector2 vec2, float percent) {
            return new Vector2(vec1.X + (vec2.X - vec1.X) * percent, vec1.Y + (vec2.Y - vec1.Y) * percent);
        }
    }
}