using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Player;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using Map = mapKnight.Extended.Graphics.Map;
using mapKnight.Core.World.Components;

namespace mapKnight.Extended.Screens {

    public class GameplayScreen : Screen {
        private const int MAX_TIME_BETWEEN_UPDATES = 100;

        private UILabel debugLabel;
        private UIPanel leftPanel, rightPanel;
        private UIGesturePanel controlPanel;
        private UIBar healthBar;

        private Map map;
        private Entity testEntity;
        private PlayerComponent testEntityPlayer;

        public GameplayScreen( ) {
        }

        public override void Draw( ) {
            map.Draw( );
            base.Draw( );
        }

        public override void Load( ) {
            int begin = Environment.TickCount;
            map = Assets.Load<Map>("Schtart");
            map.EntityAdded += (Entity obj) => { obj.Prepare( ); };
#if DEBUG
            Debug.Print(this, $"map loading took {Environment.TickCount - begin} ms");
#endif

            debugLabel = new UILabel(this, new UIRightMargin(0.1f), new UITopMargin(0.075f), 0.05f, "", UITextAlignment.Right);
            SetupControls( );
            base.Load( );
        }

        private void SetupControls( ) {
            controlPanel = new UIGesturePanel(this, new UILeftMargin(0), new UITopMargin(0), new RelativeSize(3f / 5f, 1f), Assets.GetGestureStore("gestures"));
            controlPanel.OnGesturePerformed += (string gesture) => {
#if DEBUG
                global::Android.Widget.Toast.MakeText(Assets.Context, gesture, global::Android.Widget.ToastLength.Short).Show( );
#endif
                if (gesture == UIGesturePanel.SWIPE_UP) {
                    if (testEntity.GetComponent<MotionComponent>( ).IsOnGround || testEntity.GetComponent<MotionComponent>( ).IsOnPlatform)
                        testEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Jump);
                } else
                    testEntity.SetComponentInfo(ComponentData.InputGesture, gesture);
            };

            leftPanel = new UIPanel(this, new UIRightMargin(Window.Ratio * 2f / 5f), new UITopMargin(0), new RelativeSize(1f / 5f, 1f));
            leftPanel.Click += ( ) => testEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Left);
            leftPanel.Release += ( ) => testEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Left);
            leftPanel.Leave += ( ) => testEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Left);

            rightPanel = new UIPanel(this, new UIRightMargin(0), new UITopMargin(0), new RelativeSize(1f / 5f, 1f));
            rightPanel.Click += ( ) => testEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Right);
            rightPanel.Release += ( ) => testEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Right);
            rightPanel.Leave += ( ) => testEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Right);
        }

        public override void Update(DeltaTime dt) {
            if (Math.Abs(Manager.FrameTime.Milliseconds) < MAX_TIME_BETWEEN_UPDATES) {
                map.Update(dt);
                base.Update(dt);
                debugLabel.Color = Color.White;
            } else {
                debugLabel.Color = Color.Red;
            }
            debugLabel.Text = $"frame: {Manager.FrameTime.TotalMilliseconds:00.0}\n" +
                              $"update: {Manager.UpdateTime.TotalMilliseconds:00.0}\n" +
                            $"draw: {Manager.DrawTime.TotalMilliseconds:00.0}\n";
        }

        protected override void Activated( ) {
            while(map.Entities.Count > 0) {
                map.Entities[0].Destroy( );
                map.Entities.RemoveAt(0);
            }

            EntityCollection.Enemys.Guardians.Tent.Create(new Vector2(12, 18), map);
            EntityCollection.Enemys.Slime.Create(new Vector2(55, 20), map);
            EntityCollection.Enemys.Plugger.Create(new Vector2(52, 4), map);
            EntityCollection.Enemys.Plugger.Create(new Vector2(53, 4), map);
            EntityCollection.Enemys.Plugger.Create(new Vector2(54, 4), map);
            EntityCollection.Enemys.Plugger.Create(new Vector2(55, 4), map);
            EntityCollection.Obstacles.Landmine.Create(new Vector2(24, 4), map);
            EntityCollection.NPCs.Lenny.Create(new Vector2(7, 1), map);
            EntityCollection.Enemys.Shell.Create(new Vector2(36, 4), map);
            EntityCollection.Platforms.Copper.Create(new Vector2(13.1f, 4), map);
            EntityCollection.Platforms.Copper.Create(new Vector2(5f, 13), map);
            EntityCollection.Enemys.Sepling.Create(new Vector2(10, 3), map);
            EntityCollection.Enemys.Shark.Create(new Vector2(40, 21), map);
            EntityCollection.Obstacles.Moonball.Create(new Vector2(34, 17), map);
            EntityCollection.Enemys.BlackHole.Create(new Vector2(68f, 9f), map);

            testEntity = EntityCollection.Players.Diamond.Create(new Vector2(7, 18), map);
            testEntityPlayer = testEntity.GetComponent<PlayerComponent>( );
            map.Focus(testEntity.ID);

            healthBar?.Dispose( );
            healthBar = new UIBar(this, new Color(255, 0, 0, 127), new Color(255, 255, 255, 63), testEntityPlayer.Health, new UILeftMargin(0), new UITopMargin(0), new RelativeSize(1f, 0.025f), UIDepths.MIDDLE);

            for (int i = 0; i < map.Entities.Count; i++)
                map.Entities[i].Prepare( );
            base.Activated( );
        }
    }
}