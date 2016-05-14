using Newtonsoft.Json;
using System;

namespace mapKnight.Android.Entity {
    class ComponentSerializationBinder : SerializationBinder {
        public override Type BindToType (string assemblyName, string typeName) {
            Type resolvingType = Type.GetType ($"mapKnight.Android.Entity.Components.Configs.{typeName}ComponentConfig");
            return resolvingType;
        }

        public override void BindToName (Type serializedType, out string assemblyName, out string typeName) {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }
}