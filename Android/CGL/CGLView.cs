using Android.Content;
using Android.Opengl;

namespace mapKnight.Android.CGL {
    public class CGLView : GLSurfaceView {
        GLSurfaceView.IRenderer Renderer;

        public CGLView (Context context) : base (context) {
            base.SetEGLConfigChooser (8, 8, 8, 8, 16, 0);
            this.SetEGLContextClientVersion (2);
            
            Renderer = new CGLRenderer ();
            this.SetRenderer (Renderer);
            this.RenderMode = Rendermode.Continuously;
        }
    }

}