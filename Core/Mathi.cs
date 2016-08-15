using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Core {

    public static class Mathi {

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
    }
}