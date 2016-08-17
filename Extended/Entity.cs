﻿using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components;
using mapKnight.Extended.Exceptions;
using Newtonsoft.Json;

namespace mapKnight.Extended {

    public class Entity {
        public readonly int ID;
        public readonly EntityInfo Info;
        public readonly string Name;
        public readonly int Species;
        private const int TICKS_PER_SECOND = 4;

        #region static

        private static Queue<Entity> destroyedEntitys = new Queue<Entity>( );

        private static int nextTick = Environment.TickCount + timeBetweenTicks;

        private static int timeBetweenTicks = 1000 / TICKS_PER_SECOND;

        public static event Action<Entity> EntityAdded;

        public static event Action EntityRemoved;

        public static List<Entity> Entities { get; } = new List<Entity>( );
        private static int currentInstance { get; set; }

        private static int currentSpecies { get; set; }

        public static void PostUpdateAll ( ) {
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].PostUpdate( );
        }

        public static void UpdateAll (DeltaTime dt) {
            while (destroyedEntitys.Count > 0) {
                Entities.Remove(destroyedEntitys.Dequeue( ));
                EntityRemoved?.Invoke( );
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

        #endregion static

        private Component[ ] components;
        private Dictionary<ComponentEnum, Queue<object>> pendingComponentInfos = new Dictionary<ComponentEnum, Queue<object>>( );

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
                IsTemporary = components.Any(c => c.Component == ComponentEnum.AI_Trigger_InternalTrigger),

                HasArmor = components.Any(c => c.Component == ComponentEnum.Stats_Armor),
                HasDamage = components.Any(c => c.Component == ComponentEnum.Stats_Damage),
                HasHealth = components.Any(c => c.Component == ComponentEnum.Stats_Health)
            };
            Entities.Add(this);
            EntityAdded?.Invoke(this);
        }

        ~Entity ( ) {
            Destroy( );
        }

        public event Action Destroyed;

        public bool IsDestroyed { get; private set; } = false;
        public bool IsOnScreen { get { return Owner.IsOnScreen(this); } }
        public IEntityWorld Owner { get; private set; }
        public Vector2 PositionOnScreen { get { return Owner.GetPositionOnScreen(this); } }
        public Transform Transform { get; set; }

        public void Collision (Entity collidingEntity) {
            for (int i = 0; i < components.Length; i++)
                components[i].Collision(collidingEntity);
            //Debug.Print(this, $"{Name}({ID}) colliding with {collidingEntity.Name}({collidingEntity.ID})");
        }

        public void Destroy ( ) {
            if (IsDestroyed)
                return;
            foreach (Component component in components)
                component.Destroy( );
            IsDestroyed = true;
            destroyedEntitys.Enqueue(this);
            Destroyed?.Invoke( );
        }

        public T GetComponent<T> ( ) where T : Component {
            Type type = typeof(T);
            return (T)components.FirstOrDefault(c => c.GetType( ) == type);
        }

        public object GetComponentInfo (ComponentEnum requester) {
            // not containing needs to be handled with HasComponentInfo
            return pendingComponentInfos[requester].Dequeue( );
        }

        public bool HasComponent<T> ( ) where T : Component {
            Type type = typeof(T);
            return components.Any(c => c.GetType( ) == type);
        }

        public bool HasComponentInfo (ComponentEnum requester) {
            return pendingComponentInfos.ContainsKey(requester) && pendingComponentInfos[requester].Count > 0;
        }

        public void PostUpdate ( ) {
            for (int i = 0; i < components.Length; i++)
                components[i].PostUpdate( );
        }

        public void Prepare ( ) {
            foreach (Component component in components)
                component.Prepare( );
        }

        public void SetComponentInfo (ComponentEnum target, object data) {
            if (!pendingComponentInfos.ContainsKey(target))
                pendingComponentInfos.Add(target, new Queue<object>( ));
            pendingComponentInfos[target].Enqueue(data);
        }

        public void Tick ( ) {
            for (int i = 0; i < components.Length; i++)
                components[i].Tick( );
        }

        public void Update (DeltaTime dt) {
            for (int i = 0; i < components.Length; i++)
                components[i].Update(dt);
        }

        public struct EntityInfo {
            public bool HasArmor;
            public bool HasDamage;

            // Stats
            public bool HasHealth;

            public bool IsPlatform;
            public bool IsPlayer;
            public bool IsTemporary;
        }

        public class Configuration {
            public ComponentList Components;
            public string Name;
            public Transform Transform;
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