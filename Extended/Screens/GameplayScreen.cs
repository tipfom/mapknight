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
        private Entity playerEntity;
        private PlayerComponent playerComponent;
        private string mapName;

        public GameplayScreen(string mapName) {
            this.mapName = mapName;
        }

        public override void Draw( ) {
            map.Draw( );
            base.Draw( );
        }

        public override void Load( ) {
            map = Assets.Load<Map>(mapName);
            for (int i = 0; i < map.Entities.Count; i++) {
                map.Entities[i].Prepare( );
            }
            map.EntityAdded += (Entity obj) => { obj.Prepare( ); };

            playerEntity = EntityCollection.Players.Diamond.Create(map.SpawnPoint, map);
            playerComponent = playerEntity.GetComponent<PlayerComponent>( );
            map.Focus(playerEntity.ID);

            debugLabel = new UILabel(this, new UIRightMargin(0.1f), new UITopMargin(0.075f), 0.05f, "", UITextAlignment.Right);
            healthBar = new UIBar(this, new Color(255, 0, 0, 127), new Color(255, 255, 255, 63), playerComponent.Health, new UILeftMargin(0), new UITopMargin(0), new RelativeSize(1f, 0.025f), UIDepths.MIDDLE);

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
                    if (playerEntity.GetComponent<MotionComponent>( ).IsOnGround || playerEntity.GetComponent<MotionComponent>( ).IsOnPlatform)
                        playerEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Jump);
                } else
                    playerEntity.SetComponentInfo(ComponentData.InputGesture, gesture);
            };

            leftPanel = new UIPanel(this, new UIRightMargin(Window.Ratio * 2f / 5f), new UITopMargin(0), new RelativeSize(1f / 5f, 1f));
            leftPanel.Click += ( ) => playerEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Left);
            leftPanel.Release += ( ) => playerEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Left);
            leftPanel.Leave += ( ) => playerEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Left);

            rightPanel = new UIPanel(this, new UIRightMargin(0), new UITopMargin(0), new RelativeSize(1f / 5f, 1f));
            rightPanel.Click += ( ) => playerEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Right);
            rightPanel.Release += ( ) => playerEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Right);
            rightPanel.Leave += ( ) => playerEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Right);
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
    }
}