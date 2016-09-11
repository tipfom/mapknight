using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Components.Attributes {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateAfter : ComponentRequirement {
        public readonly Type Relation;

        public UpdateAfter (Type relation) : base(relation) {
            Relation = relation;
        }
    }
}