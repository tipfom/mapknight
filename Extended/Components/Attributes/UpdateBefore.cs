using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components.Attributes {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateBefore : ComponentRequirement {
        public Type Relation;

        public UpdateBefore (Type relation) : base(relation) {
            Relation = relation;
        }
    }
}