using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.Entity {
    public class CGLEntityPreset : EntityPreset {
        protected List<CGLSet> sets;
        protected List<CGLAnimation> animations;

        protected List<CGLBoundedPoint> boundedPoints;

        protected int weight;
        protected fSize bounds;

        public CGLEntityPreset (XMLElemental entityConfig, Context context) : base (entityConfig) {
            sets = new List<CGLSet> ();
            animations = new List<CGLAnimation> ();
            boundedPoints = new List<CGLBoundedPoint> ();

            foreach (XMLElemental set in entityConfig.GetAll ("set")) {
                sets.Add (new CGLSet (set, context));
            }
            foreach (XMLElemental animation in entityConfig.GetAll ("anim")) {
                animations.Add (new CGLAnimation (animation));
            }

            foreach (XMLElemental def in entityConfig["def"].GetAll ()) {
                switch (def.Name) {
                case "slot":
                    for (int i = 0; i < Convert.ToInt32 (def.Attributes["bpcount"]); i++) {
                        boundedPoints.Add (new CGLBoundedPoint (ParseCoordinates (def["texture"].Attributes, (Size)sets[0].Texture.Bounds),
                            (Slot)Enum.Parse (typeof (Slot), def.Attributes["name"], true), def.Attributes["name"] + "_" + i.ToString (),
                            new fSize (
                                float.Parse (def["drawsize"].Attributes["width"], NumberStyles.Any, CultureInfo.InvariantCulture),
                                float.Parse (def["drawsize"].Attributes["height"], NumberStyles.Any, CultureInfo.InvariantCulture))));
                    }
                    break;
                }
            }

            weight = int.Parse (entityConfig["physx"]["bounds"].Attributes["weight"]);
            bounds = new fSize (float.Parse (entityConfig["physx"]["bounds"].Attributes["width"]), float.Parse (entityConfig["physx"]["bounds"].Attributes["height"]));
        }

        public CGLEntity Instantiate (uint level, string set) {
            return Instantiate (level, set, new fPoint (0, 0));
        }

        public CGLEntity Instantiate (uint level, string set, fPoint position) {
            return new CGLEntity (defaultAttributes[Attribute.Health] + (int)((level - 1) * attributeIncrease[Attribute.Health]), position, name,
                weight, bounds,
                boundedPoints, animations, sets.Find ((CGLSet obj) => obj.Name == set));
        }

        private fRectangle ParseCoordinates (Dictionary<string, string> config, Size imageSize) {
            // konvertiert die geradezahligen koordination von sprites in opengl koordinaten
            return (new Rectangle (new Point (Convert.ToInt32 (config["x"]), Convert.ToInt32 (config["y"])), new Size (Convert.ToInt32 (config["width"]), Convert.ToInt32 (config["height"]))) / imageSize);
        }
    }
}

