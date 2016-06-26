using mapKnight.Core;
using mapKnight.Extended.Components.Communication;
using System.Collections.Generic;

namespace mapKnight.Extended {
    public class Entity {
        private Dictionary<Identifier, Component> components = new Dictionary<Identifier, Component> ();
        private Dictionary<Identifier, Queue<Info>> pendingComponentData = new Dictionary<Identifier, Queue<Info>> ();
        public IEntityContainer Owner { get; private set; }

        public bool IsOnScreen { get { return Owner.IsOnScreen (this); } }
        public Vector2 PositionOnScreen { get { return Owner.GetPositionOnScreen (this); } }

        public readonly string Name;
        public readonly int Species;
        public readonly int ID;

        public Transform Transform { get; set; }
        public Entity (List<ComponentConfig> components, Transform transform, IEntityContainer owner, string name, int species) {
            Name = name;
            Owner = owner;
            Transform = transform;
            Species = species;
            ID = owner.NewInstance( );

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

        public bool HasComponent (Identifier component) {
            return components.ContainsKey (component);
        }

        public Component GetComponent (Identifier component) {
            return components[component];
        }

        public bool HasComponentInfo (Identifier requester) {
            return pendingComponentData.ContainsKey (requester) && pendingComponentData[requester].Count > 0;
        }

        public Info GetComponentInfo (Identifier requester) {
            // not containing needs to be handled with HasComponentInfo
            return pendingComponentData[requester].Dequeue ();
        }

        public object GetComponentState (Identifier Identifier) {
            if (HasComponent (Identifier)) {
                return components[Identifier].State;
            } else
                return null;
        }

        public void SetComponentInfo (Identifier target, Identifier sender, Data ComponentAction, object data) {
            if (!pendingComponentData.ContainsKey (target))
                pendingComponentData.Add (target, new Queue<Info> ());
            pendingComponentData[target].Enqueue (new Info () { Action = ComponentAction, Sender = sender, Data = data });
        }

        public void Update (float dt) {
            foreach (Component component in components.Values) {
                component.Update (dt);
            }
        }
    }
}
