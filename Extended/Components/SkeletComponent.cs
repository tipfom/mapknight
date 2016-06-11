using mapKnight.Core;
using mapKnight.Extended.Components.Communication;
using mapKnight.Extended.Graphics;
using System.Collections.Generic;

namespace mapKnight.Extended.Components {
    public class SkeletComponent : Component {
        Dictionary<string, float[]> defaultVertexData;

        public SkeletComponent (Entity owner, Dictionary<string, Bone> bones) : base (owner) {
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
            Identifier sender = Identifier.Skelet;

            if (!Owner.HasComponentInfo (Identifier.Skelet))
                currentVertexData = defaultVertexData.DeepClone ();
            else {
                Info ComponentInfo = Owner.GetComponentInfo (Identifier.Skelet);
                sender = ComponentInfo.Sender;
                currentVertexData = (Dictionary<string, float[]>)ComponentInfo.Data;
            }

            // update currentvertexdata based on the current transform
            foreach (string bone in currentVertexData.Keys) {
                for (int i = 0; i < currentVertexData[bone].Length / 2; i++) {
                    currentVertexData[bone][i * 2 + 0] = (currentVertexData[bone][i * 2 + 0] - 0.5f) * Owner.Transform.Bounds.X * Owner.Owner.VertexSize;
                    currentVertexData[bone][i * 2 + 1] = (currentVertexData[bone][i * 2 + 1] - 0.5f) * Owner.Transform.Bounds.Y * Owner.Owner.VertexSize;
                }
            }

            Owner.SetComponentInfo (Identifier.Draw, sender, Data.Verticies, currentVertexData);
        }
    }
}