using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [ComponentRequirement(typeof(DrawComponent))]
    [UpdateBefore(ComponentEnum.Draw)]
    public class SkeletComponent : Component {
        private Dictionary<string, float[ ]> defaultVertexData;

        public SkeletComponent (Entity owner, Dictionary<string, VertexBone> bones) : base(owner) {
            defaultVertexData = new Dictionary<string, float[ ]>( );
            foreach (var entry in bones) {
                defaultVertexData.Add(entry.Key,
                    new float[ ] {
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

        public override void Update (DeltaTime dt) {
            Dictionary<string, float[ ]> currentVertexData;

            if (!Owner.HasComponentInfo(ComponentData.VerticiesSkelet))
                currentVertexData = defaultVertexData.DeepClone( );
            else {
                Dictionary<string, float[ ]> info = (Dictionary<string, float[ ]>)Owner.GetComponentInfo(ComponentData.VerticiesSkelet)[0];
                currentVertexData = info;
            }

            // update currentvertexdata based on the current transform
            foreach (string bone in currentVertexData.Keys) {
                for (int i = 0; i < currentVertexData[bone].Length / 2; i++) {
                    currentVertexData[bone][i * 2 + 0] = (currentVertexData[bone][i * 2 + 0] - 0.5f) * Owner.Transform.Size.X * Owner.World.VertexSize;
                    currentVertexData[bone][i * 2 + 1] = (currentVertexData[bone][i * 2 + 1] - 0.5f) * Owner.Transform.Size.Y * Owner.World.VertexSize;
                }
            }

            Owner.SetComponentInfo(ComponentData.Verticies, currentVertexData);
        }

        

        public new class Configuration : Component.Configuration {
            public Dictionary<string, VertexBone> Bones;

            public override Component Create (Entity owner) {
                return new SkeletComponent(owner, Bones);
            }
        }
    }
}