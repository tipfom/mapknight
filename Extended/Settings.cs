using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended {
    public class Settings {
        public static Settings Singleton = new Settings( );

        public bool UseAdvancedLightning = true;

        public Settings ( ) {

        }
    }
}
