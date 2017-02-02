using System;
using mapKnight.Core;
using mapKnight.Extended.Components;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Player;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using Map = mapKnight.Extended.Graphics.Map;

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

        public GameplayScreen ( ) {
            Entity.EntityAdded += (Entity obj) => { obj.Prepare( ); };
        }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
        }

        public override void Load ( ) {
            int begin = Environment.TickCount;
            map = Assets.Load<Map>("beatiful_map");
#if DEBUG
            Debug.Print(this, $"map loading took {Environment.TickCount - begin} ms");
#endif

            EntityCollection.Enemys.Guardians.Tent.Create(new Vector2(26, 18), map);
            EntityCollection.Enemys.Slime.Create(new Vector2(9, 7.5f), map);
            EntityCollection.Enemys.Plugger.Create(new Vector2(72, 10), map);
            EntityCollection.Obstacles.Landmine.Create(new Vector2(21, 7), map);
            EntityCollection.Obstacles.Landmine.Create(new Vector2(22, 7), map);
            EntityCollection.Obstacles.Landmine.Create(new Vector2(2.5f, 8 ), map);
            EntityCollection.NPCs.Lenny.Create(new Vector2(62, 12), map);
            EntityCollection.Enemys.Shell.Create(new Vector2(42, 11), map);
            EntityCollection.Platforms.Copper.Create(new Vector2(3, 9), map);
            EntityCollection.Enemys.Sepling.Create(map.SpawnPoint + new Vector2(10, 1), map);
            EntityCollection.Enemys.Shark.Create(map.SpawnPoint + new Vector2(10, 1), map);

            testEntity = EntityCollection.Players.Diamond.Create(map.SpawnPoint, map);
            testEntityPlayer = testEntity.GetComponent<PlayerComponent>( );
            map.Focus(testEntity.ID);
            
            healthBar = new UIBar(this, new Color(255, 0, 0, 127), new Color(255, 255, 255, 63), testEntityPlayer.Health, new UILeftMargin(0), new UITopMargin(0), new Vector2(2 * Window.Ratio, 0.05f), UIDepths.MIDDLE);
            debugLabel = new UILabel(this, new UIRightMargin(0.1f), new UITopMargin(0.05f), 0.05f, "", UITextAlignment.Right);
            SetupControls( );
            Window.Changed += ( ) => {
                SetupControls( );
                healthBar.Size = new Vector2(2 * Window.Ratio, healthBar.Size.Y);
            };
            base.Load( );
        }

        private void SetupControls ( ) {
            controlPanel?.Dispose( );
            leftPanel?.Dispose( );
            rightPanel?.Dispose( );

            controlPanel = new UIGesturePanel(this, new UILeftMargin(0), new UITopMargin(0), new Vector2(Window.Ratio * 4f / 3f, 2), Assets.GetGestureStore("gestures"));
            controlPanel.OnGesturePerformed += (string gesture) => {
                global::Android.Widget.Toast.MakeText(Assets.Context, gesture, global::Android.Widget.ToastLength.Short).Show( );
                if (gesture == UIGesturePanel.SWIPE_UP) {
                    if(testEntity.GetComponent<MotionComponent>().IsOnGround  || testEntity.GetComponent<MotionComponent>( ).IsOnPlatform)
                    testEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Jump);
                } else
                    testEntity.SetComponentInfo(ComponentData.InputGesture, gesture);
            };

            leftPanel = new UIPanel(this, new UIRightMargin(Window.Ratio * 1f / 3f), new UITopMargin(0), new Vector2(Window.Ratio * 2f / 3f, 2));
            leftPanel.Click += ( ) => testEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Left);
            leftPanel.Release += ( ) => testEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Left);

            rightPanel = new UIPanel(this, new UIRightMargin(0), new UITopMargin(0), new Vector2(Window.Ratio * 2f / 3f, 2));
            rightPanel.Click += ( ) => testEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Right);
            rightPanel.Release += ( ) => testEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Right);
        }

        public override void Update (DeltaTime dt) {
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

        protected override void Activated ( ) {
            for(int i = 0; i < Entity.Entities.Count;i++)
                Entity.Entities[i].Prepare( );
            base.Activated( );
        }
    }
}