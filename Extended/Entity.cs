using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components;

namespace mapKnight.Extended {

    public class Entity {
        public readonly int ID;
        public EntityDomain Domain;
        public readonly int Species;
        private const int TICKS_PER_SECOND = 4;

        #region static

        private static Queue<Entity> destroyedEntitys = new Queue<Entity>( );
        private static int nextTick = Environment.TickCount + timeBetweenTicks;
        private static int timeBetweenTicks = 1000 / TICKS_PER_SECOND;

        private static int currentInstance;
        private static int currentSpecies;
        private static Dictionary<int, string> entityNames = new Dictionary<int, string>( );

        public static List<Entity> Entities = new List<Entity>( );

        public static event Action<Entity> EntityAdded;
        public static event Action EntityRemoved;

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
        private Dictionary<ComponentData, Queue<object[ ]>> pendingComponentInfos = new Dictionary<ComponentData, Queue<object[ ]>>( );

        public Entity (ComponentList components, Transform transform, IEntityWorld world, int species) {
            World = world;
            Transform = transform;
            Species = species;
            ID = ++currentInstance;

            foreach (ComponentData componentDataValue in Enum.GetValues(typeof(ComponentData)))
                pendingComponentInfos.Add(componentDataValue, new Queue<object[ ]>( ));

            this.components = new Component[components.Count];
            for (int i = 0; i < components.Count; i++) {
                this.components[i] = components[i].Create(this);
            }

            for(int i = 0; i < components.Count; i++) {
                this.components[i].Load( );
            }

            Entities.Add(this);
            EntityAdded?.Invoke(this);
        }

        ~Entity ( ) {
            Destroy( );
        }

        public event Action Destroyed;

        public bool IsDestroyed { get; private set; } = false;
        public bool IsOnScreen { get { return World.IsOnScreen(this); } }
        public Vector2 PositionOnScreen { get { return World.GetPositionOnScreen(this); } }
        public Transform Transform { get; set; }
        public IEntityWorld World { get; private set; }
        public string Name { get { return entityNames[Species]; } }

        public bool HasComponentInfo (ComponentData data) {
            return pendingComponentInfos[data].Count > 0;
        }

        public object[ ] GetComponentInfo (ComponentData data) {
            // not containing needs to be handled with HasComponentInfo
            return pendingComponentInfos[data].Dequeue( );
        }

        public void SetComponentInfo (ComponentData target, params object[ ] data) {
            pendingComponentInfos[target].Enqueue(data);
        }

        public bool HasComponent<T> ( ) where T : Component {
            Type type = typeof(T);
            return components.Any(c => c.GetType( ) == type);
        }

        public T GetComponent<T> ( ) where T : Component {
            Type type = typeof(T);
            return (T)components.FirstOrDefault(c => c.GetType( ) == type);
        }
        public void Collision (Entity collidingEntity) {
            for (int i = 0; i < components.Length; i++)
                components[i].Collision(collidingEntity);
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

        public void PostUpdate ( ) {
            for (int i = 0; i < components.Length; i++)
                components[i].PostUpdate( );
        }

        public void Prepare ( ) {
            foreach (Component component in components)
                component.Prepare( );
        }

        public void Tick ( ) {
            for (int i = 0; i < components.Length; i++)
                components[i].Tick( );
        }

        public void Update (DeltaTime dt) {
            for (int i = 0; i < components.Length; i++)
                components[i].Update(dt);
        }

        public override string ToString ( ) {
            return Name;
        }

        public override int GetHashCode ( ) {
            return ID;
        }

        public class Configuration {
            public ComponentList Components;
            public string Name;
            public Transform Transform;
            public int Species = -1;

            public Configuration ( ) {
            }

            public Configuration(string name, Vector2 size) {
                Name = name;
                Transform = new Transform(default(Vector2), size);
                Components = new ComponentList( );
            }

            public Entity Create (Vector2 spawnLocation, IEntityWorld world) {
                if (Species == -1 || Components.HasChanged) {
                    Species = ++currentSpecies;
                    entityNames.Add(Species, Name);
                    Components.ResolveComponentDependencies( );
                    Components.Sort( );
                }
                return new Entity(Components, new Transform(spawnLocation, Transform.Size), world, Species);
            }
        }
    }
}