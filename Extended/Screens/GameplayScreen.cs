using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components;
using mapKnight.Extended.Components.AI;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using Map = mapKnight.Extended.Graphics.Map;

namespace mapKnight.Extended.Screens {

    public class GameplayScreen : Screen, PlayerComponent.IInputProvider {
        private UILabel debugLabel;
        private UIButton leftButton, rightButton, jumpButton;
        private Map map;
        private Entity testEntity;
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

            Entity.Configuration mobConfig = Assets.Load<Entity.Configuration>("potatoe_patrick2");
            mobConfig.Components.Add(new _2Component.Configuration( ));
            mobConfig.Create(new Vector2(7 + mobConfig.Transform.BoundsHalf.X, 5 + mobConfig.Transform.BoundsHalf.Y), map); // 9, 12
            mobConfig.Create(new Vector2(7 + mobConfig.Transform.BoundsHalf.X, 7 + mobConfig.Transform.BoundsHalf.Y), map); // 9, 12
            mobConfig.Create(new Vector2(7 + mobConfig.Transform.BoundsHalf.X, 10 + mobConfig.Transform.BoundsHalf.Y), map); // 9, 12

            Entity.Configuration mobConfig2 = Assets.Load<Entity.Configuration>("potatoe_patrick");
            mobConfig2.Components.Add(new _1Component.Configuration( ) { ScaredToFall = true });
            mobConfig2.Create(new Vector2(9, 12 + mobConfig2.Transform.BoundsHalf.Y), map);

            Entity.Configuration testEntityConfig = Assets.Load<Entity.Configuration>("potatoe_patrick");
            testEntityConfig.Components.Add(new PlayerComponent.Configuration(this));
            testEntity = testEntityConfig.Create(new Vector2(5 + testEntityConfig.Transform.BoundsHalf.X, 13 + testEntityConfig.Transform.BoundsHalf.Y), map);
            map.Focus(testEntity.ID);

            base.Load( );
        }

        public override void Update (DeltaTime dt) {
            map.Update(dt);
            base.Update(dt);
            debugLabel.Text = $"frame: {Manager.FrameTime.TotalMilliseconds:00.0}\n" +
                                $"update: {Manager.UpdateTime.TotalMilliseconds:00.0}\n" +
                                $"draw: {Manager.DrawTime.TotalMilliseconds:00.0}";
        }

        protected override void Activated ( ) {
            foreach (Entity entity in Entity.Entities)
                entity.Prepare( );
            base.Activated( );
        }
    }
}