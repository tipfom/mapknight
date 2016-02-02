using System;
using System.Collections.Generic;
using System.Linq;

using mapKnight.Utils;

namespace mapKnight.Android.CGL.Entity
{
	public class CGLAnimation
	{
		public string Action;
		public readonly Dictionary<string,float[]> Default = new Dictionary<string,float[]> ();
		// first float = x, second float = y, third float = rotation in degree
		public Dictionary<string,float[]> Current = new Dictionary<string,float[]> ();
		private List<Tuple<int, Dictionary<string,float[]>>> steps = new List<Tuple<int, Dictionary<string, float[]>>> ();

		private int loopTime;
		private int currentStep;
		private int timeNeeded;
		private int timePassed;
		private int timeNext;
		// time till next sequence

		public bool Finished = true;
		public readonly bool Loopable;
		public readonly bool Abortable;

		public CGLAnimation (XMLElemental animConfig)
		{
			Action = animConfig.Attributes ["action"];
			Abortable = Boolean.Parse (animConfig.Attributes ["abortable"]);
			Loopable = Boolean.Parse (animConfig.Attributes ["loopable"]);
			if (Loopable)
				loopTime = int.Parse (animConfig.Get ("default").Attributes ["time"]);

			// load default
			foreach (XMLElemental bpoint in animConfig["default"].GetAll()) {
				// add a new entry to the default dictionary and load the positiondata und the mirrored bool into a a new tuple
				Default.Add (bpoint.Attributes ["name"], new float[] { 
						float.Parse (bpoint.Attributes ["x"]), 
						float.Parse (bpoint.Attributes ["y"]), 
						float.Parse (bpoint.Attributes ["rot"]), 
						bool.Parse (bpoint.Attributes ["mirrored"]) ? 1f : 0f // if mirrored is true set last value to 1 else to 0
					});
			}

			foreach (XMLElemental step in animConfig.GetAll("step")) {
				// query each step in the entity-file and add the steptime and a new dict to the steps list
				steps.Add (new Tuple<int, Dictionary<string, float[]>> (int.Parse (step.Attributes ["time"]), new Dictionary<string, float[]> ()));

				foreach (XMLElemental bpoint in step.GetAll()) {
					// query each boundedpoint and add the bpoint data to the dict of the last added step
					steps [steps.Count - 1].Item2.Add (bpoint.Attributes ["name"], new float[] {
							float.Parse (bpoint.Attributes ["x"]),
							float.Parse (bpoint.Attributes ["y"]),
							float.Parse (bpoint.Attributes ["rot"]),
							bool.Parse (bpoint.Attributes ["mirrored"]) ? 1f : 0f
						}); 
					// see above
				}
			}

			for (int i = 0; i < steps.Count; i++) { // timeup
				timeNeeded += steps [i].Item1;
			}
			if (Loopable)
				timeNeeded += loopTime;
		}

		public void Start ()
		{
			Finished = false;
			currentStep = 0;
			Current = Default.Clone ();
			timePassed = 0;
			if (steps.Count > 0)
				timeNext = steps [0].Item1;
			else
				timeNext = 0;
		}

		public void Step (int deltatime)
		{
			// end if animation is allready finished
			if (Finished == true)
				return;
			
			timePassed += deltatime;

			if (timePassed >= timeNeeded) {
				Finished = true;
			} else {
				if (timePassed > timeNext) {
					currentStep++;
					if (currentStep < steps.Count)
						timeNext += steps [currentStep].Item1;
					else
						timeNext += loopTime;
				}
					
				for (int i = 0; i < steps [0].Item2.Count; i++) {
					if (currentStep == 0) {
						// first step ( ref-point = default )
						string currentItem = steps [currentStep].Item2.ElementAt (i).Key;
						float lerpPercent = 1f - (float)(timeNext - timePassed) / (float)steps [currentStep].Item1;
						float newX = CGLTools.Lerp (Default [currentItem] [0], steps [currentStep].Item2 [currentItem] [0], lerpPercent);
						float newY = CGLTools.Lerp (Default [currentItem] [1], steps [currentStep].Item2 [currentItem] [1], lerpPercent);
						float newRotation = CGLTools.Lerp (Default [currentItem] [2], steps [currentStep].Item2 [currentItem] [2], lerpPercent);
						Current [currentItem] = new float[] {
							newX,
							newY,
							newRotation,
							steps [currentStep].Item2 [currentItem] [3]
						};
					} else if (currentStep < steps.Count) {
						// normal step ( ref-point = last final position )
						string currentItem = steps [currentStep].Item2.ElementAt (i).Key;
						float lerpPercent = 1f - (float)(timeNext - timePassed) / (float)steps [currentStep].Item1;
						float newX = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [0], steps [currentStep].Item2 [currentItem] [0], lerpPercent);
						float newY = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [1], steps [currentStep].Item2 [currentItem] [1], lerpPercent);
						float newRotation = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [2], steps [currentStep].Item2 [currentItem] [2], lerpPercent);
						Current [currentItem] = new float[] {
							newX,
							newY,
							newRotation,
							steps [currentStep].Item2 [currentItem] [3]
						};
					} else {
						// go back to default
						string currentItem = steps [currentStep - 1].Item2.ElementAt (i).Key;
						float lerpPercent = 1f - (float)(timeNext - timePassed) / (float)loopTime;
						float newX = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [0], Default [currentItem] [0], lerpPercent);
						float newY = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [1], Default [currentItem] [1], lerpPercent);
						float newRotation = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [2], Default [currentItem] [2], lerpPercent);
						Current [currentItem] = new float[] {
							newX,
							newY,
							newRotation,
							Default [currentItem] [3]
						};
					}
				}
			}
		}
	}
}
