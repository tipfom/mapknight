using mapKnight.Extended.Components.Communication;
using System;
using System.Collections.Generic;

namespace mapKnight.Extended {
    public abstract class ComponentConfig {
        public Identifier Type { get { return (Identifier)Enum.Parse (typeof (Identifier), this.GetType ().Name.Replace ("ComponentConfig", "")); } }
        public abstract Component Create (Entity owner);
        // used for sorting
        // -2 is last called, 2 is first
        public abstract int Priority { get; }

        public static Comparer<ComponentConfig> Comparer { get; } = new ComponentConfigComparer( );

        private class ComponentConfigComparer : Comparer<ComponentConfig> {
            public override int Compare (ComponentConfig x, ComponentConfig y) {
                if (x.Priority > y.Priority) {
                    return -1; // the ones with the higher priority need to be called first
                } else if (x.Priority < y.Priority) {
                    return 1;
                } else {
                    return 0;
                }
            }
        }
    }
}