using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Components;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using Map = mapKnight.Extended.Graphics.Map;

namespace mapKnight.Extended.Screens {
    public class GameplayScreen : Screen, UserControlComponent.IInputProvider {
        Map map;
        Entity testEntity;
        UIButton leftButton, rightButton, jumpButton;
        UILabel debugLabel;

        public bool Jump { get { return jumpButton.Clicked; } }

        public bool Left { get { return leftButton.Clicked; } }

        public bool Right { get { return rightButton.Clicked; } }

        public override void Load ( ) {
            jumpButton = new UIButton(this, new UILeftMargin(0.05f), new UIBottomMargin(0.05f), new Vector2(0.5f, 0.5f), "J");
            leftButton = new UIButton(this, new UIRightMargin(0.5f), new UIBottomMargin(0.1f), new Vector2(0.4f, 0.4f), "L");
            rightButton = new UIButton(this, new UIRightMargin(0.05f), new UIBottomMargin(0.1f), new Vector2(0.4f, 0.4f), "R");
            debugLabel = new UILabel(this, new UIRightMargin(0.1f), new UITopMargin(0.05f), 0.05f, "", UITextAlignment.Right);

            map = Assets.Load<Map>("testMap");

            Entity.Configuration mobConfig = Assets.Load<Entity.Configuration>("potatoe_patrick");
            mobConfig.Components.Add(new Components.AI._1Component.Configuration( ) { ScaredToFall = true });
            mobConfig.Create(new Core.Vector2(5, 5), map);


            Entity.Configuration testEntityConfig = Assets.Load<Entity.Configuration>("potatoe_patrick");
            testEntityConfig.Components.Add(new UserControlComponent.Configuration(this));
            testEntity = testEntityConfig.Create(new Core.Vector2(5, 5), map);
            map.Focus(testEntity.ID);

            base.Load( );
        }

        protected override void Activated ( ) {
            foreach (Entity entity in Entity.Entities)
                entity.Prepare( );
            base.Activated( );
        }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
        }

        public override void Tick ( ) {
            foreach (Entity entity in Entity.Entities)
                entity.Tick( );
        }

        public override void Update (TimeSpan dt) {
            map.Update(dt);
            base.Update(dt);
            debugLabel.Text = $"frame: {Manager.FrameTime.TotalMilliseconds:00.0}\n" +
                                $"update: {Manager.UpdateTime.TotalMilliseconds:00.0}\n" +
                                $"draw: {Manager.DrawTime.TotalMilliseconds:00.0}";
        }
    }
}
