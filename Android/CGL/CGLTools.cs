using Java.Nio;
using mapKnight.Basic;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL {
    public static class CGLTools {
        public static BufferData GenerateFramebuffer (int width, int height) {
            BufferData bufferdata = new BufferData ( ) { Width = width, Height = height };

            int[ ] temp = new int[1];
            GL.GlGenFramebuffers (1, temp, 0);
            bufferdata.FrameBuffer = temp[0];

            GL.GlGenTextures (1, temp, 0);
            bufferdata.FrameBufferTexture = temp[0];

            GL.GlGenRenderbuffers (1, temp, 0);
            bufferdata.RenderBuffer = temp[0];

            GL.GlBindTexture (GL.GlTexture2d, bufferdata.FrameBufferTexture);
            GL.GlTexImage2D (GL.GlTexture2d, 0, GL.GlRgba, width, height, 0, GL.GlRgba, GL.GlUnsignedByte, null);

            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

            GL.GlBindRenderbuffer (GL.GlRenderbuffer, bufferdata.RenderBuffer);
            GL.GlRenderbufferStorage (GL.GlRenderbuffer, GL.GlDepthComponent16, width, height);

            GL.GlBindFramebuffer (GL.GlFramebuffer, bufferdata.FrameBuffer);

            GL.GlFramebufferTexture2D (GL.GlFramebuffer, GL.GlColorAttachment0, GL.GlTexture2d, bufferdata.FrameBufferTexture, 0);

            // reset
            GL.GlBindTexture (GL.GlTexture2d, 0);
            GL.GlBindRenderbuffer (GL.GlRenderbuffer, 0);
            GL.GlBindFramebuffer (GL.GlFramebuffer, 0);

            return bufferdata;
        }

        public static BufferData GenerateFramebuffer () {
            return GenerateFramebuffer ((Size)Screen.ScreenSize);
        }

        public static BufferData GenerateFramebuffer (Size size) {
            return GenerateFramebuffer (size.Width, size.Height);
        }

        public static void DeleteTexture (int texture) {
            GL.GlDeleteTextures (1, new int[ ] { texture }, 0);
        }

        public static void DeleteFrameBuffer (int framebuffer) {
            GL.GlDeleteFramebuffers (1, new int[ ] { framebuffer }, 0);
        }

        public static void DeleteRenderBuffer (int renderbuffer) {
            GL.GlDeleteRenderbuffers (1, new int[ ] { renderbuffer }, 0);
        }

        public static void DeleteBufferData (BufferData bufferdata) {
            DeleteFrameBuffer (bufferdata.FrameBuffer);
            DeleteRenderBuffer (bufferdata.RenderBuffer);
            DeleteTexture (bufferdata.FrameBufferTexture);
        }

        public static BufferData UpdateFramebuffer (BufferData oldbufferdata) {
            DeleteBufferData (oldbufferdata);
            return GenerateFramebuffer (oldbufferdata.Width, oldbufferdata.Height);
        }

        public static FloatBuffer CreateBuffer (float[ ] source) {
            return CreateBuffer (source, source.Length);
        }
        public static ShortBuffer CreateShortBuffer (int size) {
            ByteBuffer byteBuffer = ByteBuffer.AllocateDirect ((int)size * sizeof (float));
            byteBuffer.Order (ByteOrder.NativeOrder ( ));
            ShortBuffer result = byteBuffer.AsShortBuffer ( );
            result.Position (0);
            return result;
        }

        public static FloatBuffer CreateFloatBuffer (int size) {
            ByteBuffer byteBuffer = ByteBuffer.AllocateDirect ((int)size * sizeof (float));
            byteBuffer.Order (ByteOrder.NativeOrder ( ));
            FloatBuffer result = byteBuffer.AsFloatBuffer ( );
            result.Position (0);
            return result;

        }

        public static FloatBuffer CreateBuffer (float[ ] source, int size) {
            ByteBuffer byteBuffer = ByteBuffer.AllocateDirect (size * sizeof (float));
            byteBuffer.Order (ByteOrder.NativeOrder ( ));
            FloatBuffer floatBuffer = byteBuffer.AsFloatBuffer ( );
            floatBuffer.Put (source);
            floatBuffer.Position (0);
            return floatBuffer;
        }

        public static ShortBuffer CreateBuffer (short[ ] source) {
            return CreateBuffer (source, source.Length);
        }

        public static ShortBuffer CreateBuffer (short[ ] source, int size) {
            ByteBuffer byteBuffer = ByteBuffer.AllocateDirect (size * sizeof (short));
            byteBuffer.Order (ByteOrder.NativeOrder ( ));
            ShortBuffer shortBuffer = byteBuffer.AsShortBuffer ( );
            shortBuffer.Put (source);
            shortBuffer.Position (0);
            return shortBuffer;
        }
    }
}