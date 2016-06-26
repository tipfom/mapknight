using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Extended.Components.Communication;
using mapKnight.Extended.Exceptions;
using Newtonsoft.Json;

namespace mapKnight.Extended {
    public abstract class Component {
        protected Entity Owner;

        public object State;

        public Component (Entity owner) {
            this.Owner = owner;
            this.State = null;
        }

        public virtual void Update (float dt) {

        }

        public virtual void Prepare ( ) {

        }

        public override string ToString ( ) {
            return this.GetType( ).Name;
        }

        public static Type GetComponentConfigType (Identifier type) {
            return Type.GetType($"mapKnight.Extended.Components.Configs.{type.ToString( )}ComponentConfig");
        }

        public static Type GetIdentifier (Identifier type) {
            return Type.GetType($"mapKnight.Extended.Components.{type.ToString( )}Component");
        }

        // use inheritance (Animation needs Skelet and Draw, Skelet needs Draw, so Animation only needs Skelet)
        private static Dictionary<Identifier, Identifier> Dependencies = new Dictionary<Identifier, Identifier>( ) {
            [Identifier.Animation] = Identifier.Skelet,
            [Identifier.Gravity] = Identifier.Motion,
            [Identifier.Push] = Identifier.Motion,
            [Identifier.Skelet] = Identifier.Draw,
            [Identifier.Sprite] = Identifier.Draw,
            [Identifier.Texture] = Identifier.Draw,
            [Identifier.Speed] = Identifier.Motion,
            [Identifier.UserControl] = Identifier.Speed
        };

        public static void ResolveDependencies (ref List<ComponentConfig> componentConfigs) {
            IEnumerable<Identifier> Identifiers = componentConfigs.ToArray( ).Select(componentConfig => componentConfig.Type);
            foreach (Identifier Identifier in Identifiers) {
                if (!Dependencies.ContainsKey(Identifier))
                    continue;

                Identifier dependency = Dependencies[Identifier];

                if (!componentConfigs.Exists(componentConfig => componentConfig.Type == dependency)) {
                    // dependency doesnt exist allready
                    bool canBeCreated = GetIdentifier(dependency).GetConstructor(new[ ] { typeof(Entity) }) != null;
                    // ConstructorInfo dependencyConstructor = dependencyType.GetConstructor (new[] { typeof (Entity) });
                    // check if dependency component could be initialized with an entity only
                    if (!canBeCreated)
                        throw new ComponentDependencyException(Identifier, dependency);
                    else
                        componentConfigs.Add((ComponentConfig)Activator.CreateInstance(GetComponentConfigType(dependency)));
                }
            }
        }

        public static SerializationBinder SerializationBinder { get; } = new ComponentSerializationBinder( );

        private class ComponentSerializationBinder : SerializationBinder {
            public override Type BindToType (string assemblyName, string typeName) {
                Type resolvingType = Type.GetType($"mapKnight.Extended.Components.Configs.{typeName}ComponentConfig");
                return resolvingType;
            }

            public override void BindToName (Type serializedType, out string assemblyName, out string typeName) {
                assemblyName = null;
                typeName = serializedType.Name;
            }
        }
    }
}