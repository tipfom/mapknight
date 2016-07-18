using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended {
#if DEBUG
    public static class Debug {
        public static void Print (object sender, string message) {
            Print(sender.GetType( ), message);
        }

        public static void Print (Type sender, string message) {
            System.Diagnostics.Debug.WriteLine(sender.Name + ": " + message);
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
#endif
}
