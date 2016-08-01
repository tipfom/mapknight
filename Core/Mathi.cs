using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Core {
    public static class Mathi {
        public static int Floor (float value) {
            if (value < 0)
                return (int)value - 1;
            return (int)value;
        }

        public static int Ceil (float value) {
            if (value > 0)
                return (int)value + 1;
            return (int)value;
        }

        public static int Clamp (int value, int min, int max) {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
