using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ComponentRequirement : Attribute {
        public Type Requiring { get; private set; }

        public ComponentRequirement (Type t) {
            if (t.IsSubclassOf(typeof(Component))) {
                Requiring = t;
            } else {
                throw new InvalidOperationException($"requirement ({t.FullName}) was not a subclass of component");
            }
        }
    }
}
