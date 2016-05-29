using Android.Content;
using Android.Graphics;
using Android.Opengl;
using mapKnight.Core;
using mapKnight.Graphics;
using System.IO;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android {
    class AssetProvider : Assets.IAssetProvider {
        public static Context Context { get; set; }

        public Stream GetStream (params string[ ] path) {
            return Context.Assets.Open(System.IO.Path.Combine(path));

        }

        public Texture2D GetTexture (string name) {
            // load texture
            int[ ] loadedtexture = new int[1];
            GL.GlGenTextures(1, loadedtexture, 0);

            BitmapFactory.Options bfoptions = new BitmapFactory.Options( );
            bfoptions.InScaled = false;
            Stream bitmapStream = Context.Assets.Open(System.IO.Path.Combine("textures", name));
            Bitmap bitmap = BitmapFactory.DecodeStream(bitmapStream, null, bfoptions);
            bitmapStream.Close( );
            bfoptions.Dispose( );

            GL.GlBindTexture(GL.GlTexture2d, loadedtexture[0]);

            GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
            GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
            GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
            GL.GlTexParameteri(GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

            GLUtils.TexImage2D(GL.GlTexture2d, 0, bitmap, 0);

            Texture2D loadedimage = new Texture2D(loadedtexture[0], new Size(bitmap.Width, bitmap.Height), name);

            bitmap.Recycle( );
            GL.GlBindTexture(GL.GlTexture2d, 0);

            // Error Check
            int error = GL.GlGetError( );
            if (error != 0) {
                throw new FileLoadException($"error while loading image ({error})");
            } else if (loadedtexture[0] == 0) {
                throw new FileLoadException("loaded image id is 0");
            }

            return loadedimage;
        }
    }
}