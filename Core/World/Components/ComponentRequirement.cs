using System;

namespace mapKnight.Core.World.Components {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ComponentRequirement : Attribute {

        public ComponentRequirement (Type t) {
            if (t.IsSubclassOf(typeof(Component))) {
                Requiring = t;
            } else {
                throw new InvalidOperationException($"requirement ({t.FullName}) was not a subclass of component");
            }
        }

        public Type Requiring { get; private set; }
    }
}