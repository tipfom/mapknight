using System;

namespace mapKnight.Core.World.Components {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Instantiatable : Attribute {
    }
}