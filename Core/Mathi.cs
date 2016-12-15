using System;

namespace mapKnight.Core {
    public static class Mathi {
        private static int x, y, z, w;

        static Mathi ( ) {
            Random random = new Random(Environment.TickCount);
            x = random.Next( );
            y = random.Next( );
            z = random.Next( );
            w = random.Next( );
        }

        public static int Ceil (float value) {
            int valuei = (int)value;
            if (value > 0 && value != valuei)
                return valuei + 1;
            return valuei;
        }

        public static int Clamp (int value, int min, int max) {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static int Floor (float value) {
            int valuei = (int)value;
            if (value < 0 && valuei != value)
                return valuei - 1;
            return valuei;
        }

        public static int Random ( ) {
            int t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;
            w ^= (w >> 19) ^ t ^ (t >> 8);
            return w;
        }

        public static int Random (int max) {
            return (int)(Mathf.Random( ) * max);
        }

        public static int Random (int min, int max) {
            return min + (int)(Mathf.Random( ) * (max - min));
        }
    }
}