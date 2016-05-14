using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using mapKnight.Basic;
using System;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL {

    public class CGLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer {
        // times
        private int drawTime;
        private int updateTime;
        private int frameTime = 1;

        public CGLRenderer () {
        }

        //            infoText.Text =
        //                $"frameTime:{frameTime} ({(1000f / frameTime).ToString ("00.00")} fps)\n" +
        //                $"updateTime:{updateTime}\n" +
        //                $"drawTime:{drawTime}\n" +
        //                $"version:{Content.Version.ToString (false)} " +
        //#if DEBUG
        //                        "(DEBUG)\n" +
        //#else
        //                        "(STABLE)\n" +
        //#endif
        //                "code by tipfom\n" +
        //                "textures by fenon\n";

        #region IRenderer implementation

        public void OnDrawFrame (IGL10 gl) {
            updateTime = Update ();
            drawTime = Draw ();
            calculateFrameTime ();

            //Log.Print (this, $"running at {1000f / frameTime} fps (drawtime={drawTime}; updatetime={updateTime})");
        }

        public void OnSurfaceChanged (IGL10 gl, int width, int height) {
            GL.GlViewport (0, 0, width, height);
            Screen.Change (new Size (width, height));
        }

        public void OnSurfaceCreated (IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config) {
            // init ( needs to get called from openglcontext :/ )
            Content.Init ();
            GL.GlClearColor (0f, 0f, 0f, 1.0f);
        }

        #endregion IRenderer implementation

        private int Draw () {
            int beginTime = Environment.TickCount;

            GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);
            Content.SceneManager.Current.Draw ();
            return Environment.TickCount - beginTime;
        }

        private int Update () {
            int beginTime = Environment.TickCount;

            Content.SceneManager.Current.Update (frameTime);

            return Environment.TickCount - beginTime;
        }

        private int lastTick;
        private void calculateFrameTime () {
            frameTime = Environment.TickCount - lastTick;
            lastTick = Environment.TickCount;
        }
    }
}