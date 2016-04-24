using System;
using System.Collections.Generic;
using System.IO;
using Android.Content;
using Android.Graphics;
using Android.Opengl;
using mapKnight.Basic;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL {
    public struct CGLSprite<T> {
        public int Texture { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Dictionary<T, fRectangle> Sprites { get; private set; }

        public CGLSprite (string filename, Context GameContext, List<XMLElemental> filedefs) : this (GameContext.Resources.Assets.Open (filename), filedefs) {

        }

        public CGLSprite (Stream filestream, List<XMLElemental> filedefs) : this () {
            loadTexture (filestream);
            translateDefs (filedefs);
        }

        private void loadTexture (Stream filestream) {
            int[] loadedtexture = new int[1];
            GL.GlGenTextures (1, loadedtexture, 0);

            BitmapFactory.Options bfoptions = new BitmapFactory.Options ();
            bfoptions.InScaled = false;
            Bitmap bitmap = BitmapFactory.DecodeStream (filestream, null, bfoptions);

            GL.GlBindTexture (GL.GlTexture2d, loadedtexture[0]);

            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMinFilter, GL.GlNearest);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureMagFilter, GL.GlNearest);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapS, GL.GlClampToEdge);
            GL.GlTexParameteri (GL.GlTexture2d, GL.GlTextureWrapT, GL.GlClampToEdge);

            GLUtils.TexImage2D (GL.GlTexture2d, 0, bitmap, 0);

            Height = bitmap.Height;
            Width = bitmap.Width;

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

            // set MainTexture to the loaded texture
            Texture = loadedtexture[0];
        }

        private void translateDefs (List<XMLElemental> defs) {
            Sprites = new Dictionary<T, fRectangle> ();
            foreach (XMLElemental def in defs) {
                fPoint Position = new fPoint ((float)Convert.ToInt32 (def.Attributes["x"]) / (float)Width, (float)Convert.ToInt32 (def.Attributes["y"]) / (float)Height);
                fSize Size = new fSize ((float)Convert.ToInt32 (def.Attributes["width"]) / (float)Width, (float)Convert.ToInt32 (def.Attributes["height"]) / (float)Height);
                T id = (T)Convert.ChangeType (def.Attributes["id"], typeof (T));
                Sprites.Add (id, new fRectangle (Position, Size));
            }
        }
    }
}

