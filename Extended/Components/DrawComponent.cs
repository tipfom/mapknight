using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components {
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
                    ComponentInfo ComponentInfo = Owner.GetComponentInfo(ComponentEnum.Draw);
                    switch (ComponentInfo.Action) {
                        case ComponentData.Texture:
                            // bodypart name, sprite name
                            spriteData = (Dictionary<string, string>)ComponentInfo.Data;
                            break;
                        case ComponentData.Verticies:
                            vertexData = (Dictionary<string, float[ ]>)ComponentInfo.Data;
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

                Owner.Owner.Renderer.QueueVertexData(Owner.Species, entityVertexData);
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