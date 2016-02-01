using System;
using System.Collections.Generic;
using System.IO;

using Android.Content;

using mapKnight.Utils;
using mapKnight.Values;
using mapKnight.Entity;

namespace mapKnight.Android.CGL.Entity
{
	public class CGLSet : Set
	{
		public readonly int Texture;
		public readonly Size TextureSize;

		public CGLSet (XMLElemental setConfig, Context context) : base (setConfig)
		{
			// load texture
			CGLTools.LoadedImage limage = CGLTools.LoadImage (context, Path.Combine ("sets", Name + ".png"));
			Texture = limage.Texture;
			TextureSize = new Size (limage.Width, limage.Height);
		}
	}
}