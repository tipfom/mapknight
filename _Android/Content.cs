using System;
using System.Reflection;
using Android.Content;
using mapKnight.Android;
using mapKnight.Android.CGL;
using mapKnight.Android.CGL.GUI;
using mapKnight.Android.CGL.Programs;
using mapKnight.Basic;

namespace mapKnight {
    public static class Content {
        #region instances
        // screen bounds
        public static Size ScreenSize { get; private set; }

        public static float ScreenRatio { get; private set; }

        // version string
        public static Basic.Version Version;

        // current touch manager
        public static ButtonManager TouchManager { get; private set; }

        //character
        public static Character Character { get; private set; }

        //data
        public static SaveManager Data { get; private set; }

        public static Context Context { get; private set; }

        //map
        public static CGLMap Map { get; private set; }

        //viewing
        public static CGLCamera Camera { get; private set; }

        public static GUITextRenderer TextRenderer { get; private set; }
        public static CGLInterface Interface { get; private set; }

        // programs
        public static MatrixProgram MatrixProgram { get; private set; }
        public static NormalProgram NormalProgram { get; private set; }
        #endregion

        #region inits
        public delegate void HandleInitCompleted (Context GameContext);

        public static event HandleInitCompleted OnInit;

        public static event HandleInitCompleted OnPreInit;

        public static event HandleInitCompleted OnAfterInit;

        public delegate void HandleUpdate ();

        public static event HandleUpdate OnUpdate;

        public static void PreInit (XMLElemental configfile, Context context) {
            Context = context;
            Version = new Basic.Version (Assembly.GetExecutingAssembly ().GetName ().Version.ToString ());


            ScreenSize = new Size (context.Resources.DisplayMetrics.WidthPixels, context.Resources.DisplayMetrics.HeightPixels);
            ScreenRatio = (float)ScreenSize.Width / (float)ScreenSize.Height;

            Log.All (typeof (Content), "Current Version : " + Version.ToString (), MessageType.Info);

            OnPreInit?.Invoke (context);
        }

        public static void Init (XMLElemental configfile) {
            ProgramHelper.Load ();
            NormalProgram = new NormalProgram ();
            MatrixProgram = new MatrixProgram ();

            TouchManager = new ButtonManager ();
            Map = new CGLMap ("testmap.map");
            Camera = new CGLCamera (0.3f);
            Data = new SaveManager (System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "gamedata.db3"));
            Interface = new CGLInterface (configfile);
            TextRenderer = new Android.CGL.GUI.GUITextRenderer ();
            LoadCharacter ();

            OnInit?.Invoke (Content.Context);
        }

        public static void AfterInit () {
            Map.AddEntity (Content.Character);
            Camera.Update ();

            OnAfterInit?.Invoke (Context);
        }

        public static void Update (Size screensize) {
            ScreenSize = screensize;
            ScreenRatio = (float)ScreenSize.Width / (float)ScreenSize.Height;

            OnUpdate?.Invoke ();
        }

        private static void LoadCharacter () {
            CharacterPreset preset = new CharacterPreset (XMLElemental.Load (Context.Assets.Open ("character/robot.character")), Context);
            Character = preset.Instantiate (10, "futuristic");
            Character.CollisionMask = Android.PhysX.PhysXFlag.Map;
        }
        #endregion
    }
}