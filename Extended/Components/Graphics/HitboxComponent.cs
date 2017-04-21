using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
#if DEBUG
namespace mapKnight.Extended.Components.Graphics {
    public class HitboxComponent : Component {
        private const int HITBOX_SPECIES = -2;
        private static Spritebatch2D HITBOX_TEXTURE;

        private Color color;
        private float[ ] originVertexData;
        private float[ ] transformedVertexData;

        public HitboxComponent (Entity owner, Color color) : base(owner) {
            color.A = 128;
            this.color = color;
        }

        public override void Prepare ( ) {
            if (HITBOX_TEXTURE == null) {
                HITBOX_TEXTURE = new Spritebatch2D(new Dictionary<string, int[ ]>( ) { ["0"] = new int[ ] { 0, 0, 1, 1 } }, Texture2D.CreateEmpty( ));
                Owner.World.Renderer.AddTexture(HITBOX_SPECIES, HITBOX_TEXTURE);
            }

            UpdateOriginVertexData( );
            Owner.Transform.SizeChanged += UpdateOriginVertexData;
            Window.Changed += UpdateOriginVertexData;
            transformedVertexData = new float[8];
        }

        public override void Draw ( ) {
            if (Owner.IsOnScreen)
                Owner.World.Renderer.QueueVertexData(HITBOX_SPECIES, ConstructVertexData( ));
        }

        public override void Destroy ( ) {
            Window.Changed -= UpdateOriginVertexData;
        }

        private void UpdateOriginVertexData ( ) {
            originVertexData = (Owner.Transform.Size * Owner.World.VertexSize).ToQuad( );
        }

        private IEnumerable<VertexData> ConstructVertexData ( ) {
            Mathf.TransformAtOrigin(originVertexData, ref transformedVertexData, Owner.PositionOnScreen.X, Owner.PositionOnScreen.Y, 0f, false);
            yield return new VertexData(transformedVertexData, "0", color);
        }

        public new class Configuration : Component.Configuration {
            public Color Color = Color.Red;

            public override Component Create (Entity owner) {
                return new HitboxComponent(owner, Color);
            }
        }
    }
}
#endif