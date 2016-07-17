using System;
using System.Diagnostics;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended {
    public static class Log {
        public static void Print (object sender, string message) {
            Print(sender.GetType( ), message);
        }

        public static void Print (Type sender, string message) {
            Debug.WriteLine(sender.Name + ": " + message);
        }

        public static void Print (object sender, object message) {
            Print(sender.GetType( ), message.ToString( ));
        }

        public static void Print (Type sender, object message) {
            Print(sender, message.ToString( ));
        }

        public static void CheckGL (object sender) {
            CheckGL(sender.GetType( ));
        }

        public static void CheckGL (Type sender) {
            ErrorCode error = GL.GetErrorCode( );
            if (error != ErrorCode.NoError) {
                Print(sender, $"OPENGLERROR {error}");
            }
        }
    }
}
