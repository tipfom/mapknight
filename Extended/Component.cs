using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components;
using mapKnight.Extended.Exceptions;
using Newtonsoft.Json;

namespace mapKnight.Extended {
    public abstract class Component {
        protected Entity Owner;

        public Component (Entity owner) {
            this.Owner = owner;
        }

        ~Component ( ) {
            Destroy( );
        }

        public virtual void Destroy ( ) {

        }

        public virtual void Update (TimeSpan dt) {

        }

        public virtual void Tick ( ) {

        }

        public virtual void PostUpdate ( ) {

        }

        public virtual void Prepare ( ) {

        }

        public override string ToString ( ) {
            return this.GetType( ).Name;
        }

        public static SerializationBinder SerializationBinder { get; } = new ComponentSerializationBinder( );

        private class ComponentSerializationBinder : SerializationBinder {
            public override Type BindToType (string assemblyName, string typeName) {
                return Type.GetType($"mapKnight.Extended.Components.{ typeName }Component+Configuration");
            }

            public override void BindToName (Type serializedType, out string assemblyName, out string typeName) {
                assemblyName = null;
                typeName = serializedType.Name;
            }
        }

        public abstract class Configuration {
            public ComponentEnum Component { get; }

            public abstract Component Create (Entity owner);

            public Configuration ( ) {
                Component = (ComponentEnum)Enum.Parse(typeof(ComponentEnum), this.GetType( ).FullName.Substring(30).Replace("Component+Configuration", "").Replace(".", "_"));
            }
        }
    }
}