using System;
using System.Collections.Generic;

using Android.Views;
using GL = Android.Opengl.GLES20;

using mapKnight.Values;

namespace mapKnight.Android
{
	public class ButtonManager : TouchManager
	{
		public ButtonManager () : base ()
		{
		}

		public Button Create (int x, int y, int width, int height)
		{
			return new Button (this, x, y, width, height);
		}

		public Button Create (Point position, Size size)
		{
			return new Button (this, position.X, position.Y, size.Width, size.Height);
		}

		public Button Create (Rectangle hitbox)
		{
			return new Button (this, hitbox.Position.X, hitbox.Position.Y, hitbox.Size.Width, hitbox.Size.Height);
		}
	}
}