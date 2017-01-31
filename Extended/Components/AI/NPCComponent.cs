using mapKnight.Core;

namespace mapKnight.Extended.Components.AI {
    public class NPCComponent : Component {
        public bool Available = true;

        private string[ ] messages;
        private int currentIndex = -1;
        
        public NPCComponent (Entity owner, string[] messages) : base(owner) {
            owner.Domain = EntityDomain.NPC;
            this.messages = messages;
        }

        public override void Collision (Entity collidingEntity) {
            if(collidingEntity.Domain == EntityDomain.Player) {
                Owner.SetComponentInfo(ComponentData.Color, Color.Fuchsia);
            }
        }

        public string NextMessage ( ) {
            currentIndex++;
            if (currentIndex == messages.Length) {
                currentIndex = -1;
                Available = false;
                return null;
            } else {
                return messages[currentIndex];
            }
        }

        public new class Configuration : Component.Configuration {
            public string[ ] Messages;

            public override Component Create (Entity owner) {
                return new NPCComponent(owner, Messages);
            }
        }
    }
}
