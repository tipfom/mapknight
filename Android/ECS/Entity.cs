using mapKnight.Basic;
using System.Collections.Generic;

namespace mapKnight.Android.ECS {
    public class Entity {
        private Dictionary<ComponentType, Component> components = new Dictionary<ComponentType, Component> ();
        private Dictionary<ComponentType, Queue<ComponentInfo>> pendingComponentData = new Dictionary<ComponentType, Queue<ComponentInfo>> ();
        public IEntityContainer Owner { get; private set; }

        public bool IsOnScreen { get { return Owner.IsOnScreen (this); } }
        public Vector2 PositionOnScreen { get { return Owner.GetPositionOnScreen (this); } }

        public readonly string Name;
        public readonly int ID;
        public Transform Transform { get; set; }
        public Entity (List<ComponentConfig> components, Transform transform, IEntityContainer owner, string name, int id) {
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

        public void Prepare () {
            foreach (Component component in components.Values) {
                component.Prepare ();
            }
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
            return pendingComponentData[requester].Dequeue ();
        }

        public object GetComponentState (ComponentType ComponentType) {
            if (HasComponent (ComponentType)) {
                return components[ComponentType].State;
            } else
                return null;
        }

        public void SetComponentInfo (ComponentType target, ComponentType sender, ComponentAction ComponentAction, object data) {
            if (!pendingComponentData.ContainsKey (target))
                pendingComponentData.Add (target, new Queue<ComponentInfo> ());
            pendingComponentData[target].Enqueue (new ComponentInfo () { Action = ComponentAction, Sender = sender, Data = data });
        }

        public void Update (float dt) {
            foreach (Component component in components.Values) {
                component.Update (dt);
            }
        }
    }
}
