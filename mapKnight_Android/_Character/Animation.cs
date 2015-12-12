using System;
using System.Collections.Generic;
using System.Linq;

using mapKnight.Values;
using mapKnight.Utils;

namespace mapKnight.Android
{
	public class Animation : ICloneable
	{
		public readonly bool Abortable;

		private List<AnimationStep> iAnimationSteps;

		private Vector2D[] iTranslations;
		private Vector2D[] iFinalTranslations;
		private fVector2D[] iMovement;
		private int iCurrentAnimation;
		private int iCurrentAnimTime;
		private int iNextAnimTime;

		public Animation (XMLElemental config)
		{
			Abortable = Convert.ToBoolean (config.Attributes ["abortable"]);

			iAnimationSteps = new List<AnimationStep> ();
			foreach (XMLElemental step in config.GetAll("step")) {
				iAnimationSteps.Add (new AnimationStep (step));
			}

			iTranslations = new Vector2D[Content.Character.BodyParts];
			iMovement = new fVector2D[Content.Character.BodyParts];
		}

		protected Animation (bool abortable, List<AnimationStep> animationsteps)
		{
			Abortable = abortable;
			iAnimationSteps = animationsteps;
		}

		public void Abort ()
		{
			if (Abortable && iCurrentAnimation > -1) {
				iCurrentAnimation = -1;
			}
		}

		public void Start ()
		{
			if (iCurrentAnimation < 0) {
				iCurrentAnimation = 0;
			}
		}

		public void Update (int frametime)
		{
			Update (frametime, false);
		}

		public void Update (int frametime, bool repeat)
		{
			if (iCurrentAnimation > -1) {
				iCurrentAnimTime += frametime;
				if (iCurrentAnimTime <= iNextAnimTime) {
					for (int i = 0; i < iTranslations.Length; i++) {
						iTranslations [i] += iMovement [i] * frametime;
					}
				} else {
					// der nächste animationsschritt muss ausgeführt werden
					if (iFinalTranslations != null)
						iFinalTranslations = iAnimationSteps [iCurrentAnimation].GetFinalPosition (iFinalTranslations);
					else
						iFinalTranslations = iAnimationSteps [iCurrentAnimation].GetFinalPosition ();
					iTranslations = iFinalTranslations;

					iCurrentAnimation++;
					if (iCurrentAnimation >= iAnimationSteps.Count && repeat) {
						iCurrentAnimation = 0;
					} else if (iCurrentAnimation >= iAnimationSteps.Count) {
						iCurrentAnimation = -1;
						iFinalTranslations = null;
						return;
					}

					for (int i = 0; i < Content.Character.BodyParts; i++) {
						if (iAnimationSteps [iCurrentAnimation].Movements.ContainsKey (i)) {
							//wenn sich das körperteil bewegt
							float moveX = (float)iAnimationSteps [iCurrentAnimation].Movements [i].X / (float)iAnimationSteps [iCurrentAnimation].Time;
							float moveY = (float)iAnimationSteps [iCurrentAnimation].Movements [i].Y / (float)iAnimationSteps [iCurrentAnimation].Time;
							iMovement [i] = new fVector2D (moveX, moveY);
						} else {
							iMovement [i] = new fVector2D (0, 0);
						}
					}

					iCurrentAnimTime -= iNextAnimTime;
					iNextAnimTime = iAnimationSteps [iCurrentAnimation].Time;
				}
			}
		}

		public Vector2D? GetTranslation (string name)
		{
			return GetTranslation (Content.Character.GetID (name));
		}

		public Vector2D? GetTranslation (int id)
		{
			if (id < iAnimationSteps.Count)
				return iTranslations [id];
			return null;
		}

		public object Clone ()
		{
			return (object)new Animation (Abortable, iAnimationSteps.Clone ());
		}
	}
}