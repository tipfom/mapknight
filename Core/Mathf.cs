using System;
using System.Runtime.InteropServices;

namespace mapKnight.Core {

    public static class Mathf {
        public const float E = (float)Math.E;
        public const float PI = (float)Math.PI;

        public static float Clamp (float value, float min, float max) {
            if (value < min) return min;
            if (value > max) return max;
            return value;
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
                    float x = Mathf.PI * percent;
                    if (v1 < v2)
                        x += Mathf.PI; // interpolate on the raising part of the cosine function
                    return (float)(v1 + (Mathf.Cos(x) + 1) / 2 * (v1 - v2));

                case Interpolation.Jump:
                    return (percent > 0.5) ? v1 : v2;

                default:
                    return Interpolate(v1, v2, percent, Interpolation.Linear);
            }
        }

        public static Vector2 Interpolate (Vector2 vec1, Vector2 vec2, float percent) {
            return new Vector2(vec1.X + (vec2.X - vec1.X) * percent, vec1.Y + (vec2.Y - vec1.Y) * percent);
        }

        public static float[ ] Transform (float[ ] verticies, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY, float angle, bool mirrored = false) {
            if (verticies.Length % 2 != 0)
                return null;

            float[ ] transformedRotatedVerticies = new float[verticies.Length];
            for (int i = 0; i < verticies.Length / 2; i++) {
                if (mirrored) {
                    transformedRotatedVerticies[i * 2 + 0] = newCenterX - (verticies[i * 2 + 0] - oldCenterX) * Cos(angle) + (verticies[i * 2 + 1] - oldCenterY) * Sin(angle);
                    transformedRotatedVerticies[i * 2 + 1] = newCenterY + (verticies[i * 2 + 0] - oldCenterX) * Sin(angle) + (verticies[i * 2 + 1] - oldCenterY) * Cos(angle);
                } else {
                    transformedRotatedVerticies[i * 2 + 0] = newCenterX + (verticies[i * 2 + 0] - oldCenterX) * Cos(angle) - (verticies[i * 2 + 1] - oldCenterY) * Sin(angle);
                    transformedRotatedVerticies[i * 2 + 1] = newCenterY + (verticies[i * 2 + 0] - oldCenterX) * Sin(angle) + (verticies[i * 2 + 1] - oldCenterY) * Cos(angle);
                }
            }
            return transformedRotatedVerticies;
        }

        public static float[ ] TransformAtOrigin (float[ ] verticies, float x, float y, float angle, Vector2 scale, bool mirrored) {
            float[ ] transformedRotatedVerticies = new float[verticies.Length];
            float s = Sin(angle), c = Cos(angle);
            for (int i = 0; i < verticies.Length; i += 2) {
                if (mirrored) {
                    transformedRotatedVerticies[i + 0] = x - verticies[i] * scale.X * c + verticies[i + 1] * scale.Y * s;
                    transformedRotatedVerticies[i + 1] = y + verticies[i] * scale.X * s + verticies[i + 1] * scale.Y * c;
                } else {
                    transformedRotatedVerticies[i + 0] = x + verticies[i] * scale.X * c - verticies[i + 1] * scale.Y * s;
                    transformedRotatedVerticies[i + 1] = y + verticies[i] * scale.X * s + verticies[i + 1] * scale.Y * c;
                }
            }
            return transformedRotatedVerticies;
        }
        public static float[ ] Translate (float[ ] verticies, float scaleX, float oldCenterX, float oldCenterY, float newCenterX, float newCenterY) {
            if (verticies.Length % 2 != 0)
                return null;

            float[ ] transformedVerticies = new float[verticies.Length];
            float shiftingX = newCenterX - oldCenterX;
            float shiftingY = newCenterY - oldCenterY;
            for (int i = 0; i < verticies.Length / 2; i++) {
                transformedVerticies[i * 2 + 0] = scaleX * verticies[i * 2 + 0] + shiftingX;
                transformedVerticies[i * 2 + 1] = verticies[i * 2 + 1] + shiftingY;
            }
            return transformedVerticies;
        }

