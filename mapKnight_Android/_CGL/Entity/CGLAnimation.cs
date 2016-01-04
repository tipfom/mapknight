using System;
using System.Collections.Generic;
using System.Linq;

using mapKnight.Utils;

namespace mapKnight.Android.CGL.Entity
{
	public class CGLAnimation
	{
		public string Action;
		public Dictionary<string,float[]> Default = new Dictionary<string, float[]> ();
		// first float = x, second float = y, third float = rotation in degree
		public Dictionary<string,float[]> Current = new Dictionary<string, float[]> ();
		private List<Tuple<int, Dictionary<string,float[]>>> steps = new List<Tuple<int, Dictionary<string, float[]>>> ();
		private int currentStep;
		private int timeNeeded;
		private int timePassed;
		private int timeNext;
		// time till next sequence

		public bool Finished;
		public readonly bool Loopable;
		public readonly bool Abortable;

		public CGLAnimation (XMLElemental animConfig)
		{
			Action = animConfig.Attributes ["action"];
			Abortable = Boolean.Parse (animConfig.Attributes ["abortable"]);
			Loopable = Boolean.Parse (animConfig.Attributes ["loopable"]);

			// load default
			foreach (XMLElemental bpoint in animConfig["default"].GetAll()) {
				Default.Add (bpoint.Attributes ["name"], new float[] {
					float.Parse (bpoint.Attributes ["x"]),
					float.Parse (bpoint.Attributes ["y"]),
					float.Parse (bpoint.Attributes ["rot"])
				});
			}

			foreach (XMLElemental step in animConfig.GetAll("step")) {
				steps.Add (new Tuple<int, Dictionary<string, float[]>> (int.Parse (step.Attributes ["time"]), new Dictionary<string, float[]> ()));

				foreach (XMLElemental bpoint in step.GetAll()) {
					steps [steps.Count - 1].Item2.Add (bpoint.Name, new float[] {
						float.Parse (bpoint.Attributes ["x"]),
						float.Parse (bpoint.Attributes ["y"], float.Parse (bpoint.Attributes ["rot"]))
					});
				}
			}

			timeNeeded = (int)steps.Aggregate ((overall, current) => overall += current.Item1); // add time up
		}

		public void Start ()
		{
			Finished = false;
			currentStep = 0;
			Current = Default;
			timePassed = 0;
		}

		public void Step (float deltatime)
		{
			if (Finished == true)
				return;
			
			timePassed += deltatime;

			if (timePassed >= timeNeeded) {
				Finished = true;
				currentStep = -1;
				Current = Default;
			} else {
				if (timePassed > timeNext) {
					Current = steps [currentStep].Item2;
					currentStep++;
					timeNext += steps [currentStep].Item1;
				}
			}
		}
	}
}
