using System;

namespace mapKnight.Android {
    public static class Log {
        public static void Print (object sender, string message) {
            Print (sender.GetType ( ), message);
        }

        public static void Print (Type sender, string message) {
#if DEBUG
            global::Android.Util.Log.Debug (sender.FullName, message);
#else
            global::Android.Util.Log.Info (sender.FullName, message);
#endif
        }

        public static void Warn (object sender, string message) {
            Warn (sender.GetType ( ), message);
        }

        public static void Warn (Type sender, string message) {
            global::Android.Util.Log.Warn (sender.FullName, message);
        }
    }
}