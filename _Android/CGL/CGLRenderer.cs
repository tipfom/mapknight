using System;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using mapKnight.Android.CGL.Entity;
using mapKnight.Basic;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL {

    public class CGLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer {
        private float ratio;
        private GUI.GUILabel infoText;
        private GUI.GUIButton testButton;
        private GUI.GUIBar testBar;
        private ChangingProperty barProp;
        private int repeats = 0;

        // times
        private int drawTime;
        private int updateTime;
        private int frameTime = 1;

        public CGLRenderer () {
        }

        #region IRenderer implementation

        public void OnDrawFrame (IGL10 gl) {
            drawTime = Draw ( );
            updateTime = Update ( );

            infoText.Text =
                $"frameTime:{frameTime} ({(1000f / frameTime).ToString ("00.00")} fps)\n" +
                $"updateTime:{updateTime}\n" +
                $"drawTime:{drawTime}\n" +
                $"version:{Content.Version.ToString (false)} " +
#if DEBUG
                        "(DEBUG)\n" +
#else
                        "(STABLE)\n" +
#endif
                "code by tipfom\n" +
                "textures by fenon\n";
            if (repeats % 1 == 0)
                barProp.Current = frameTime;

            repeats++;
            CalculateFrameRate ( );
        }

        public void OnSurfaceChanged (IGL10 gl, int width, int height) {
            ratio = (float)width / height;

            GL.GlViewport (0, 0, width, height);
            GL.GlClearColor (0f, 0f, 0f, 1.0f);

            Screen.Change (new Size (width, height));
        }

        public void OnSurfaceCreated (IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config) {
            // init
            Content.Init ( );

            infoText = new GUI.GUILabel (new fVector2D (0.01f, 0.01f), 0.075f);
            testButton = new GUI.GUIButton ("test", new fRectangle (0.25f, 0.25f, 0.5f, 0.5f));
            testButton.Color = new Color (255, 0, 0, 255);
            barProp = new ChangingProperty (30);
            testBar = new GUI.GUIBar (barProp, new fRectangle (0.5f, 0.01f, 0.5f, 0.2f));
        }

        #endregion IRenderer implementation

        private int Draw () {
            int beginTime = Environment.TickCount;
            GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

            Content.Map.Draw ( );
            CGLEntity.Draw (frameTime);
            Content.GUI.Draw ( );

            return Environment.TickCount - beginTime;
        }

        private int Update () {
            int beginTime = Environment.TickCount;
            Content.Map.Step (frameTime);
            Content.Camera.Update ( );
            Content.GUI.Update (frameTime);

            return Environment.TickCount - beginTime;
        }

        private int lastTick;

        private void CalculateFrameRate () {
            frameTime = Environment.TickCount - lastTick;
            lastTick = Environment.TickCount;
        }
    }
}