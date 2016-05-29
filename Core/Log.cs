using System;
using System.Diagnostics;

namespace mapKnight.Core {
    public static class Log {
        public static void Print (object sender, string message) {
            Print (sender.GetType ( ), message);
        }

        public static void Print (Type sender, string message) {
            Debug.WriteLine (sender.FullName + message);
        }

        public static void Print (object sender, object message) {
            Print (sender.GetType ( ), message.ToString ( ));
        }

        public static void Print (Type sender, object message) {
            Print (sender, message.ToString ( ));
        }
    }
}
