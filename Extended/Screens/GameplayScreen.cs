using System;
using mapKnight.Core;
using mapKnight.Extended.Components.Movement;
using mapKnight.Extended.Components.Stats;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using Map = mapKnight.Extended.Graphics.Map;

namespace mapKnight.Extended.Screens {

    public class GameplayScreen : Screen, PlayerComponent.IInputProvider {
        private const int MAX_TIME_BETWEEN_UPDATES = 100;

        private UILabel debugLabel;
        private UIPanel leftPanel, rightPanel;
        private UIGesturePanel controlPanel;

        private Map map;
        private Entity testEntity;
        private HealthComponent testEntityHealth;
        private bool _Jump;

        public GameplayScreen ( ) {
            Entity.EntityAdded += (Entity obj) => { if (IsActive) obj.Prepare( ); };
        }

        public bool Jump { get { if (_Jump) { _Jump = false; return true; } return false; } }

        public bool Left { get { return leftPanel.Clicked; } }

        public bool Right { get { return rightPanel.Clicked; } }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
        }

        public override void Load ( ) {
            controlPanel = new UIGesturePanel(this, new UILeftMargin(0), new UITopMargin(0), new Vector2(Window.Ratio * 4f / 3f, 2), Assets.GetGestureStore("gestures"));
            leftPanel = new UIPanel(this, new UILeftMargin(Window.Ratio * 1f / 3f), new UITopMargin(0), new Vector2(Window.Ratio * 1f / 3f, 2));
            rightPanel = new UIPanel(this, new UILeftMargin(0), new UITopMargin(0), new Vector2(Window.Ratio * 1f / 3f, 2));
            Window.Changed += () => {
                controlPanel.Dispose( );
                leftPanel.Dispose( );
                rightPanel.Dispose( );
                controlPanel = new UIGesturePanel(this, new UILeftMargin(0), new UITopMargin(0), new Vector2(Window.Ratio * 4f / 3f, 2), Assets.GetGestureStore("gestures"));
                controlPanel.OnGesturePerformed += (string gesture) => {
                    if(gesture == "Martin") {
                        _Jump = true;
                    } else {
                        global::Android.Widget.Toast.MakeText(Assets.Context, gesture, global::Android.Widget.ToastLength.Short).Show( );
                    }
                };
                leftPanel = new UIPanel(this, new UIRightMargin(Window.Ratio * 1f / 3f), new UITopMargin(0), new Vector2(Window.Ratio * 2f / 3f, 2));
                rightPanel = new UIPanel(this, new UIRightMargin(0), new UITopMargin(0), new Vector2(Window.Ratio * 2f / 3f, 2));
            };

            debugLabel = new UILabel(this, new UIRightMargin(0.1f), new UITopMargin(0.05f), 0.05f, "", UITextAlignment.Right);

            map = Assets.Load<Map>("beatiful_map");
            
            Entity.Configuration sawConfig = Assets.Load<Entity.Configuration>("circularsaw");
            Entity.Configuration walkingTrowieConfig = Assets.Load<Entity.Configuration>("walking_trowie");
            Entity.Configuration landMineConfig = Assets.Load<Entity.Configuration>("landmine");
            Entity.Configuration turretConfig = Assets.Load<Entity.Configuration>("tourret");
            Entity.Configuration meatballConfig = Assets.Load<Entity.Configuration>("meatball");
            Entity.Configuration hastoConfig = Assets.Load<Entity.Configuration>("hasto");

            sawConfig.Create(new Vector2(3, 6), map);

            walkingTrowieConfig.Create(new Vector2(72, 10 + walkingTrowieConfig.Transform.HalfSize.Y), map);

            landMineConfig.Create(new Vector2(21, 7 + landMineConfig.Transform.HalfSize.Y), map);
            landMineConfig.Create(new Vector2(22, 7 + landMineConfig.Transform.HalfSize.Y), map);

            turretConfig.Create(new Vector2(62, 12 + turretConfig.Transform.HalfSize.Y), map);

            meatballConfig.Create(new Vector2(3, 10), map);

            hastoConfig.Create(new Vector2(42, 11 + hastoConfig.Transform.HalfSize.Y), map);

            Entity.Configuration playerConfig = Assets.Load<Entity.Configuration>("player");
            playerConfig.Components.Add(new PlayerComponent.Configuration(this));
            testEntity = playerConfig.Create(map.SpawnPoint, map);
            testEntityHealth = testEntity.GetComponent<HealthComponent>( );
            map.Focus(testEntity.ID);

            base.Load( );
        }
        
        public override void Update (DeltaTime dt) {
            if (Math.Abs(Manager.FrameTime.Milliseconds) < MAX_TIME_BETWEEN_UPDATES) {
                map.Update(dt);
                base.Update(dt);
            }
            debugLabel.Text = $"frame: {Manager.FrameTime.TotalMilliseconds:00.0}\n" +
                              $"update: {Manager.UpdateTime.TotalMilliseconds:00.0}\n" +
                            $"draw: {Manager.DrawTime.TotalMilliseconds:00.0}\n" +
                          $"health: {testEntityHealth.Current:00.0} ({testEntityHealth.Initial:00.0})";
        }

        protected override void Activated ( ) {
            foreach (Entity entity in Entity.Entities)
                entity.Prepare( );
            base.Activated( );
        }
    }
}