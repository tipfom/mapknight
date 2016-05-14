using System;

namespace mapKnight.Android.ECS {
    public class ComponentLoadException : TypeLoadException {
        public ComponentLoadException (ComponentType type, string errormessage) : base ($"Error while loading {type.ToString ()}Component : {errormessage}") {

        }
    }
}