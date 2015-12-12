using System;

using Android.Views;
using Android.Content;
using Android.Util;
using Android.Opengl;

namespace mapKnight.Android.CGL
{
	public class CGLView : GLSurfaceView
	{
		GLSurfaceView.IRenderer Renderer;

		public CGLView (Context context) : base (context)
		{
			this.SetEGLContextClientVersion (2);

			Renderer = new CGLRenderer (context);
			base.SetEGLConfigChooser (8, 8, 8, 8, 16, 0);
			this.SetRenderer (Renderer);
			this.RenderMode = Rendermode.Continuously;
		}
	}

}