using mapKnight.Core.Components.Communication;
using System;

namespace mapKnight.Core.Exceptions {
    public class ComponentLoadException : TypeLoadException {
        public ComponentLoadException (Identifier type, string errormessage) : base ($"Error while loading {type.ToString ()}Component : {errormessage}") {

        }
    }
}