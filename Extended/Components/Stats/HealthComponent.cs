using System;
using mapKnight.Core;
using System.Collections.Generic;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.UI;

namespace mapKnight.Extended.Components.Stats {

    public class HealthComponent : Component {
        private const int HEALTHBAR_ENTITY_ID = -2;
        private const float HEALTHBAR_WIDTH_PER_VALUE = 0.1f;

        public readonly int Initial;
        public float Current;
        public Func<Entity, bool> IsHit; 

        private ArmorComponent armorComponent;

        public HealthComponent (Entity owner, int health) : base(owner) {
            Initial = health;
            Current = health;
        }

        public override void Prepare ( ) {
            if (Owner.HasComponent<ArmorComponent>())
                armorComponent = Owner.GetComponent<ArmorComponent>( );
            if(!Owner.World.Renderer.HasTexture(HEALTHBAR_ENTITY_ID))
            Owner.World.Renderer.AddTexture(HEALTHBAR_ENTITY_ID, Assets.Load<SpriteBatch>("interface"));
        }

        public override void Update (DeltaTime dt) {
            while (Owner.HasComponentInfo(ComponentData.Damage)) {
                object[] data = Owner.GetComponentInfo(ComponentData.Damage);
                if (IsHit?.Invoke((Entity)data[0]) ?? true) {
                    Current -= ((float)data[1] * ((armorComponent != null) ? armorComponent.PhysicalMultiplier : 1f));
                    if (Current < 0)
                        Owner.Destroy( );
                }
            }
        }

        public override void PostUpdate( ) {
            Owner.World.Renderer.QueueVertexData(HEALTHBAR_ENTITY_ID, GetHealthbar( ));
        }

        private List<VertexData> GetHealthbar( ) {
            List<VertexData> healthbarData = new List<VertexData>(1);
            float width = Current * HEALTHBAR_WIDTH_PER_VALUE * Owner.World.VertexSize;
            float height = 0.1f * Owner.World.VertexSize;
            healthbarData.Add(new VertexData(UIRectangle.GetVerticies(Owner.PositionOnScreen.X - width / 2f, Owner.PositionOnScreen.Y + 2 * height + Owner.Transform.HalfSize.Y * Owner.World.VertexSize, width, height), "blank", Color.Red));
            return healthbarData;
        }

        public new class Configuration : Component.Configuration {
            public int Value;

            public override Component Create (Entity owner) {
                return new HealthComponent(owner, Value);
            }
        }
    }
}