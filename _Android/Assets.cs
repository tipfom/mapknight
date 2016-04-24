using System;
using System.IO;
using Android.Graphics;
using Android.Opengl;
using mapKnight.Android.CGL;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android {
    class Assets {
        public static CGLTexture2D LoadTexture (string name) {
            // load texture
            int[] loadedtexture = new int[1];
            GL.GlGenTextures (1, loadedtexture, 0);

            BitmapFactory.Options bfoptions = new BitmapFactory.Options ();
            bfoptions.InScaled = false;
            Stream bitmapStream = Content.Context.Assets.Open (System.IO.Path.Combine ("textures", name));
            Bitmap bitmap = BitmapFactory.DecodeStream (bitmapStream, null, bfoptions);
            bitmapStream.Close ();
            bfoptions.Dispose ();

            GL.GlBindTexture (GL.GlTexture2d, loadedtexture[0]);

            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

            GLUtils.TexImage2D (GL.GlTexture2d, 0, bitmap, 0);

            CGLTexture2D loadedimage = new CGLTexture2D (loadedtexture[0], bitmap.Width, bitmap.Height);

            bitmap.Recycle ();
            GL.GlBindTexture (GL.GlTexture2d, 0);

            // Error Check
            int error = GL.GlGetError ();
            if (error != 0) {
                Log.All (typeof (Content), "error while loading mainimage (errorcode => " + error.ToString () + ")", MessageType.Debug);
                throw new FileLoadException ("error while loading mainimage (errorcode => " + error.ToString () + ")");
            }
            if (loadedtexture[0] == 0) {
                Log.All (typeof (Content), "loaded mainimage is zero", MessageType.Debug);
                throw new FileLoadException ("loaded mainimage is zero");
            }

            return loadedimage;
        }

        [Obsolete ("dont save image data as bytes, its stupid")]
        public static CGLTexture2D LoadTexture (byte[] data) {
            int[] loadedtexture = new int[1];
            GL.GlGenTextures (1, loadedtexture, 0);

            Bitmap bitmap = BitmapFactory.DecodeByteArray (data, 0, data.Length);

            GL.GlBindTexture (GL.GlTexture2d, loadedtexture[0]);

            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

            GLUtils.TexImage2D (GL.GlTexture2d, 0, bitmap, 0);

            CGLTexture2D limage = new CGLTexture2D (loadedtexture[0], bitmap.Width, bitmap.Height);

            bitmap.Recycle ();
            GL.GlBindTexture (GL.GlTexture2d, 0);

            // Error Check
            int error = GL.GlGetError ();
            if (error != 0) {
                Log.All (typeof (Content), "error while loading mainimage (errorcode => " + error.ToString () + ")", MessageType.Debug);
                throw new FileLoadException ("error while loading mainimage (errorcode => " + error.ToString () + ")");
            }
            if (loadedtexture[0] == 0) {
                Log.All (typeof (Content), "loaded mainimage is zero", MessageType.Debug);
                throw new FileLoadException ("loaded mainimage is zero");
            }

            return limage;
        }
    }
}