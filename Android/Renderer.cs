using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using mapKnight.Core;
using mapKnight.Graphics;

namespace mapKnight.Android {

    public class Renderer : Java.Lang.Object, GLSurfaceView.IRenderer {
        public Renderer ( ) {

        }

        public void OnDrawFrame (IGL10 gl) {
            Manager.Update( );
        }

        public void OnSurfaceChanged (IGL10 gl, int width, int height) {
            Window.Change(width, height);
        }

        public void OnSurfaceCreated (IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config) {
            // init ( needs to get called from openglcontext :/ )
            Manager.Initialize( );
        }
    }
}