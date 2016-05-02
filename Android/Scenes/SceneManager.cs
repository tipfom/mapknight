using System;
using System.Collections.Generic;
using mapKnight.Android.CGL.GUI;

namespace mapKnight.Android.Scenes {
    public class SceneManager {
        private List<IScene> addedScenes = new List<IScene> ( );
        private int currentScene;
        public IScene Current { get { return addedScenes[currentScene]; } }

        public SceneManager (IScene startScene, params IScene[ ] others) {
            addedScenes.Add (startScene);
            addedScenes.AddRange (others);
            Next (null, startScene.GetType ( ), null);
        }

        public void Next (object sender, Type nextscene, object[] data) {
            currentScene = addedScenes.FindIndex ((IScene scene) => scene.GetType ( ) == nextscene);
            currentScene = (currentScene != -1) ? currentScene : 0;
            Current.Begin (sender?.GetType (), data);
        }

        public List<GUIItem> GetCurrentGUIItems () {
            return Current.GUI.GetAllItems ();
        }
    }
}