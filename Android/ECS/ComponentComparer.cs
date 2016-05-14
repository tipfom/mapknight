using System.Collections.Generic;

namespace mapKnight.Android.ECS {
    public class ComponentComparer : Comparer<ComponentConfig> {
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