using Android.Graphics;
using Android.Opengl;
using mapKnight.Android.CGL;
using mapKnight.Android.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android {
    class Assets {

        private static Dictionary<string, CGLTexture2D> textureCache = new Dictionary<string, CGLTexture2D> ();
        public static CGLTexture2D LoadTexture (string name) {
            if (!textureCache.ContainsKey (name)) {
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

                CGLTexture2D loadedimage = new CGLTexture2D (loadedtexture[0], name, bitmap.Width, bitmap.Height);

                bitmap.Recycle ();
                GL.GlBindTexture (GL.GlTexture2d, 0);

                // Error Check
                int error = GL.GlGetError ();
                if (error != 0) {
                    Log.Warn (typeof (Content), "error while loading image (errorcode => " + error.ToString () + ")");
                    throw new FileLoadException ("error while loading image (errorcode => " + error.ToString () + ")");
                }
                if (loadedtexture[0] == 0) {
                    Log.Warn (typeof (Content), "loaded image is zero");
                    throw new FileLoadException ("loaded image is zero");
                }

                textureCache.Add (name, loadedimage);
                return loadedimage;
            } else {
                return textureCache[name];
            }

        }

        public static CGLSprite2D LoadSprite (string name, string config) {
            CGLTexture2D texture = Load<CGLTexture2D> (name);
            Dictionary<string, int[]> spriteContent = new Dictionary<string, int[]> ();
            JsonConvert.PopulateObject (Load<string> ("config", "sprites", config), spriteContent);
            return new CGLSprite2D (spriteContent, texture.Texture, name, texture.Width, texture.Height);
        }

        public static string LoadText (string path) {
            using (StreamReader reader = new StreamReader (Content.Context.Assets.Open (path))) {
                return reader.ReadToEnd ();
            }
        }

        private static JsonSerializerSettings entitySerializerSettings = new JsonSerializerSettings () {
            TypeNameHandling = TypeNameHandling.Auto,
            Binder = new Entity.ComponentSerializationBinder ()
        };
        public static EntityConfig LoadEntity (string name) {
            return JsonConvert.DeserializeObject<EntityConfig> (Assets.Load<string> ("entities", $"{name}.json"), entitySerializerSettings);
        }

        public static T Load<T>(params string[] paths) {
            Type request = typeof (T);
            if (request == typeof (CGLTexture2D) && paths.Length == 1) {
                return (T)((object)LoadTexture (paths[0]));
            } else if (request == typeof (string)) {
                return (T)((object)LoadText (System.IO.Path.Combine (paths)));
            } else if (request == typeof (CGLSprite2D) && paths.Length == 2) {
                return (T)((object)LoadSprite (paths[0], paths[1]));
            } else if (request == typeof (EntityConfig) && paths.Length == 1) {
                return (T)((object)LoadEntity (paths[0]));
            } else {
                throw new FileLoadException ($"requested filetype { request.FullName } couldnt be loaded with { paths.Length } parameters");
            }
        }
    }
}