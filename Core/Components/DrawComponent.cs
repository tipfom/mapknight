using mapKnight.Core.Components.Communication;
using mapKnight.Graphics;
using System;
using System.Collections.Generic;

namespace mapKnight.Core.Components {
    public class DrawComponent : Component {
        public DrawComponent (Entity owner) : base(owner) {

        }

        public override void Update (float dt) {
            if (Owner.IsOnScreen) {
                List<VertexData> entityVertexData = new List<VertexData>( );
                Dictionary<string, string> spriteData = new Dictionary<string, string>( );
                Dictionary<string, float[ ]> vertexData = new Dictionary<string, float[ ]>( );
                Color colorData = Color.White;

                while (Owner.HasComponentInfo(Identifier.Draw)) {
                    Info ComponentInfo = Owner.GetComponentInfo(Identifier.Draw);
                    switch (ComponentInfo.Action) {
                        case Data.Texture:
                            // bodypart name, sprite name
                            spriteData = (Dictionary<string, string>)ComponentInfo.Data;
                            break;
                        case Data.Verticies:
                            vertexData = (Dictionary<string, float[ ]>)ComponentInfo.Data;
                            break;
                    }
                }
                
                Vector2 positionOnScreen = Owner.PositionOnScreen;
                foreach (var entry in vertexData) {
                    VertexData entryVertexData = new VertexData(
                        Mathf.Translate(entry.Value, 0, 0, positionOnScreen.X, positionOnScreen.Y),
                        spriteData[entry.Key],
                        0,
                        colorData);
                }

                Owner.Owner.Renderer.QueueVertexData(Owner.ID, entityVertexData);
            }
        }
    }
}