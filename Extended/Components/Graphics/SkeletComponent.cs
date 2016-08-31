using System;
using System.Collections.Generic;
using System.Linq;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [ComponentRequirement(typeof(DrawComponent))]
    [UpdateBefore(ComponentEnum.Draw)]
    public class SkeletComponent : Component {
        private float[ ][ ] defaultVertexData;

        public SkeletComponent (Entity owner, float[ ][ ] defaultvertexdata) : base(owner) {
            defaultVertexData = defaultvertexdata;
        }

        public override void Update (DeltaTime dt) {
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
            public Dictionary<string, VertexBone> Bones {
                set {
                    List<string> sortedBones = new List<string>(value.Keys);
                    sortedBones.Sort( );
                    internalParsedBones = value.Keys.Select(key => GetVerticies(value[key])).ToArray( );
                }
            }

            private float[ ] GetVerticies (VertexBone bone) {
                return new float[ ] {
                        bone.Position.X - bone.Size.X / 2 - 0.5f,
                        bone.Position.Y + bone.Size.Y / 2 - 0.5f,
                        bone.Position.X - bone.Size.X / 2 - 0.5f,
                        bone.Position.Y - bone.Size.Y / 2 - 0.5f,
                        bone.Position.X + bone.Size.X / 2 - 0.5f,
                        bone.Position.Y - bone.Size.Y / 2 - 0.5f,
                        bone.Position.X + bone.Size.X / 2 - 0.5f,
                        bone.Position.Y + bone.Size.Y / 2 - 0.5f
                    };
            }

            public override Component Create (Entity owner) {
                return new SkeletComponent(owner, internalParsedBones);
            }
        }
    }
}