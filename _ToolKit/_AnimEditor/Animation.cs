using System;
using System.Collections.Generic;

using mapKnight.Basic;

namespace mapKnight.ToolKit {
    public class Animation {
        public string Action;
        public readonly Dictionary<string, float[]> Default = new Dictionary<string, float[]> ();
        // first float = x, second float = y, third float = rotation in degree, fourth = mirrored
        public List<Tuple<int, Dictionary<string, float[]>>> steps = new List<Tuple<int, Dictionary<string, float[]>>> ();

        public bool Finished = true;
        public bool Loopable;
        public bool Abortable;
        public int DefaultTime = 0;

        public Animation (string[] boundedPoints) {
            foreach (string bp in boundedPoints) {
                Default.Add (bp, new float[] { 0f, 0f, 0f, 0f });
            }
        }

        public Animation (XMLElemental animConfig) {
            Action = animConfig.Attributes["action"];
            Abortable = Boolean.Parse (animConfig.Attributes["abortable"]);
            Loopable = Boolean.Parse (animConfig.Attributes["loopable"]);
            DefaultTime = int.Parse (animConfig.Get ("default").Attributes["time"]);

            // load default
            foreach (XMLElemental bpoint in animConfig["default"].GetAll ()) {
                // add a new entry to the default dictionary and load the positiondata und the mirrored bool into a a new tuple
                Default.Add (bpoint.Attributes["name"], new float[] {
                        -float.Parse (bpoint.Attributes ["x"]),
                        float.Parse (bpoint.Attributes ["y"]),
                        float.Parse (bpoint.Attributes ["rot"]),
                        bool.Parse (bpoint.Attributes ["mirrored"]) ? 1f : 0f // if mirrored is true set last value to 1 else to 0
					});
            }

            foreach (XMLElemental step in animConfig.GetAll ("step")) {
                // query each step in the entity-file and add the steptime and a new dict to the steps list
                steps.Add (new Tuple<int, Dictionary<string, float[]>> (int.Parse (step.Attributes["time"]), new Dictionary<string, float[]> ()));

                foreach (XMLElemental bpoint in step.GetAll ()) {
                    // query each boundedpoint and add the bpoint data to the dict of the last added step
                    steps[steps.Count - 1].Item2.Add (bpoint.Attributes["name"], new float[] {
                            -float.Parse (bpoint.Attributes ["x"]),
                            float.Parse (bpoint.Attributes ["y"]),
                            float.Parse (bpoint.Attributes ["rot"]),
                            bool.Parse (bpoint.Attributes ["mirrored"]) ? 1f : 0f
                        });
                    // see above
                }
            }
        }

        public void AddStep () {
            if (steps.Count != 0)
                steps.Add (new Tuple<int, Dictionary<string, float[]>> (0, this.steps[steps.Count - 1].Item2.Clone ()));
            else
                steps.Add (new Tuple<int, Dictionary<string, float[]>> (0, this.Default.Clone ()));
        }

        public void AddStepDefault () {
            steps.Add (new Tuple<int, Dictionary<string, float[]>> (0, this.Default.Clone ()));
        }

        public void RemoveStep (int index) {
            steps.RemoveAt (index);
        }

        public void SetTime (int time, int index) {
            if (index == 0)
                DefaultTime = time;
            else
                steps[index - 1] = new Tuple<int, Dictionary<string, float[]>> (time, steps[index - 1].Item2);
        }

        public int GetTime (int index) {
            if (index == 0)
                return DefaultTime;
            else
                return steps[index - 1].Item1;
        }

        public Dictionary<string, float[]> GetStep (int index) {
            if (index == 0)
                return Default;
            else
                return steps[index - 1].Item2;
        }

        public XMLElemental Flush () {
            XMLElemental animElemental = new XMLElemental ("anim");

            animElemental.Attributes.Add ("action", Action);
            animElemental.Attributes.Add ("abortable", Abortable.ToString ().ToLower ());
            animElemental.Attributes.Add ("loopable", Loopable.ToString ().ToLower ());

            // add default
            animElemental.AddChild ("default");
            animElemental.Get ("default").Attributes.Add ("time", DefaultTime.ToString ());
            foreach (KeyValuePair<string, float[]> kvpair in Default) {
                XMLElemental currentBP = new XMLElemental ("bpoint");
                currentBP.Attributes.Add ("name", kvpair.Key);
                currentBP.Attributes.Add ("x", (-kvpair.Value[0]).ToString ());
                currentBP.Attributes.Add ("y", kvpair.Value[1].ToString ());
                currentBP.Attributes.Add ("rot", kvpair.Value[2].ToString ());
                currentBP.Attributes.Add ("mirrored", ((kvpair.Value[3] == 1f) ? true : false).ToString ().ToLower ());
                animElemental.Get ("default").AddChild (currentBP);
            }

            // add the other steps
            foreach (Tuple<int, Dictionary<string, float[]>> step in steps) {
                XMLElemental stepElemental = new XMLElemental ("step");
                stepElemental.Attributes.Add ("time", step.Item1.ToString ());

                // add bps
                foreach (KeyValuePair<string, float[]> kvpair in step.Item2) {
                    XMLElemental currentBP = new XMLElemental ("bpoint");
                    currentBP.Attributes.Add ("name", kvpair.Key);
                    currentBP.Attributes.Add ("x", (-kvpair.Value[0]).ToString ());
                    currentBP.Attributes.Add ("y", kvpair.Value[1].ToString ());
                    currentBP.Attributes.Add ("rot", kvpair.Value[2].ToString ());
                    currentBP.Attributes.Add ("mirrored", ((kvpair.Value[3] == 1f) ? true : false).ToString ().ToLower ());
                    stepElemental.AddChild (currentBP);
                }

                animElemental.AddChild (stepElemental);
            }

            return animElemental;
        }
    }
}
