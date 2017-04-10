using System.Collections.Generic;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Core.World.Serialization;

namespace mapKnight.Extended.Components.AI {
    public class NPCComponent : Component {
        private bool _Available = true;
        public bool Available {
            get {
                if (!_Available) {
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "deny", true);
                    Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle_none", false);
                }
                return _Available;
            }
        }

        private string[ ] messages;
        private int currentIndex = -1;

        public NPCComponent (Entity owner) : base(owner) {
            owner.Domain = EntityDomain.NPC;
        }

        public override void Load(Dictionary<DataID, object> data) {
            messages = (string[ ])data[DataID.NPC_Messages];
        }

        public override void Collision (Entity collidingEntity) {
            if (collidingEntity.Domain == EntityDomain.Player && _Available) {
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle_active", true);
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle_inactive", false);
            }
        }

        public string NextMessage ( ) {
            currentIndex++;
            if (currentIndex == messages.Length) {
                currentIndex = -1;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle_none", true);
                return null;
            } else {
                _Available = false;
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "talk", true);
                Owner.SetComponentInfo(ComponentData.SpriteAnimation, "idle_none", false);
                return messages[currentIndex];
            }
        }

        public new class Configuration : Component.Configuration {
            public override Component Create (Entity owner) {
                return new NPCComponent(owner);
            }
        }
    }
}
