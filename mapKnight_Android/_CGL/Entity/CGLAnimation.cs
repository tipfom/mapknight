using System;

using mapKnight.Utils;

namespace mapKnight.Android.CGL.Entity
{
	public class CGLAnimation
	{
		public string Action;
		public float[] Default;
		public float[] Current;
		public bool Finished;
		public readonly bool Abortable;

		public CGLAnimation (XMLElemental animConfig)
		{
			Action = animConfig.Attributes ["action"];
			Abortable = Boolean.Parse (animConfig.Attributes ["abortable"]);


		}

		public void Start ()
		{
			
		}

		public void Step (float deltatime)
		{
			
		}
	}
}

