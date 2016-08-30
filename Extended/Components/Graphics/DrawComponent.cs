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
                Dictionary<string, string> spriteData = new Dictionary<string, string>( );
                Dictionary<string, float[ ]> vertexData = new Dictionary<string, float[ ]>( );
                Color colorData = Color.White;

                while (Owner.HasComponentInfo(ComponentEnum.Draw)) {
                    Tuple<ComponentData, object> ComponentInfo = (Tuple<ComponentData, object>)Owner.GetComponentInfo(ComponentEnum.Draw);
                    switch (ComponentInfo.Item1) {
                        case ComponentData.Texture:
                            // bodypart name, sprite name
                            spriteData = (Dictionary<string, string>)ComponentInfo.Item2;
                            break;

                        case ComponentData.Verticies:
                            vertexData = (Dictionary<string, float[ ]>)ComponentInfo.Item2;
                            break;
                    }
                }

                Vector2 positionOnScreen = Owner.PositionOnScreen;
                foreach (var entry in vertexData) {
                    VertexData entryVertexData = new VertexData(
                        Mathf.Translate(entry.Value, 0, 0, positionOnScreen.X, positionOnScreen.Y),
                        spriteData[entry.Key],
                        colorData);
                    entityVertexData.Add(entryVertexData);
                }

                Owner.World.Renderer.QueueVertexData(Owner.Species, entityVertexData);
            } else {
                while (Owner.HasComponentInfo(ComponentEnum.Draw)) {
                    Owner.GetComponentInfo(ComponentEnum.Draw);
                }
            }
        }

        public new class Configuration : Component.Configuration {

            public override Component Create (Entity owner) {
                return new DrawComponent(owner);
            }
        }
    }
}