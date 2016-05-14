using System;

namespace mapKnight.Android.ECS {
    public abstract class ComponentConfig {
        public ComponentType Type { get { return (ComponentType)Enum.Parse (typeof (ComponentType), this.GetType ().Name.Replace ("ComponentConfig", "")); } }
        public abstract Component Create (Entity owner);
        // used for sorting
        // -2 is last called, 2 is first
        public abstract int Priority { get; }
    }
}