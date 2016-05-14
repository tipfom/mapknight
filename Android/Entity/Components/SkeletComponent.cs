using mapKnight.Basic;
using System.Collections.Generic;

namespace mapKnight.Android.Entity.Components {
    public class SkeletComponent : Component {
        Dictionary<string, float[]> defaultVertexData;

        public SkeletComponent (Entity owner, Dictionary<string, Rectangle> bones) : base (owner) {
            defaultVertexData = new Dictionary<string, float[]> ();
            foreach (var entry in bones) {
                defaultVertexData.Add (entry.Key,
                    new float[] {
                        entry.Value.Position.X - entry.Value.Size.X / 2,
                        entry.Value.Position.Y + entry.Value.Size.Y / 2,
                        entry.Value.Position.X - entry.Value.Size.X / 2,
                        entry.Value.Position.Y - entry.Value.Size.Y / 2,
                        entry.Value.Position.X + entry.Value.Size.X / 2,
                        entry.Value.Position.Y - entry.Value.Size.Y / 2,
                        entry.Value.Position.X + entry.Value.Size.X / 2,
                        entry.Value.Position.Y + entry.Value.Size.Y / 2
                    });
            }
        }

        public override void Update (float dt) {
            Dictionary<string, float[]> currentVertexData;
            Type sender = Type.Skelet;

            if (!Owner.HasComponentInfo (Type.Skelet))
                currentVertexData = defaultVertexData.Clone ();
            else {
                Info componentInfo = Owner.GetComponentInfo (Type.Skelet);
                sender = componentInfo.Sender;
                currentVertexData = ((Dictionary<string, float[]>)componentInfo.Data).Clone ();
            }

            // update currentvertexdata based on the current transform
            foreach (string bone in currentVertexData.Keys) {
                for (int i = 0; i < currentVertexData[bone].Length / 2; i++) {
                    currentVertexData[bone][i * 2 + 0] = (currentVertexData[bone][i * 2 + 0] - 0.5f) * Owner.Transform.Bounds.X * Owner.Owner.VertexSize;
                    currentVertexData[bone][i * 2 + 1] = (currentVertexData[bone][i * 2 + 1] - 0.5f) * Owner.Transform.Bounds.Y * Owner.Owner.VertexSize;
                }
            }

            Owner.SetComponentInfo (Type.Draw, sender, Action.VertexData, currentVertexData);
        }
    }
}