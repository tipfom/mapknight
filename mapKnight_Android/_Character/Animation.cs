using System;
using System.Collections.Generic;
using System.Linq;

namespace mapKnight_Android
{
	public class Animation : ICloneable
	{
		public readonly bool Abortable;
		private List<AnimationStep> iAnimationSteps;

		public Animation (XMLElemental config)
		{
			Abortable = Convert.ToBoolean (config.Attributes ["abortable"]);

			iAnimationSteps = new List<AnimationStep> ();
			foreach (XMLElemental step in config.GetAll("step")) {
				iAnimationSteps.Add (new AnimationStep (step));
			}
		}

		protected Animation (bool abortable, List<AnimationStep> animationsteps)
		{
			Abortable = abortable;
			iAnimationSteps = animationsteps;
		}

		public void Abort ()
		{
			if (Abortable) {
				// Abort running animation
			}
		}

		public void Start ()
		{
			
		}

		public void Update ()
		{
			
		}

		public object Clone ()
		{
			return (object)new Animation (Abortable, iAnimationSteps.Clone ());
		}
	}
}

