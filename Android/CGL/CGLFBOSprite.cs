using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using mapKnight.Basic;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL {
    public class CGLFBOSprite {
        private BufferData frameBuffer;
        // texturenames, dictionary< spritename, verticies>
        private Dictionary<CGLTexture2D, Dictionary<string, float[]>> sprites = new Dictionary<CGLTexture2D, Dictionary<string, float[]>> ();
        
        public CGLFBOSprite () {
            int buffersize = CGLTools.GetMaxTextureSize ();
            frameBuffer = CGLTools.GenerateFramebuffer (buffersize, buffersize);
        }

        public void AddTexture(CGLTexture2D texture) {
            sprites.Add (texture, new Dictionary<string, float[]> ());
        }

        public void AddSprite(CGLTexture2D texture, string name, Point spriteLocation, Size spriteSize) {
            float top = (float)spriteLocation.Y / frameBuffer.Height;
            float bottom = (float)(spriteLocation.Y+spriteSize.Height)  / frameBuffer.Height;
            float left = (float)spriteLocation.X / frameBuffer.Width;
            float right = (float)(spriteLocation.X +spriteSize.Width) / frameBuffer.Width;

            sprites[texture].Add (name, new float[] { left,top,left,bottom,right,bottom,right,top});
        }

        public void RemoveTexture(CGLTexture2D texture) {
            sprites.Remove (texture);
        }

        public void Prepare () {
            // draw all textures to buffer

        }
    }
}