using mapKnight.Core.World.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Core.World
{
    public abstract class Component
    {
#if __ANDROID__
        public static SerializationBinder SerializationBinder { get; } = new ComponentSerializationBinder( );
#endif

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

        public virtual void Load(Dictionary<DataID, object> data) {
        }

        public virtual void Update(DeltaTime dt) {
        }

        public override string ToString( ) {
            return GetType( ).Name;
        }

        public abstract class Configuration {
            public abstract Component Create(Entity owner);
        }

#if __ANDROID__
        private class ComponentSerializationBinder : SerializationBinder {

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName) {
                assemblyName = null;
                typeName = serializedType.Name;
            }

            public override Type BindToType(string assemblyName, string typeName) {
                return Type.GetType($"mapKnight.Extended.Components.{ typeName }Component+Configuration");
            }
        }
#endif
    }
}
