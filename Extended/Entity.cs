using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components;
using mapKnight.Extended.Exceptions;
using Newtonsoft.Json;

namespace mapKnight.Extended {
    public class Entity {
        #region static 

        public static List<Entity> Entities { get; } = new List<Entity>( );
        public static List<Entity> Platforms { get; } = new List<Entity>( );

        private static void Add (Entity entity) {
            Entities.Add(entity);
            if (entity.HasComponent(ComponentEnum.Platform))
                Platforms.Add(entity);
        }

        private static int currentInstance { get; set; }
        private static int currentSpecies { get; set; }

        #endregion

        private Dictionary<ComponentEnum, Component> components = new Dictionary<ComponentEnum, Component>( );
        private Dictionary<ComponentEnum, Queue<ComponentInfo>> pendingComponentData = new Dictionary<ComponentEnum, Queue<ComponentInfo>>( );
        public IEntityWorld Owner { get; private set; }

        public bool IsOnScreen { get { return Owner.IsOnScreen(this); } }
        public Vector2 PositionOnScreen { get { return Owner.GetPositionOnScreen(this); } }

        public readonly string Name;
        public readonly int Species;
        public readonly int ID;

        public Transform Transform { get; set; }
        public Entity (ComponentList components, Transform transform, IEntityWorld owner, string name, int species) {
            Name = name;
            Owner = owner;
            Transform = transform;
            Species = species;
            ID = ++currentInstance;

            foreach (Component.Configuration config in components) {
                this.components.Add(config.Component, config.Create(this));
            }

            Add(this);
        }

        public void Prepare ( ) {
            foreach (Component component in components.Values) {
                component.Prepare( );
            }
        }

        public bool HasComponent (ComponentEnum component) {
            return components.ContainsKey(component);
        }

        public Component GetComponent (ComponentEnum component) {
            return components[component];
        }

        public bool HasComponentInfo (ComponentEnum requester) {
            return pendingComponentData.ContainsKey(requester) && pendingComponentData[requester].Count > 0;
        }

        public ComponentInfo GetComponentInfo (ComponentEnum requester) {
            // not containing needs to be handled with HasComponentInfo
            return pendingComponentData[requester].Dequeue( );
        }

        public object GetComponentState (ComponentEnum id) {
            if (HasComponent(id)) {
                return components[id].State;
            } else
                return null;
        }

        public void SetComponentInfo (ComponentEnum target, ComponentEnum sender, ComponentData ComponentAction, object data) {
            if (!pendingComponentData.ContainsKey(target))
                pendingComponentData.Add(target, new Queue<ComponentInfo>( ));
            pendingComponentData[target].Enqueue(new ComponentInfo( ) { Action = ComponentAction, Sender = sender, Data = data });
        }

        public void Update (TimeSpan dt) {
            foreach (Component component in components.Values) {
                component.Update(dt);
            }
        }

        public void PostUpdate ( ) {
            foreach (Component component in components.Values) {
                component.PostUpdate( );
            }
        }

        public class Configuration {
            public string Name;
            public Transform Transform;
            public ComponentList Components;
            private int entitySpecies = -1;

            public Entity Create (Vector2 spawnLocation, IEntityWorld container) {
                if (entitySpecies == -1 || Components.HasChanged) {
                    entitySpecies = ++currentSpecies;
                    ResolveComponentDependencies( );
                }
                return new Entity(Components, new Transform(spawnLocation, Transform.Bounds), container, Name, entitySpecies);
            }

            private void ResolveComponentDependencies ( ) {
                HashSet<Type> instanciatedTypes = new HashSet<Type>(Components.Select(config => config.GetType( )));
                for (int i = 0; i < Components.Count; i++) {
                    Type componentType = Type.GetType(Components[i].GetType( ).FullName.Replace("+Configuration", ""));
                    ComponentRequirement[ ] requirements = (ComponentRequirement[ ])componentType.GetCustomAttributes(typeof(ComponentRequirement), false);
                    foreach (ComponentRequirement requirement in requirements) {
                        Type componentConfigType = Type.GetType(requirement.Requiring.FullName + "+Configuration");
                        if (!instanciatedTypes.Contains(componentConfigType)) {
                            bool canBeInstanciated = requirement.Requiring.GetConstructor(new Type[ ] { typeof(Entity) }) != null;
                            if (canBeInstanciated) {
                                Components.Add((Component.Configuration)Activator.CreateInstance(componentConfigType));
                                instanciatedTypes.Add(componentConfigType);
                            } else
                                throw new ComponentDependencyException(ComponentEnum.Animation, requirement.Requiring);
                        }
                    }
                }
            }
        }
    }
}
