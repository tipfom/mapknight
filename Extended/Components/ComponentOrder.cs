using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ComponentOrder : Attribute {
        public ComponentEnum Relation { get; }
        public bool Before { get; }

        public ComponentOrder (ComponentEnum relation, bool before = true) {
            Relation = relation;
            Before = before;
        }
    }
}
