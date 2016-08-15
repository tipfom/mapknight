using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components;
using mapKnight.Extended.Exceptions;
using Newtonsoft.Json;

namespace mapKnight.Extended {
    public class Entity {
        const int TICKS_PER_SECOND = 4;

        #region static 

        public static event Action EntitiesChanged;
        public static List<Entity> Entities { get; } = new List<Entity>( );
        private static Queue<Entity> destroyedEntitys = new Queue<Entity>( );

        private static void CalculateCollisions ( ) {
            int outerLoopsBounds = Entities.Count - 1;
            for (int i = 0; i < outerLoopsBounds; i++) {
                for (int l = i + 1; l < Entities.Count; l++) {
                    if (Entities[i].Transform.Intersects(Entities[l].Transform)) {
                        Entities[i].Collision(Entities[l]);
                        Entities[l].Collision(Entities[i]);
                    }
                }
            }
        }

        private static int timeBetweenTicks = 1000 / TICKS_PER_SECOND;
        private static int nextTick = Environment.TickCount + timeBetweenTicks;
        public static void UpdateAll (DeltaTime dt) {
            while (destroyedEntitys.Count > 0) {
                Entities.Remove(destroyedEntitys.Dequeue( ));
                EntitiesChanged?.Invoke( );
            }

            if (Environment.TickCount > nextTick) {
                nextTick += timeBetweenTicks;
                for (int i = 0; i < Entities.Count; i++)
                    Entities[i].Tick( );
            }

            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Update(dt);

            CalculateCollisions( );
        }

        public static void PostUpdateAll ( ) {
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].PostUpdate( );
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

        public readonly EntityInfo Info;

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

            // set entity informations
            Info = new EntityInfo( ) {
                IsPlatform = components.Any(c => c.Component == ComponentEnum.Platform),
                IsPlayer = components.Any(c => c.Component == ComponentEnum.Player),

                HasArmor = components.Any(c => c.Component == ComponentEnum.Stats_Armor),
                HasDamage = components.Any(c => c.Component == ComponentEnum.Stats_Damage),
                HasHealth = components.Any(c => c.Component == ComponentEnum.Stats_Health)
            };
            Entities.Add(this);
            EntitiesChanged?.Invoke( );
        }

        ~Entity ( ) {
            Destroy( );
        }

        public void Destroy ( ) {
            if (IsDestroyed)
                return;
            foreach (Component component in components)
                component.Destroy( );
            IsDestroyed = true;
            destroyedEntitys.Enqueue(this);
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

        public void Update (DeltaTime dt) {
            for (int i = 0; i < components.Length; i++)
                components[i].Update(dt);
        }

        public void PostUpdate ( ) {
            for (int i = 0; i < components.Length; i++)
                components[i].PostUpdate( );
        }

        public void Tick ( ) {
            for (int i = 0; i < components.Length; i++)
                components[i].Tick( );
        }

        public void Collision (Entity collidingEntity) {
            for (int i = 0; i < components.Length; i++)
                components[i].Collision(collidingEntity);
            //Debug.Print(this, $"{Name}({ID}) colliding with {collidingEntity.Name}({collidingEntity.ID})");
        }

        public struct EntityInfo {
            public bool IsPlatform;
            public bool IsPlayer;

            // Stats
            public bool HasHealth;
            public bool HasArmor;
            public bool HasDamage;
        }

        public class Configuration {
            public string Name;
            public Transform Transform;
            public ComponentList Components;
            private int entitySpecies = -1;

            public Entity Create (Vector2 spawnLocation, IEntityWorld container) {
                if (entitySpecies == -1 || Components.HasChanged) {
                    entitySpecies = ++currentSpecies;
                    Components.ResolveComponentDependencies( );
                }
                return new Entity(Components, new Transform(spawnLocation, Transform.Bounds), container, Name, entitySpecies);
            }
        }
    }
}
