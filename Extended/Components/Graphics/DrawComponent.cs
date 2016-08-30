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

                if (Owner.HasComponentInfo(ComponentData.Texture))
                    spriteData = (Dictionary<string, string>)Owner.GetComponentInfo(ComponentData.Texture)[0];
                if (Owner.HasComponentInfo(ComponentData.Verticies))
                    vertexData = (Dictionary<string, float[ ]>)Owner.GetComponentInfo(ComponentData.Verticies)[0];


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
                while (Owner.HasComponentInfo(ComponentData.Verticies))
                    Owner.GetComponentInfo(ComponentData.Verticies);
                while (Owner.HasComponentInfo(ComponentData.Texture))
                    Owner.GetComponentInfo(ComponentData.Texture);
            }
        }

        public new class Configuration : Component.Configuration {

            public override Component Create (Entity owner) {
                return new DrawComponent(owner);
            }
        }
    }
}