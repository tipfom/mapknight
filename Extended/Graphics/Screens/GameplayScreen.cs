using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Extended.Components;
using mapKnight.Extended.Components.Configs;

namespace mapKnight.Extended.Graphics.Screens {
    public class GameplayScreen : Screen, UserControlComponent.IInputProvider {
        Map map;
        Entity testEntity;
        GUI.GUIButton leftButton, rightButton, jumpButton;

        public bool Jump { get { return jumpButton.Clicked; } }

        public bool Left { get { return leftButton.Clicked; } }

        public bool Right { get { return rightButton.Clicked; } }

        public GameplayScreen ( ) {
            jumpButton = new GUI.GUIButton("J", new Core.Rectangle(-1f, 0, 0.3f, 0.3f));
            leftButton = new GUI.GUIButton("L", new Core.Rectangle(-1.6f, 0f, 0.3f, 0.3f));
            rightButton = new GUI.GUIButton("R", new Core.Rectangle(-1.3f, 0f, 0.3f, 0.3f));

            Interface.Add(jumpButton);
            Interface.Add(leftButton);
            Interface.Add(rightButton);

            map = Assets.Load<Map>("testMap");
            EntityConfig testEntityConfig = Assets.Load<EntityConfig>("potatoe_patrick");
            testEntityConfig.Components.Add(new UserControlComponentConfig(this));
            testEntity = testEntityConfig.Create(new Core.Vector2(5, 5), map);
            map.Focus(testEntity.ID);
        }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
        }

        public override void Update (TimeSpan time) {
            map.Update(time, 1);
            base.Update(time);
        }
    }
}
