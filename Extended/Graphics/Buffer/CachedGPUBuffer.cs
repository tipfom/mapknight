using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Buffer {
    public class CachedGPUBuffer : IAttributeBuffer {
        public int Dimensions { get; private set; }
        public int Length { get; private set; }
        public int Bytes { get; private set; }
        public int Stride { get; set; }

        public float[ ] Cache { get; set; }
        private int buffer;

        public CachedGPUBuffer (int dimensions, int quads, BufferUsage usage = BufferUsage.DynamicDraw) :
            this(dimensions, quads, new float[4 * quads * dimensions], usage) {

        }

        public CachedGPUBuffer (int dimensions, int quads, float[ ] initialData, BufferUsage usage = BufferUsage.DynamicDraw) {
            Dimensions = dimensions;
            Length = Dimensions * quads * 4;
            Bytes = Length * sizeof(float);
            Stride = Dimensions * sizeof(float);
            Cache = initialData;

            // gen buffer
            GL.GenBuffers(1, out buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Bytes), initialData, usage);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Put ( ) {
            if (Cache.Length == Length) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, new IntPtr(Bytes), Cache);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            } else {
#if DEBUG
                Debug.Print(this, "cache length didnt fit buffer, skipping");
#endif
            }
        }

        public void Bind (AttributeHandle attribute) {
            Bind(attribute.Location);
        }

        public void Bind (int location) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.VertexAttribPointer(location, Dimensions, VertexAttribPointerType.Float, false, Stride, 0);
        }

        public void Dispose ( ) {
            GL.DeleteBuffers(1, ref buffer);
            Cache = null;
            Dimensions = 0;
            Length = 0;
            Bytes = 0;
            Stride = 0;
        }
    }
}
