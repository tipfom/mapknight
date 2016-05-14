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
            ComponentType sender = ComponentType.Skelet;

            if (!Owner.HasComponentInfo (ComponentType.Skelet))
                currentVertexData = defaultVertexData.Clone ();
            else {
                ComponentInfo ComponentInfo = Owner.GetComponentInfo (ComponentType.Skelet);
                sender = ComponentInfo.Sender;
                currentVertexData = ((Dictionary<string, float[]>)ComponentInfo.Data).Clone ();
            }

            // update currentvertexdata based on the current transform
            foreach (string bone in currentVertexData.Keys) {
                for (int i = 0; i < currentVertexData[bone].Length / 2; i++) {
                    currentVertexData[bone][i * 2 + 0] = (currentVertexData[bone][i * 2 + 0] - 0.5f) * Owner.Transform.Bounds.X * Owner.Owner.VertexSize;
                    currentVertexData[bone][i * 2 + 1] = (currentVertexData[bone][i * 2 + 1] - 0.5f) * Owner.Transform.Bounds.Y * Owner.Owner.VertexSize;
                }
            }

            Owner.SetComponentInfo (ComponentType.Draw, sender, ComponentAction.VertexData, currentVertexData);
        }
    }
}