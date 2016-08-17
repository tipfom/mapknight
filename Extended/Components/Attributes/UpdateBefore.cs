using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components.Attributes {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateBefore : Attribute {
        public ComponentEnum Relation;

        public UpdateBefore (ComponentEnum relation) {
            Relation = relation;
        }
    }
}