using System;
using Android.Content;
using OpenTK;
using OpenTK.Platform.Android;
using mapKnight.Extended;
using mapKnight.Extended.Graphics;

namespace mapKnight.Android {
    public class View : AndroidGameView {
        bool firstLoad = false;

        public View (Context context) : base(context) {
            OpenTK.Graphics.GraphicsContext.ShareContexts = true;
            ContextRenderingApi = OpenTK.Graphics.GLVersion.ES2;
            Window.Info = WindowInfo;
        }

        protected override void OnSizeChanged (int w, int h, int oldw, int oldh) {
            Window.Change(w, h);
            base.OnSizeChanged(w, h, oldw, oldh);
        }

        protected override void OnLoad (EventArgs e) {
            base.OnLoad(e);
            if (!firstLoad)
                Manager.Initialize( );
            firstLoad = true;
            Run( );
        }

        protected override void OnRenderFrame (FrameEventArgs e) {
            Manager.Update( );
            SwapBuffers( );
        }

        protected override void Dispose (bool disposing) {
            firstLoad = false;
            base.Dispose(disposing);
        }
    }
}