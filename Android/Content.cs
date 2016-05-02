using System;
using System.Reflection;
using Android.Content;
using mapKnight.Android;
using mapKnight.Android.CGL;
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
        public static Basic.Version Version;
        private static ContentConfig config;

        //character
        public static Character Character { get; private set; }

        //data
        public static Context Context { get; private set; }

        public static SceneManager SceneManager { get; private set; }
        public static GUITouchHandler TouchHandler { get; private set; }

        // programs
        public static MatrixProgram MatrixProgram { get; private set; }
        public static FBOProgram FBOProgram { get; private set; }
        public static ColorProgram ColorProgram { get; private set; }
        #endregion

        #region inits
        public delegate void HandleInitCompleted (Context GameContext);

        public static event HandleInitCompleted Initialized;

        public static void PrepareInit (Context context) {
            Context = context;
            Version = new Basic.Version (Assembly.GetExecutingAssembly ( ).GetName ( ).Version.ToString ( ));
            config = JsonConvert.DeserializeObject<ContentConfig> (Assets.Load<string> ("config", "content.json"));

            Screen.Change (new Size (context.Resources.DisplayMetrics.WidthPixels, context.Resources.DisplayMetrics.HeightPixels));
            Log.Print (typeof (Content), "Current Version : " + Version.ToString ( ));
        }

        public static void Init () {
            ProgramHelper.Load ( );
            FBOProgram = new FBOProgram ( );
            MatrixProgram = new MatrixProgram ( );
            ColorProgram = new ColorProgram ( );
            SceneManager = new SceneManager (
                new MainMenuScene ( ),
                new GameScene (JsonConvert.DeserializeObject<GameConfig> (Assets.Load<string> ("config", "game.json"))));
            TouchHandler = new GUITouchHandler (SceneManager.GetCurrentGUIItems);
            // Data = new SaveManager (System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "gamedata.db3"));

            LoadCharacter ( );

            Initialized?.Invoke (Content.Context);
        }

        private static void LoadCharacter () {
            CharacterPreset preset = new CharacterPreset (XMLElemental.Load (Context.Assets.Open ("character/robot.character")), Context);
            Character = preset.Instantiate (10, "futuristic");
            Character.CollisionMask = Android.Physics.Flag.Map;
        }
        #endregion
    }
}