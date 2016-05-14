using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace mapKnight.Android.ECS {
    public abstract class Component {
        protected Entity Owner;

        public object State;

        public Component (Entity owner) {
            this.Owner = owner;
            this.State = null;
        }

        public virtual void Update (float dt) {

        }

        public virtual void Prepare () {

        }

        public override string ToString () {
            return this.GetType ().Name;
        }

        public static Type GetComponentConfigType (ComponentType type) {
            return Type.GetType ($"mapKnight.Android.ECS.Components.Configs.{type.ToString ()}ComponentConfig");
        }

        public static Type GetComponentType (ComponentType type) {
            return Type.GetType ($"mapKnight.Android.ECS.Components.{type.ToString ()}Component");
        }

        // use inheritance (Animation needs Skelet and Draw, Skelet needs Draw, so Animation only needs Skelet)
        private static Dictionary<ComponentType, ComponentType> Dependencies = new Dictionary<ComponentType, ComponentType> () {
            [ComponentType.Animation] = ComponentType.Skelet,
            [ComponentType.Gravity] = ComponentType.Motion,
            [ComponentType.Push] = ComponentType.Motion,
            [ComponentType.Skelet] = ComponentType.Draw,
            [ComponentType.Sprite] = ComponentType.Draw,
            [ComponentType.Texture] = ComponentType.Draw
        };

        public static void ResolveDependencies (ref List<ComponentConfig> componentConfigs) {
            IEnumerable<ComponentType> componentTypes = componentConfigs.ToArray ().Select (componentConfig => componentConfig.Type);
            foreach (ComponentType componentType in componentTypes) {
                if (!Dependencies.ContainsKey (componentType))
                    continue;

                ComponentType dependency = Dependencies[componentType];

                if (!componentConfigs.Exists (componentConfig => componentConfig.Type == dependency)) {
                    // dependency doesnt exist allready
                    Type dependencyType = GetComponentType (dependency);
                    ConstructorInfo dependencyConstructor = dependencyType.GetConstructor (new[] { typeof (Entity) });
                    // check if dependency component could be initialized with an entity only
                    if (dependencyConstructor == null)
                        throw new ComponentDependencyException (componentType, dependency);
                    else
                        componentConfigs.Add ((ComponentConfig)Activator.CreateInstance (GetComponentConfigType (dependency)));

                }
            }
        }
    }
}