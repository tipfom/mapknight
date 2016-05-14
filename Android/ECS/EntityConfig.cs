using mapKnight.Basic;
using System.Collections.Generic;

namespace mapKnight.Android.ECS {
    public class EntityConfig {
        public string Name;
        public Transform Transform;
        private List<ComponentConfig> _Component;
        public List<ComponentConfig> Components { get { return _Component; } set { _Component = value; _Component.Sort (new ComponentComparer ()); } }
        private int entityID = -1;

        public Entity Create (Vector2 spawnLocation, IEntityContainer container) {
            if (entityID == -1)
                entityID = container.CreateID ();
            return new Entity (Components, new Transform (spawnLocation, Transform.Bounds), container, Name, entityID);
        }
    }
}