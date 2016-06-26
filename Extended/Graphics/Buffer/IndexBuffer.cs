using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Buffer {
    public class IndexBuffer : IBuffer {
        public int Length { get; }
        public int Bytes { get; }

        private int buffer;

        public IndexBuffer (int quads) {
            Length = quads * 6;
            Bytes = Length * sizeof(short);

            // create and fill buffer
            short[ ] data = new short[Length];
            for (int i = 0; i < quads; i++) {
                data[i * 6 + 0] = (short)(i * 4 + 0);
                data[i * 6 + 1] = (short)(i * 4 + 1);
                data[i * 6 + 2] = (short)(i * 4 + 2);
                data[i * 6 + 3] = (short)(i * 4 + 0);
                data[i * 6 + 4] = (short)(i * 4 + 2);
                data[i * 6 + 5] = (short)(i * 4 + 3);
            }

            GL.GenBuffers(1, out buffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(data.Length * sizeof(short)), data, BufferUsage.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Bind ( ) {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer);
        }

        public void Delete ( ) {
            GL.DeleteBuffers(1, ref buffer);
        }
    }
}
