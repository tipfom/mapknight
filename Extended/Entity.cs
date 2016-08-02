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
        public static SortedList<Entity, PlatformComponent> Platforms { get; } = new SortedList<Entity, PlatformComponent>( );

        private static void Add (Entity entity) {
            Entities.Add(entity);
            if (entity.HasComponent<PlatformComponent>( ))
                Platforms.Add(entity, entity.GetComponent<PlatformComponent>( ));
        }

        private static int currentInstance { get; set; }
        private static int currentSpecies { get; set; }

        #endregion

        private Component[ ] components;
        private Dictionary<ComponentEnum, Queue<ComponentInfo>> pendingComponentData = new Dictionary<ComponentEnum, Queue<ComponentInfo>>( );
        public IEntityWorld Owner { get; private set; }

        public bool IsOnScreen { get { return Owner.IsOnScreen(this); } }
        public Vector2 PositionOnScreen { get { return Owner.GetPositionOnScreen(this); } }
        public bool IsDestroyed { get; private set; } = false;

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

            this.components = new Component[components.Count];
            for (int i = 0; i < components.Count; i++) {
                this.components[i] = components[i].Create(this);
            }

            Add(this);
        }

        ~Entity ( ) {
            Destroy( );
        }

        public void Destroy ( ) {
            if (IsDestroyed)
                return;
            foreach (Component component in components)
                component.Destroy( );
            Transform = null;
            IsDestroyed = true;
        }

        public void Prepare ( ) {
            foreach (Component component in components)
                component.Prepare( );
        }

        public bool HasComponent<T> ( ) where T : Component {
            Type type = typeof(T);
            return components.Any(c => c.GetType( ) == type);
        }

        public T GetComponent<T> ( ) where T : Component {
            Type type = typeof(T);
            return (T)components.FirstOrDefault(c => c.GetType( ) == type);
        }

        public bool HasComponentInfo (ComponentEnum requester) {
            return pendingComponentData.ContainsKey(requester) && pendingComponentData[requester].Count > 0;
        }

        public ComponentInfo GetComponentInfo (ComponentEnum requester) {
            // not containing needs to be handled with HasComponentInfo
            return pendingComponentData[requester].Dequeue( );
        }

        public void SetComponentInfo (ComponentEnum target, ComponentEnum sender, ComponentData ComponentAction, object data) {
            if (!pendingComponentData.ContainsKey(target))
                pendingComponentData.Add(target, new Queue<ComponentInfo>( ));
            pendingComponentData[target].Enqueue(new ComponentInfo( ) { Action = ComponentAction, Sender = sender, Data = data });
        }

        public void Update (TimeSpan dt) {
            foreach (Component component in components)
                component.Update(dt);
        }

        public void PostUpdate ( ) {
            foreach (Component component in components)
                component.PostUpdate( );
        }

        public void Tick ( ) {
            foreach (Component component in components)
                component.Tick( );
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
