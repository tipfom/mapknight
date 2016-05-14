using mapKnight.Android.CGL;
using mapKnight.Basic;
using System.Collections.Generic;

namespace mapKnight.Android.Entity {
    public class Entity {
        private Dictionary<ComponentType, Component> components = new Dictionary<ComponentType, Component> ();
        private Dictionary<ComponentType, Stack<ComponentInfo>> pendingComponentData = new Dictionary<ComponentType, Stack<ComponentInfo>> ();
        public IContainer Owner { get; private set; }

        public readonly string Name;
        public readonly int ID;
        public Transform Transform { get; set; }
        public Entity (List<ComponentConfig> components, Transform transform, IContainer owner, string name, int id) {
            Name = name;
            Owner = owner;
            Transform = transform;
            ID = id;

            Component.ResolveDependencies (ref components);
            foreach (ComponentConfig config in components) {
                this.components.Add (config.Type, config.Create (this));
            }

            Owner.Add (this);
        }

        public bool HasComponent (ComponentType component) {
            return components.ContainsKey (component);
        }

        public Component GetComponent (ComponentType component) {
            return components[component];
        }

        public bool HasComponentInfo (ComponentType requester) {
            return pendingComponentData.ContainsKey (requester) && pendingComponentData[requester].Count > 0;
        }

        public ComponentInfo GetComponentInfo (ComponentType requester) {
            // not containing needs to be handled with HasComponentInfo
            return pendingComponentData[requester].Pop ();
        }

        public object GetComponentState (ComponentType ComponentType) {
            if (HasComponent (ComponentType)) {
                return components[ComponentType].State;
            } else
                return null;
        }

        public void SetComponentInfo (ComponentType target, ComponentType sender, ComponentAction ComponentAction, object data) {
            if (!pendingComponentData.ContainsKey (target))
                pendingComponentData.Add (target, new Stack<ComponentInfo> ());
            pendingComponentData[target].Push (new ComponentInfo () { Action = ComponentAction, Sender = sender, Data = data });
        }

        public void Update (float dt) {
            foreach (Component component in components.Values) {
                component.Update (dt);
            }
        }

        public class Config {
            public string Name;
            public Transform Transform;
            private List<ComponentConfig> _Component;
            public List<ComponentConfig> Components { get { return _Component; } set { _Component = value; _Component.Sort (new ComponentComparer ()); } }
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
