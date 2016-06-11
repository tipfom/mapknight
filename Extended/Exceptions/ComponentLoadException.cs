using mapKnight.Extended.Components.Communication;
using System;

namespace mapKnight.Extended.Exceptions {
    public class ComponentLoadException : TypeLoadException {
        public ComponentLoadException (Identifier type, string errormessage) : base ($"Error while loading {type.ToString ()}Component : {errormessage}") {

        }
    }
}