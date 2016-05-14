using mapKnight.Android.CGL;
using mapKnight.Basic;
using System.Collections.Generic;

namespace mapKnight.Android.Entity {
    public class Entity {
        private Dictionary<Component.Type, Component> components = new Dictionary<Component.Type, Component> ();
        private Dictionary<Component.Type, Stack<Component.Info>> pendingComponentData = new Dictionary<Component.Type, Stack<Component.Info>> ();
        public IContainer Owner { get; private set; }

        public readonly string Name;
        public readonly int ID;
        public Transform Transform { get; set; }
        public Entity (List<Component.Config> components, Transform transform, IContainer owner, string name, int id) {
            Name = name;
            Owner = owner;
            Transform = transform;
            ID = id;

            Component.ResolveDependencies (ref components);
            foreach (Component.Config config in components) {
                this.components.Add (config.Type, config.Create (this));
            }

            Owner.Add (this);
        }

        public bool HasComponent (Component.Type component) {
            return components.ContainsKey (component);
        }

        public Component GetComponent (Component.Type component) {
            return components[component];
        }

        public bool HasComponentInfo (Component.Type requester) {
            return pendingComponentData.ContainsKey (requester) && pendingComponentData[requester].Count > 0;
        }

        public Component.Info GetComponentInfo (Component.Type requester) {
            // not containing needs to be handled with HasComponentInfo
            return pendingComponentData[requester].Pop ();
        }

        public object GetComponentState (Component.Type type) {
            if (HasComponent (type)) {
                return components[type].State;
            } else
                return null;
        }

        public void SetComponentInfo (Component.Type target, Component.Type sender, Component.Action action, object data) {
            if (!pendingComponentData.ContainsKey (target))
                pendingComponentData.Add (target, new Stack<Component.Info> ());
            pendingComponentData[target].Push (new Component.Info () { Action = action, Sender = sender, Data = data });
        }

        public void Update (float dt) {
            foreach (Component component in components.Values) {
                component.Update (dt);
            }
        }

        public class Config {
            public string Name;
            public Transform Transform;
            private List<Component.Config> _Component;
            public List<Component.Config> Components { get { return _Component; } set { _Component = value; _Component.Sort (new Component.Comparer ()); } }
            private int entityID = -1;

            public Entity Create (Vector2 spawnLocation, Entity.IContainer container) {
                if (entityID == -1)
                    entityID = container.CreateID ();
                return new Entity (Components, new Transform (spawnLocation, Transform.Bounds), container, Name, entityID);
            }
        }

        public struct VertexData {
            public List<string> SpriteNames;
            public float[] VertexCoords;
            public int QuadCount;
            public int Entity;
        }

        public interface IRenderer {
            void QueueVertexData (VertexData vertexData);
            void AddTexture (int entityID, CGLSprite2D entityTexture);
        }

        public interface IContainer {
            Vector2 Gravity { get; }
            Vector2 Bounds { get; }
            bool HasCollider (int x, int y);
            int CreateID ();
            IRenderer Renderer { get; }
            CGLCamera Camera { get; }
            void Add (Entity entity);
            float VertexSize { get; }
            bool IsOnScreen (Entity entity);
        }
    }
}
