using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Buffer {
    public class ClientBuffer : IAttributeBuffer {
        public int Dimensions { get; private set; }
        public int Length { get; private set; }
        public int Bytes { get; private set; }
        public int Stride { get; set; }

        public float[ ] Data { get; set; }

        public ClientBuffer (int dimensions, int quads) {
            Dimensions = dimensions;
            Length = quads * dimensions;
            Bytes = Length * sizeof(float);
            Stride = Dimensions * sizeof(float);

            Data = new float[Length];
        }

        public void Bind (AttributeHandle attribute) {
            Bind(attribute.Location);
        }

        public void Bind (int location) {
            GL.VertexAttribPointer(location, Dimensions, VertexAttribPointerType.Float, false, Stride, Data);
        }

        public void Dispose ( ) {
            Data = null;
            Dimensions = 0;
            Length = 0;
            Bytes = 0;
            Stride = 0;
        }
    }
}
