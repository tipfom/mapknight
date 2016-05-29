using System;

namespace mapKnight.Core.Exceptions {
    public class MissingMapIdentifierException : Exception {
        public MissingMapIdentifierException () : base ("the map-identifier was missing in the file", new MapLoadException ( )) {

        }
    }
}
