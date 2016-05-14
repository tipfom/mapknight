using System.Reflection;
using Android.Content;
using mapKnight.Android;
using mapKnight.Android.CGL.GUI;
using mapKnight.Android.CGL.Programs;
using mapKnight.Android.Config;
using mapKnight.Android.Scenes;
using mapKnight.Basic;
using Newtonsoft.Json;

namespace mapKnight {
    public static class Content {
        #region instances
        // version string
        public static string Version;
        private static ContentConfig config;

        //data
        public static Context Context { get; private set; }

        public static SceneManager SceneManager { get; private set; }
        public static GUITouchHandler TouchHandler { get; private set; }

        // programs
        public static Collection ProgramCollection { get; private set; }
        #endregion

        #region inits
        public delegate void HandleInitCompleted (Context GameContext);

        public static event HandleInitCompleted Initialized;

        public static void PrepareInit (Context context) {
            Context = context;
            Version = Assembly.GetExecutingAssembly ( ).GetName ( ).Version.ToString (3);
#if DEBUG
            Version += " (DEBUG)";
#else
            Version += " (STABLE)";
#endif

            config = JsonConvert.DeserializeObject<ContentConfig> (Assets.Load<string> ("config", "content.json"));

            Screen.Change (new Size (context.Resources.DisplayMetrics.WidthPixels, context.Resources.DisplayMetrics.HeightPixels));
            Log.Print (typeof (Content), "Current Version : " + Version);
        }

        public static void Init () {
            ProgramCollection = new Collection ( );
            SceneManager = new SceneManager (
                new MainMenuScene ( ),
                new GameScene (JsonConvert.DeserializeObject<GameConfig> (Assets.Load<string> ("config", "game.json"))));
            TouchHandler = new GUITouchHandler (SceneManager.GetCurrentGUIItems);
            // Data = new SaveManager (System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "gamedata.db3"));

            Initialized?.Invoke (Content.Context);
        }

        #endregion
    }
}