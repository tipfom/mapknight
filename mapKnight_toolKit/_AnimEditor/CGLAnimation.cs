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

		public CGLAnimation (string[] boundedPoints)
		{
            foreach(string bp in boundedPoints)
            {
                Default.Add(bp, new float[] { 0f, 0f, 0f, 0f });
            }
		}

		public void Start ()
		{
            if(steps.Count > 0)
            {
                Finished = false;
                currentStep = 0;
                Current = Default.Clone();
                timePassed = 0;
                timeNext = steps[0].Item1;
            }
		}

		//public void Step (int deltatime)
		//{
		//	// end if animation is allready finished
		//	if (Finished == true)
		//		return;
			
		//	timePassed += deltatime;

		//	if (timePassed >= timeNeeded) {
		//		Finished = true;
		//	} else {
		//		if (timePassed > timeNext) {
		//			currentStep++;
		//			if (currentStep < steps.Count)
		//				timeNext += steps [currentStep].Item1;
		//			else
		//				timeNext += loopTime;
		//		}
					
		//		for (int i = 0; i < steps [0].Item2.Count; i++) {
		//			if (currentStep == 0) {
		//				// first step ( ref-point = default )
		//				string currentItem = steps [currentStep].Item2.ElementAt (i).Key;
		//				float lerpPercent = 1f - (float)(timeNext - timePassed) / (float)steps [currentStep].Item1;
		//				float newX = CGLTools.Lerp (Default [currentItem] [0], steps [currentStep].Item2 [currentItem] [0], lerpPercent);
		//				float newY = CGLTools.Lerp (Default [currentItem] [1], steps [currentStep].Item2 [currentItem] [1], lerpPercent);
		//				float newRotation = CGLTools.Lerp (Default [currentItem] [2], steps [currentStep].Item2 [currentItem] [2], lerpPercent);
		//				Current [currentItem] = new float[] {
		//					newX,
		//					newY,
		//					newRotation,
		//					steps [currentStep].Item2 [currentItem] [3]
		//				};
		//			} else if (currentStep < steps.Count) {
		//				// normal step ( ref-point = last final position )
		//				string currentItem = steps [currentStep].Item2.ElementAt (i).Key;
		//				float lerpPercent = 1f - (float)(timeNext - timePassed) / (float)steps [currentStep].Item1;
		//				float newX = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [0], steps [currentStep].Item2 [currentItem] [0], lerpPercent);
		//				float newY = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [1], steps [currentStep].Item2 [currentItem] [1], lerpPercent);
		//				float newRotation = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [2], steps [currentStep].Item2 [currentItem] [2], lerpPercent);
		//				Current [currentItem] = new float[] {
		//					newX,
		//					newY,
		//					newRotation,
		//					steps [currentStep].Item2 [currentItem] [3]
		//				};
		//			} else {
		//				// go back to default
		//				string currentItem = steps [currentStep - 1].Item2.ElementAt (i).Key;
		//				float lerpPercent = 1f - (float)(timeNext - timePassed) / (float)loopTime;
		//				float newX = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [0], Default [currentItem] [0], lerpPercent);
		//				float newY = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [1], Default [currentItem] [1], lerpPercent);
		//				float newRotation = CGLTools.Lerp (steps [currentStep - 1].Item2 [currentItem] [2], Default [currentItem] [2], lerpPercent);
		//				Current [currentItem] = new float[] {
		//					newX,
		//					newY,
		//					newRotation,
		//					Default [currentItem] [3]
		//				};
		//			}
		//		}
		//	}
		//}
	}
}