        #region lookup table
        private static float[ ] sinLookUpTable = {
            1.000000000f,
            0.999990500f,
            0.999961900f,
            0.999914300f,
            0.999847700f,
            0.999762000f,
            0.999657300f,
            0.999533600f,
            0.999390800f,
            0.999229000f,
            0.999048200f,
            0.998848400f,
            0.998629500f,
            0.998391700f,
            0.998134800f,
            0.997858900f,
            0.997564100f,
            0.997250200f,
            0.996917300f,
            0.996565500f,
            0.996194700f,
            0.995804900f,
            0.995396200f,
            0.994968500f,
            0.994521900f,
            0.994056300f,
            0.993571900f,
            0.993068500f,
            0.992546100f,
            0.992004900f,
            0.991444900f,
            0.990865900f,
            0.990268100f,
            0.989651400f,
            0.989015900f,
            0.988361500f,
            0.987688400f,
            0.986996400f,
            0.986285600f,
            0.985556100f,
            0.984807700f,
            0.984040700f,
            0.983254900f,
            0.982450400f,
            0.981627200f,
            0.980785300f,
            0.979924700f,
            0.979045500f,
            0.978147600f,
            0.977231100f,
            0.976296000f,
            0.975342300f,
            0.974370100f,
            0.973379300f,
            0.972369900f,
            0.971342100f,
            0.970295700f,
            0.969230900f,
            0.968147600f,
            0.967046000f,
            0.965925800f,
            0.964787300f,
            0.963630400f,
            0.962455200f,
            0.961261700f,
            0.960049900f,
            0.958819700f,
            0.957571400f,
            0.956304700f,
            0.955020000f,
            0.953716900f,
            0.952395800f,
            0.951056500f,
            0.949699100f,
            0.948323700f,
            0.946930100f,
            0.945518600f,
            0.944089000f,
            0.942641500f,
            0.941176000f,
            0.939692600f,
            0.938191400f,
            0.936672200f,
            0.935135200f,
            0.933580400f,
            0.932007800f,
            0.930417600f,
            0.928809500f,
            0.927183900f,
            0.925540500f,
            0.923879500f,
            0.922201000f,
            0.920504900f,
            0.918791200f,
            0.917060100f,
            0.915311500f,
            0.913545400f,
            0.911762100f,
            0.909961300f,
            0.908143200f,
            0.906307800f,
            0.904455100f,
            0.902585300f,
            0.900698200f,
            0.898794100f,
            0.896872800f,
            0.894934400f,
            0.892979000f,
            0.891006500f,
            0.889017200f,
            0.887010800f,
            0.884987700f,
            0.882947600f,
            0.880890700f,
            0.878817100f,
            0.876726700f,
            0.874619700f,
            0.872496000f,
            0.870355700f,
            0.868198800f,
            0.866025400f,
            0.863835500f,
            0.861629200f,
            0.859406400f,
            0.857167300f,
            0.854911900f,
            0.852640200f,
            0.850352200f,
            0.848048100f,
            0.845727800f,
            0.843391400f,
            0.841039000f,
            0.838670600f,
            0.836286100f,
            0.833885800f,
            0.831469600f,
            0.829037500f,
            0.826589800f,
            0.824126200f,
            0.821646900f,
            0.819152100f,
            0.816641600f,
            0.814115500f,
            0.811574000f,
            0.809017000f,
            0.806444600f,
            0.803856800f,
            0.801253800f,
            0.798635500f,
            0.796002000f,
            0.793353300f,
            0.790689600f,
            0.788010800f,
            0.785316900f,
            0.782608200f,
            0.779884500f,
            0.777146000f,
            0.774392700f,
            0.771624600f,
            0.768841800f,
            0.766044400f,
            0.763232500f,
            0.760406000f,
            0.757565000f,
            0.754709600f,
            0.751839800f,
            0.748955700f,
            0.746057400f,
            0.743144800f,
            0.740218100f,
            0.737277300f,
            0.734322500f,
            0.731353700f,
            0.728371000f,
            0.725374400f,
            0.722363900f,
            0.719339800f,
            0.716301900f,
            0.713250500f,
            0.710185300f,
            0.707106800f,
            0.704014700f,
            0.700909300f,
            0.697790400f,
            0.694658400f,
            0.691513100f,
            0.688354600f,
            0.685183000f,
            0.681998400f,
            0.678800800f,
            0.675590200f,
            0.672366800f,
            0.669130600f,
            0.665881700f,
            0.662620100f,
            0.659345800f,
            0.656059000f,
            0.652759700f,
            0.649448000f,
            0.646124000f,
            0.642787600f,
            0.639439000f,
            0.636078200f,
            0.632705300f,
            0.629320400f,
            0.625923500f,
            0.622514700f,
            0.619094000f,
            0.615661500f,
            0.612217300f,
            0.608761400f,
            0.605294000f,
            0.601815000f,
            0.598324600f,
            0.594822800f,
            0.591309700f,
            0.587785200f,
            0.584249700f,
            0.580703000f,
            0.577145200f,
            0.573576500f,
            0.569996800f,
            0.566406300f,
            0.562804900f,
            0.559192900f,
            0.555570200f,
            0.551937000f,
            0.548293200f,
            0.544639100f,
            0.540974500f,
            0.537299600f,
            0.533614500f,
            0.529919300f,
            0.526213900f,
            0.522498500f,
            0.518773300f,
            0.515038100f,
            0.511293100f,
            0.507538400f,
            0.503774000f,
            0.500000000f,
            0.496216500f,
            0.492423600f,
            0.488621200f,
            0.484809600f,
            0.480988800f,
            0.477158800f,
            0.473319700f,
            0.469471600f,
            0.465614500f,
            0.461748600f,
            0.457873900f,
            0.453990500f,
            0.450098500f,
            0.446197800f,
            0.442288700f,
            0.438371200f,
            0.434445300f,
            0.430511100f,
            0.426568700f,
            0.422618300f,
            0.418659700f,
            0.414693200f,
            0.410718900f,
            0.406736600f,
            0.402746700f,
            0.398749100f,
            0.394743900f,
            0.390731100f,
            0.386711000f,
            0.382683400f,
            0.378648600f,
            0.374606600f,
            0.370557400f,
            0.366501200f,
            0.362438100f,
            0.358367900f,
            0.354291100f,
            0.350207400f,
            0.346117000f,
            0.342020200f,
            0.337916700f,
            0.333806900f,
            0.329690600f,
            0.325568200f,
            0.321439500f,
            0.317304700f,
            0.313163800f,
            0.309017000f,
            0.304864300f,
            0.300705800f,
            0.296541600f,
            0.292371700f,
            0.288196300f,
            0.284015400f,
            0.279829000f,
            0.275637400f,
            0.271440400f,
            0.267238400f,
            0.263031200f,
            0.258819000f,
            0.254602000f,
            0.250380000f,
            0.246153300f,
            0.241921900f,
            0.237685900f,
            0.233445400f,
            0.229200400f,
            0.224951100f,
            0.220697400f,
            0.216439600f,
            0.212177700f,
            0.207911700f,
            0.203641800f,
            0.199367900f,
            0.195090300f,
            0.190809000f,
            0.186524000f,
            0.182235500f,
            0.177943500f,
            0.173648200f,
            0.169349500f,
            0.165047600f,
            0.160742600f,
            0.156434500f,
            0.152123400f,
            0.147809400f,
            0.143492600f,
            0.139173100f,
            0.134850900f,
            0.130526200f,
            0.126199000f,
            0.121869300f,
            0.117537400f,
            0.113203200f,
            0.108866900f,
            0.104528500f,
            0.100188100f,
            0.095845750f,
            0.091501620f,
            0.087155740f,
            0.082808200f,
            0.078459100f,
            0.074108490f,
            0.069756470f,
            0.065403130f,
            0.061048540f,
            0.056692790f,
            0.052335960f,
            0.047978130f,
            0.043619390f,
            0.039259820f,
            0.034899500f,
            0.030538510f,
            0.026176950f,
            0.021814880f,
            0.017452410f,
            0.013089600f,
            0.008726535f,
            0.004363309f,
            0.000000000f
        };
        #endregion

        public static float Sin (float a) {
            return Cos(a - 90);
        }

        public static float Cos (float a) {
            if (a < 0) a = -a;
            a %= 360f;
            if (a > 180) a = 360 - a;
            if (a > 90) return -sinLookUpTable[(int)((180 - a) * 4)];
            return sinLookUpTable[(int)(a * 4)];
        }

        public static float Sqrt (float z) {
            // http://blog.wouldbetheologian.com/2011/11/fast-approximate-sqrt-method-in-c.html
            if (z == 0) return 0;
            FloatIntUnion u;
            u.tmp = 0;
            float xhalf = 0.5f * z;
            u.f = z;
            u.tmp = 0x5f375a86 - (u.tmp >> 1);
            u.f = u.f * (1.5f - xhalf * u.f * u.f);
            return u.f * z;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion {
            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public int tmp;
        }
    }
}