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

        private Map map;
        private Entity testEntity;
        private HealthComponent testEntityHealth;

        public GameplayScreen ( ) {
            Entity.EntityAdded += (Entity obj) => { obj.Prepare( ); };
        }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
        }

        public override void Load ( ) {
            SetupControls( );
            Window.Changed += ( ) => {
                SetupControls( );
            };

            debugLabel = new UILabel(this, new UIRightMargin(0.1f), new UITopMargin(0.05f), 0.05f, "", UITextAlignment.Right);

            int begin = Environment.TickCount;
            map = Assets.Load<Map>("beatiful_map");
            Debug.Print(this, $"map loading took {Environment.TickCount - begin} ms");
            begin = Environment.TickCount;

            //Entity.Configuration sawConfig = Assets.Load<Entity.Configuration>("circularsaw");
            Entity.Configuration walkingTrowieConfig = Assets.Load<Entity.Configuration>("plugger");
            Entity.Configuration landMineConfig = Assets.Load<Entity.Configuration>("landmine");
            Entity.Configuration turretConfig = Assets.Load<Entity.Configuration>("tourret");
            //Entity.Configuration meatballConfig = Assets.Load<Entity.Configuration>("meatball");
            Entity.Configuration hastoConfig = Assets.Load<Entity.Configuration>("shell");
            Entity.Configuration platformConfig = Assets.Load<Entity.Configuration>("platforms/copper");
            Entity.Configuration seplingConfig = Assets.Load<Entity.Configuration>("sepling");
            Entity.Configuration sharkConfig = Assets.Load<Entity.Configuration>("shark");

            //sawConfig.Create(new Vector2(3, 6), map);

            Entity.Configuration testPrivate = new Entity.Configuration( );
            testPrivate.Name = "Testing Private";
            testPrivate.Transform = new Transform(Vector2.Zero, new Vector2(1.785714f, 1.5f));
            testPrivate.Components = new ComponentList( );
            testPrivate.Components.Add(new MotionComponent.Configuration( ));
            testPrivate.Components.Add(new Components.AI.Guardian.PrivateComponent.Configuration( ));
            testPrivate.Components.Add(new Components.Graphics.TextureComponent.Configuration( ) { Texture = "guardian/private1" });
            testPrivate.Components.Add(new Components.Graphics.SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } });
            testPrivate.Components.Add(new Components.Stats.SpeedComponent.Configuration( ) { X = 1.2f });
            testPrivate.Components.Add(new HealthComponent.Configuration( ) { Value = 1 });

            Entity.Configuration testPrivate2 = new Entity.Configuration( );
            testPrivate2.Name = "Testing Private";
            testPrivate2.Transform = new Transform(Vector2.Zero, new Vector2(1.785714f, 1.5f));
            testPrivate2.Components = new ComponentList( );
            testPrivate2.Components.Add(new MotionComponent.Configuration( ));
            testPrivate2.Components.Add(new Components.AI.Guardian.PrivateComponent.Configuration( ));
            testPrivate2.Components.Add(new Components.Graphics.TextureComponent.Configuration( ) { Texture = "guardian/private2" });
            testPrivate2.Components.Add(new Components.Graphics.SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } });
            testPrivate2.Components.Add(new Components.Stats.SpeedComponent.Configuration( ) { X = 1.2f });
            testPrivate2.Components.Add(new HealthComponent.Configuration( ) { Value = 1 });

            Entity.Configuration testPrivate3 = new Entity.Configuration( );
            testPrivate3.Name = "Testing Private";
            testPrivate3.Transform = new Transform(Vector2.Zero, new Vector2(1.785714f, 1.5f));
            testPrivate3.Components = new ComponentList( );
            testPrivate3.Components.Add(new MotionComponent.Configuration( ));
            testPrivate3.Components.Add(new Components.AI.Guardian.PrivateComponent.Configuration( ));
            testPrivate3.Components.Add(new Components.Graphics.TextureComponent.Configuration( ) { Texture = "guardian/private3" });
            testPrivate3.Components.Add(new Components.Graphics.SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } });
            testPrivate3.Components.Add(new Components.Stats.SpeedComponent.Configuration( ) { X = 1.2f });
            testPrivate3.Components.Add(new HealthComponent.Configuration( ) { Value = 1 });

            Entity.Configuration testOfficer = new Entity.Configuration( );
            testOfficer.Name = "Testing Officer";
            testOfficer.Transform = new Transform(Vector2.Zero, new Vector2(1.785714f, 1.5f));
            testOfficer.Components = new ComponentList( );
            testOfficer.Components.Add(new MotionComponent.Configuration( ));
            testOfficer.Components.Add(new Components.AI.Guardian.OfficerComponent.Configuration( ));
            testOfficer.Components.Add(new Components.Graphics.TextureComponent.Configuration( ) { Texture = "guardian/officer" });
            testOfficer.Components.Add(new Components.Graphics.SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } });
            testOfficer.Components.Add(new SpeedComponent.Configuration( ) { X = 1.2f });
            testOfficer.Components.Add(new HealthComponent.Configuration( ) { Value = 1 });

            Entity.Configuration tentConfig = new Entity.Configuration( );
            tentConfig.Name = "Testing Tent";
            tentConfig.Transform = new Transform(Vector2.Zero, new Vector2(3.405405405f, 2f));
            tentConfig.Components = new ComponentList( );
            tentConfig.Components.Add(new Components.AI.Guardian.TentComponent.Configuration( ) {
                Officer = testOfficer, PatrolRange = 12f, PrivateCount = 4, TimeBetweenPrivates = 3000,
                Privates = new Entity.Configuration[ ] {
                    testPrivate, testPrivate2, testPrivate3
                }
            });
            tentConfig.Components.Add(new Components.Graphics.TextureComponent.Configuration( ) { Texture = "guardian/tent" });
            tentConfig.Components.Add(new Components.Graphics.DrawComponent.Configuration( ));
            tentConfig.Components.Add(new Components.Graphics.SkeletComponent.Configuration( ) { Bones = new Rectangle[ ] { new Rectangle( ) { Size = Vector2.One, Position = Vector2.Zero } } });
            tentConfig.Create(new Vector2(26, 18 + tentConfig.Transform.HalfSize.Y), map);

            walkingTrowieConfig.Create(new Vector2(72, 10 + walkingTrowieConfig.Transform.HalfSize.Y), map);

            landMineConfig.Create(new Vector2(21, 7 + landMineConfig.Transform.HalfSize.Y), map);
            landMineConfig.Create(new Vector2(22, 7 + landMineConfig.Transform.HalfSize.Y), map);
            landMineConfig.Create(new Vector2(2.5f, 8 + landMineConfig.Transform.HalfSize.Y), map);

            turretConfig.Create(new Vector2(62, 12 + turretConfig.Transform.HalfSize.Y), map);

            //meatballConfig.Create(new Vector2(3, 10), map);

            hastoConfig.Create(new Vector2(42, 11 + hastoConfig.Transform.HalfSize.Y), map);

            platformConfig.Create(new Vector2(3, 9), map);

            seplingConfig.Create(map.SpawnPoint + new Vector2(10, 1), map);

            sharkConfig.Create(map.SpawnPoint + new Vector2(10, 1), map);

            Entity.Configuration playerConfig = Assets.Load<Entity.Configuration>("player");
            testEntity = playerConfig.Create(map.SpawnPoint, map);
            testEntityHealth = testEntity.GetComponent<HealthComponent>( );
            map.Focus(testEntity.ID);
            Debug.Print(this, $"player loading took {Environment.TickCount - begin} ms");

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
            }
            debugLabel.Text = $"frame: {Manager.FrameTime.TotalMilliseconds:00.0}\n" +
                              $"update: {Manager.UpdateTime.TotalMilliseconds:00.0}\n" +
                            $"draw: {Manager.DrawTime.TotalMilliseconds:00.0}\n" +
                          $"health: {testEntityHealth.Current:00.0} ({testEntityHealth.Initial:00.0})";
        }

        protected override void Activated ( ) {
            for(int i = 0; i < Entity.Entities.Count;i++)
                Entity.Entities[i].Prepare( );
            base.Activated( );
        }
    }
}