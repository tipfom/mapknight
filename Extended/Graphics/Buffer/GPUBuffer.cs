using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Buffer {
    public class GPUBuffer : IAttributeBuffer {
        public int Dimensions { get; private set; }
        public int Length { get; private set; }
        public int Bytes { get; private set; }
        public int Stride { get; set; }

        private int buffer;

        public GPUBuffer (int dimensions, int count, PrimitiveType type, BufferUsage usage = BufferUsage.DynamicDraw) :
            this(dimensions, count, type, null, usage) {

        }

        ~GPUBuffer ( ) {
            Dispose( );
        }

        public GPUBuffer (int dimensions, int count, PrimitiveType type, float[ ] initialData, BufferUsage usage = BufferUsage.DynamicDraw) {
            Dimensions = dimensions;
            Length = Dimensions * count * (int)type;
            Bytes = Length * sizeof(float);
            Stride = Dimensions * sizeof(float);

            // gen buffer
            GL.GenBuffers(1, out buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Bytes), initialData, usage);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Put (float[ ] data) {
            if (data.Length == Length) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, new IntPtr(Bytes), data);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            } else {
#if DEBUG
                Debug.Print(this, "data length didnt fit buffer, skipping");
#endif
            }
        }

        public void Bind (AttributeHandle attribute) {
            Bind(attribute.Location);
        }

        public void Bind (int location) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.VertexAttribPointer(location, Dimensions, VertexAttribPointerType.Float, false, Stride, IntPtr.Zero);
        }

        public void Dispose ( ) {
            GL.DeleteBuffers(1, ref buffer);
            Dimensions = 0;
            Length = 0;
            Bytes = 0;
            Stride = 0;
        }
    }
}
