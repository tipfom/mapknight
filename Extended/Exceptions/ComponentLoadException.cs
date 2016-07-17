using mapKnight.Extended.Components;
using System;

namespace mapKnight.Extended.Exceptions {
    public class ComponentLoadException : TypeLoadException {
        public ComponentLoadException (ComponentEnum type, string errormessage) : base($"Error while loading {type.ToString( )}Component : {errormessage}") {

        }
    }
}