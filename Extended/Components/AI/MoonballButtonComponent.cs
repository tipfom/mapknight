using System;
using System.Collections.Generic;
using mapKnight.Core.World;
using mapKnight.Core.World.Serialization;
using mapKnight.Core;

namespace mapKnight.Extended.Components.AI {
    public class MoonballButtonComponent : ButtonComponent {
        private Vector2 moonballOffset;
        private Entity.Configuration moonballConfiguration;

        public MoonballButtonComponent(Entity owner, Entity.Configuration moonballConfiguration) : base(owner, null) {
            onTriggerAction = OnTrigger;
            this.moonballConfiguration = moonballConfiguration;
        }

        public override void Load(Dictionary<DataID, object> data) {
            moonballOffset = (Vector2)data[DataID.MOONBALL_Offset];
        }

        private void OnTrigger(Entity e) {
            moonballConfiguration.Create(Owner.Transform.Center + moonballOffset, Owner.World, false);
        }

        public new class Configuration : Component.Configuration {
            public Entity.Configuration MoonballConfiguration;

            public override Component Create(Entity owner) {
                return new MoonballButtonComponent(owner, MoonballConfiguration);
            }
        }
    }
}
