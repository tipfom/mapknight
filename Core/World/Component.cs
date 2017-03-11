using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Core.World
{
    public abstract class Component
    {
        public static SerializationBinder SerializationBinder { get; } = new ComponentSerializationBinder( );

        public readonly Entity Owner;

        public Component(Entity owner) {
            this.Owner = owner;
        }

        ~Component( ) {
            Destroy( );
        }

        public virtual void Collision(Entity collidingEntity) {
        }

        public virtual void Destroy( ) {
        }

        public virtual void Draw( ) {
        }

        public virtual void Prepare( ) {
        }

        public virtual void Tick( ) {
        }

        public virtual void Load(Configuration config) {
        }

        public virtual void Update(DeltaTime dt) {
        }

        public override string ToString( ) {
            return GetType( ).Name;
        }

        public abstract class Configuration {
            public abstract Component Create(Entity owner);
        }

        private class ComponentSerializationBinder : SerializationBinder {

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName) {
                assemblyName = null;
                typeName = serializedType.Name;
            }

            public override Type BindToType(string assemblyName, string typeName) {
                return Type.GetType($"mapKnight.Extended.Components.{ typeName }Component+Configuration");
            }
        }
    }
}
