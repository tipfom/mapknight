using System;

namespace mapKnight.Extended.Components.Attributes {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UpdateBefore : ComponentRequirement {
        public readonly Type Relation;
        public readonly bool Requirement;

        public UpdateBefore (Type relation, bool requirement = true) : base(relation) {
            Relation = relation;
            Requirement = requirement;
        }
    }
}