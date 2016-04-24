using System;
using Android.Content;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using mapKnight.Basic;
using mapKnight.Android.CGL.Entity;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL {
    public class CGLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer {
        float ratio;
        Context context;
        GUI.GUILabel infoText;

        // times
        int drawTime;
        int updateTime;
        int frameTime;

        public CGLRenderer (Context Context) {
            context = Context;
        }

        #region IRenderer implementation

        public void OnDrawFrame (IGL10 gl) {
            drawTime = Draw ();
            updateTime = Update ();

            infoText.Text =
                $"frameTime:{frameTime} ({(1000f / frameTime).ToString ("00.00")} fps)\n" +
                $"updateTime:{updateTime}\n" +
                $"drawTime:{drawTime}\n";// +
              //  $"version:{Content.Version.ToString (false)}\n" +
          //      "code by tipfom\n" +
            //    "textures by fenon";

            CalculateFrameRate ();
        }


        public void OnSurfaceChanged (IGL10 gl, int width, int height) {
            ratio = (float)width / height;

            GL.GlViewport (0, 0, width, height);
            GL.GlClearColor (0f, 0f, 0f, 1.0f);

            Content.Update (new Size (width, height));
        }

        public void OnSurfaceCreated (IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config) {
            // init

            XMLElemental configfile = XMLElemental.Load (context.Assets.Open ("main.xml"));
            Content.PreInit (configfile, context);
            Content.Init (configfile);
            Content.AfterInit ();

            Content.Interface.JumpButton.OnClick += () => {
                Content.Character.Jump ();
            };
            infoText = Content.TextRenderer.Create (new fVector2D (0f, 0.1f), 0.05f, "");
        }

        #endregion

        private int Draw () {
            int beginTime = Environment.TickCount;
            GL.GlClear (GL.GlColorBufferBit | GL.GlDepthBufferBit);

            Content.Map.Draw ();
            Content.Interface.Draw ();
            Content.TextRenderer.Draw ();
            CGLEntity.Draw (frameTime);

            return Environment.TickCount - beginTime;
        }

        private int Update () {
            int beginTime = Environment.TickCount;
            Content.Map.Step (frameTime);
            Content.Camera.Update ();

            if (Content.Interface.LeftButton.Clicked) {
                Content.Character.Move (Direction.Left);
            } else if (Content.Interface.RightButton.Clicked) {
                Content.Character.Move (Direction.Right);
            } else {
                Content.Character.ResetMovement ();
            }

            return Environment.TickCount - beginTime;
        }

        private int lastTick;
        private void CalculateFrameRate () {
            frameTime = Environment.TickCount - lastTick;
            lastTick = Environment.TickCount;
        }
    }
}