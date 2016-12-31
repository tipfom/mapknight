using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [Instantiatable]
    public class DrawComponent : Component {
        private float[ ][ ] cachedVerticies;

        public DrawComponent (Entity owner) : base(owner) {
        }

        public override void PostUpdate ( ) {
            if (Owner.IsOnScreen) {
                List<VertexData> entityVertexData = new List<VertexData>( );
                string[ ] spriteData = (string[ ])Owner.GetComponentInfo(ComponentData.Texture);
                float[ ][ ] vertexData = (float[ ][ ])Owner.GetComponentInfo(ComponentData.Verticies);
                float scaleX =
                    (Owner.HasComponentInfo(ComponentData.ScaleX) ?
                    (float)Owner.GetComponentInfo(ComponentData.ScaleX)[0] : 1f);
                Color colorData =
                    (Owner.HasComponentInfo(ComponentData.Color) ?
                    (Color)Owner.GetComponentInfo(ComponentData.Color)[0] : Color.White);

                if (cachedVerticies == null) {
                    cachedVerticies = new float[vertexData.Length][ ];
                    for (int i = 0; i < vertexData.Length; i++) {
                        cachedVerticies[i] = new float[8];
                    }
                }

                Vector2 positionOnScreen = Owner.PositionOnScreen;
                for (int i = 0; i < vertexData.Length; i++) {
                    Mathf.TranslateAndScale(vertexData[i], ref cachedVerticies[i], positionOnScreen.X, positionOnScreen.Y, scaleX, 1);
                    VertexData entryVertexData = new VertexData(cachedVerticies[i], spriteData[i], colorData);
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
                while (Owner.HasComponentInfo(ComponentData.ScaleX))
                    Owner.GetComponentInfo(ComponentData.ScaleX);
            }
        }

        public new class Configuration : Component.Configuration {

            public override Component Create (Entity owner) {
                return new DrawComponent(owner);
            }
        }
    }
}