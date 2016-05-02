using System;
using mapKnight.Android.CGL;

namespace mapKnight.Android.Scenes {
    public interface IScene {
        mapKnight.Android.CGL.GUI.GUI GUI { get; }
        void Begin (Type caller, object[] data);
        void Draw ();
        void Update (float dt);
    }
}