using Android.Content;
using Android.Opengl;

namespace mapKnight.Android {
    public class View : GLSurfaceView {
        public View (Context context) : base(context) {
            base.SetEGLConfigChooser(8, 8, 8, 8, 16, 0);
            this.SetEGLContextClientVersion(2);

            this.SetRenderer(new Renderer( ));
            this.RenderMode = Rendermode.Continuously;
        }
    }

}