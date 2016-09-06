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

            map = Assets.Load<Map>("beatiful_map");
            
            Entity.Configuration sawConfig = Assets.Load<Entity.Configuration>("circularsaw");
            Entity.Configuration walkingTrowieConfig = Assets.Load<Entity.Configuration>("walking_trowie");
            Entity.Configuration landMineConfig = Assets.Load<Entity.Configuration>("landmine");
            Entity.Configuration turretConfig = Assets.Load<Entity.Configuration>("tourret");
            Entity.Configuration meatballConfig = Assets.Load<Entity.Configuration>("meatball");
            Entity.Configuration hastoConfig = Assets.Load<Entity.Configuration>("hasto");

            //sawConfig.Create(new Vector2(10, 2), map);

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