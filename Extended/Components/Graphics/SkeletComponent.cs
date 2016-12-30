using System;
using System.Linq;
using mapKnight.Core;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Graphics;

namespace mapKnight.Extended.Components.Graphics {

    [UpdateBefore(typeof(DrawComponent))]
    public class SkeletComponent : Component {
        private float _Rotation;
        public float Rotation { get { return _Rotation; } set { isDirty = isDirty || _Rotation != value; _Rotation = value; } }
        private bool _Mirrored;
        public bool Mirrored { get { return _Mirrored; } set { isDirty = isDirty || _Mirrored != value; _Mirrored = value; } }
        private bool isDirty = true;

        private readonly float[ ][ ] defaultVertexData;
        private float[ ][ ] currentVertexData;

        public SkeletComponent (Entity owner, float[ ][ ] defaultvertexdata) : base(owner) {
            defaultVertexData = defaultvertexdata;
            currentVertexData = new float[defaultVertexData.Length][ ];

            Window.Changed += ( ) => isDirty = true;
            Owner.Transform.SizeChanged += ( ) => isDirty = true;
        }

        public override void Update (DeltaTime dt) {
            if (isDirty)
                AdjustVerticies( );
        }

        public override void PostUpdate ( ) {
            if (!Owner.IsOnScreen)
                return;
            Owner.SetComponentInfo(ComponentData.Verticies, currentVertexData);
        }

        private void AdjustVerticies ( ) {
            Vector2 scale = Owner.Transform.Size * Owner.World.VertexSize;
            for (int i = 0; i < defaultVertexData.Length; i++) {
                currentVertexData[i] = Mathf.TransformAtOrigin(defaultVertexData[i], 0, 0, _Rotation, _Mirrored, scale);
            }
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