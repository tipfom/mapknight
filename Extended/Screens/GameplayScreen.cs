using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components;
using mapKnight.Extended.Components.AI;
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
        private UIButton leftButton, rightButton, jumpButton;

        private Map map;
        private Entity testEntity;
        private HealthComponent testEntityHealth;

        public GameplayScreen ( ) {
            Entity.EntityAdded += (Entity obj) => { if (IsActive) obj.Prepare( ); };
        }

        public bool Jump { get { return jumpButton.Clicked; } }

        public bool Left { get { return leftButton.Clicked; } }

        public bool Right { get { return rightButton.Clicked; } }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
        }

        public override void Load ( ) {
            jumpButton = new UIButton(this, new UILeftMargin(0.05f), new UIBottomMargin(0.05f), new Vector2(0.5f, 0.5f), "J");
            leftButton = new UIButton(this, new UIRightMargin(0.5f), new UIBottomMargin(0.1f), new Vector2(0.4f, 0.4f), "L");
            rightButton = new UIButton(this, new UIRightMargin(0.05f), new UIBottomMargin(0.1f), new Vector2(0.4f, 0.4f), "R");
            debugLabel = new UILabel(this, new UIRightMargin(0.1f), new UITopMargin(0.05f), 0.05f, "", UITextAlignment.Right);

            map = Assets.Load<Map>("testMap");

            //Entity.Configuration mobConfig = Assets.Load<Entity.Configuration>("potatoe_patrick2");
            //mobConfig.Components.Add(new _2Component.Configuration( ));
            //mobConfig.Create(new Vector2(7 + mobConfig.Transform.BoundsHalf.X, 5 + mobConfig.Transform.BoundsHalf.Y), map); // 9, 12
            //mobConfig.Create(new Vector2(7 + mobConfig.Transform.BoundsHalf.X, 7 + mobConfig.Transform.BoundsHalf.Y), map); // 9, 12
            //mobConfig.Create(new Vector2(7 + mobConfig.Transform.BoundsHalf.X, 10 + mobConfig.Transform.BoundsHalf.Y), map); // 9, 12

            //Entity.Configuration mobConfig2 = Assets.Load<Entity.Configuration>("potatoe_patrick");
            //mobConfig2.Components.Add(new _1Component.Configuration( ) { ScaredToFall = true });
            //mobConfig2.Create(new Vector2(9, 12 + mobConfig2.Transform.BoundsHalf.Y), map);

            Entity.Configuration sawConfig = Assets.Load<Entity.Configuration>("circularsaw");
            //Entity.Configuration standingTrowieConfig = Assets.Load<Entity.Configuration>("standing_trowie");
            Entity.Configuration walkingTrowieConfig = Assets.Load<Entity.Configuration>("walking_trowie");
            Entity.Configuration landMineConfig = Assets.Load<Entity.Configuration>("landmine");
            Entity.Configuration turretConfig = Assets.Load<Entity.Configuration>("tourret");
            Entity.Configuration meatballConfig = Assets.Load<Entity.Configuration>("meatball");
            Entity.Configuration hastoConfig = Assets.Load<Entity.Configuration>("hasto");

            sawConfig.Create(new Vector2(10, 2), map);

            //standingTrowieConfig.Create(new Vector2(8, 16), map);
            walkingTrowieConfig.Create(new Vector2(40, 10), map);

            landMineConfig.Create(new Vector2(10, 1 + landMineConfig.Transform.HalfSize.Y), map);

            turretConfig.Create(new Vector2(70, 3), map);

            meatballConfig.Create(new Vector2(4, 14), map);

            hastoConfig.Create(new Vector2(34, 2), map);

            Entity.Configuration playerConfig = Assets.Load<Entity.Configuration>("player");
            playerConfig.Components.Add(new PlayerComponent.Configuration(this));
            testEntity = playerConfig.Create(new Vector2(8 + playerConfig.Transform.HalfSize.X, 13 + playerConfig.Transform.HalfSize.Y), map);
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