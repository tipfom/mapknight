using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Extended.Graphics.Handle;

namespace mapKnight.Extended.Graphics.Buffer {
    public interface IAttributeBuffer : IBuffer {
        int Dimensions { get; }
        int Stride { get; set; }

        void Bind (AttributeHandle attribute);
        void Bind (int location);
    }
}
