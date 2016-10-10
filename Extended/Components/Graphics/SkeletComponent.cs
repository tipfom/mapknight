using System;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;

namespace mapKnight.Extended.Components.Graphics {

    [UpdateBefore(typeof(DrawComponent))]
    public class SkeletComponent : Component {
        private float[ ][ ] defaultVertexData;

        public SkeletComponent (Entity owner, float[ ][ ] defaultvertexdata) : base(owner) {
            defaultVertexData = defaultvertexdata;
        }

        public override void PostUpdate ( ) {
            if (!Owner.IsOnScreen) return;

            float[ ][ ] currentVertexData = new float[defaultVertexData.Length][ ];
            for (int i = 0; i < defaultVertexData.Length; i++)
                Array.Copy(defaultVertexData[i], currentVertexData[i] = new float[8], 8);

            // update currentvertexdata based on the current transform
            for (int i = 0; i < defaultVertexData.Length; i++) {
                for (int j = 0; j < 4; j++) {
                    currentVertexData[i][j * 2 + 0] = currentVertexData[i][j * 2 + 0] * Owner.Transform.Size.X * Owner.World.VertexSize;
                    currentVertexData[i][j * 2 + 1] = currentVertexData[i][j * 2 + 1] * Owner.Transform.Size.Y * Owner.World.VertexSize;
                }
            }

            Owner.SetComponentInfo(ComponentData.Verticies, currentVertexData);
        }

        public new class Configuration : Component.Configuration {
            private float[ ][ ] internalParsedBones;
            public Rectangle[ ] Bones { set { internalParsedBones = value.Select(item => item.Verticies( )).ToArray( ); } }

            public override Component Create (Entity owner) {
                return new SkeletComponent(owner, internalParsedBones);
            }
        }
    }
}