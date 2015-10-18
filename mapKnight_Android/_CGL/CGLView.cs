using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Views;
using Android.Content;
using Android.Util;

namespace mapKnight_Android{
	namespace CGL
	{
		public class CGLView : Android.Opengl.GLSurfaceView
		{
			Android.Opengl.GLSurfaceView.IRenderer Renderer;

			public CGLView (Context context, int testtextureid) : base(context)
			{
				this.SetEGLContextClientVersion (2);

				Renderer = new CGLRenderer (context, testtextureid);
				this.SetRenderer (Renderer);
				this.RenderMode = Android.Opengl.Rendermode.Continuously;
			}
		}
	}
}