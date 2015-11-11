﻿using System;

using Android.Views;
using Android.Content;
using Android.Util;

namespace mapKnight_Android.CGL
{
	public class CGLView : Android.Opengl.GLSurfaceView
	{
		Android.Opengl.GLSurfaceView.IRenderer Renderer;

		public CGLView (Context context) : base (context)
		{
			this.SetEGLContextClientVersion (2);

			Renderer = new CGLRenderer (context);
			this.SetRenderer (Renderer);
			this.RenderMode = Android.Opengl.Rendermode.Continuously;
		}
	}

}