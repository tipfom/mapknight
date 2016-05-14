using System;
using System.Collections.Generic;
using mapKnight.Basic;

namespace mapKnight.Android.Entity.Components {
    public class DrawComponent : Component {
        public DrawComponent (Entity owner) : base (owner) {

        }

        public override void Update (float dt) {
            if (Owner.Owner.IsOnScreen (Owner)) {
                Entity.VertexData entityVertexData = new Entity.VertexData ( );
                Dictionary<string, string> spriteData = new Dictionary<string, string> ( );
                Dictionary<string, float[ ]> vertexData = new Dictionary<string, float[ ]> ( );

                while (Owner.HasComponentInfo (ComponentType.Draw)) {
                    ComponentInfo ComponentInfo = Owner.GetComponentInfo (ComponentType.Draw);
                    switch (ComponentInfo.Action) {
                    case ComponentAction.TextureData:
                        // bodypart name, sprite name
                        spriteData = (Dictionary<string, string>)ComponentInfo.Data;
                        break;
                    case ComponentAction.VertexData:
                        vertexData = (Dictionary<string, float[ ]>)ComponentInfo.Data;
                        entityVertexData.QuadCount = vertexData.Count;
                        break;
                    }
                }

                entityVertexData.Entity = Owner.ID;
                entityVertexData.SpriteNames = new List<string> ( );
                entityVertexData.VertexCoords = new float[vertexData.Count * 8];
                int currentEntry = 0;

                Vector2 positionOnScreen = (Owner.Transform.Center - Owner.Owner.Camera.ScreenCentre) * Owner.Owner.VertexSize;
                foreach (var entry in vertexData) {
                    Array.Copy (MathHelper.Translate (entry.Value, 0, 0, positionOnScreen.X, positionOnScreen.Y), 0, entityVertexData.VertexCoords, currentEntry * 8, 8);
                    entityVertexData.SpriteNames.Add (spriteData[entry.Key]);
                    currentEntry++;
                }

                Owner.Owner.Renderer.QueueVertexData (entityVertexData);
            }
        }
    }
}