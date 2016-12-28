using System;
using Android.Content;
using Android.Opengl;
using OpenTK;
using OpenTK.Platform.Android;
using mapKnight.Extended;
using mapKnight.Extended.Graphics;

namespace mapKnight.Android {
    public class View : AndroidGameView {
        public View (Context context) : base(context) {
            OpenTK.Graphics.GraphicsContext.ShareContexts = true;♂
            ContextRenderingApi = OpenTK.Graphics.GLVersion.ES2;
            Window.Info = WindowInfo;
        }

        protected override void OnSizeChanged (int w, int h, int oldw, int oldh) {
            Window.Change(w, h);
            base.OnSizeChanged(w, h, oldw, oldh);
        }

        protected override void OnLoad (EventArgs e) {
            base.OnLoad(e);
            Manager.Initialize( );
            Run( );
        }

        protected override void OnRenderFrame (FrameEventArgs e) {
            Manager.Update( );
            SwapBuffers( );
        }
    }
    /*
    public class View : GLSurfaceView {
        public View (Context context) : base(context) {
            base.SetEGLConfigChooser(8, 8, 8, 8, 16, 0);
            this.SetEGLContextClientVersion(2);

            this.SetRenderer(new Renderer( ));
            this.RenderMode = Rendermode.Continuously;
        }
    }
    */
}