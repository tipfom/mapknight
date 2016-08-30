using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [Instantiatable]
    public class DrawComponent : Component {

        public DrawComponent (Entity owner) : base(owner) {
        }

        public override void PostUpdate ( ) {
            if (Owner.IsOnScreen) {
                List<VertexData> entityVertexData = new List<VertexData>( );
                string[ ] spriteData = (string[ ])Owner.GetComponentInfo(ComponentData.Texture);
                float[ ][ ] vertexData = (float[ ][ ])Owner.GetComponentInfo(ComponentData.Verticies);
                Color colorData =
                    (Owner.HasComponentInfo(ComponentData.Color) ?
                    (Color)Owner.GetComponentInfo(ComponentData.Color)[0] : Color.White);

                Vector2 positionOnScreen = Owner.PositionOnScreen;
                for (int i = 0; i < vertexData.Length; i++) {
                    VertexData entryVertexData = new VertexData(
                                   Mathf.Translate(vertexData[i], 0, 0, positionOnScreen.X, positionOnScreen.Y),
                                   spriteData[i],
                                   colorData);
                    entityVertexData.Add(entryVertexData);
                }

                Owner.World.Renderer.QueueVertexData(Owner.Species, entityVertexData);
            } else {
                while (Owner.HasComponentInfo(ComponentData.Verticies))
                    Owner.GetComponentInfo(ComponentData.Verticies);
                while (Owner.HasComponentInfo(ComponentData.Texture))
                    Owner.GetComponentInfo(ComponentData.Texture);
                while (Owner.HasComponentInfo(ComponentData.Color))
                    Owner.GetComponentInfo(ComponentData.Color);
            }
        }

        public new class Configuration : Component.Configuration {

            public override Component Create (Entity owner) {
                return new DrawComponent(owner);
            }
        }
    }
}