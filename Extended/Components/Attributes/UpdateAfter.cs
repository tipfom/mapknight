using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components.Attributes {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateAfter : Attribute {
        public readonly ComponentEnum Relation;

        public UpdateAfter (ComponentEnum relation) {
            Relation = relation;
        }
    }
}