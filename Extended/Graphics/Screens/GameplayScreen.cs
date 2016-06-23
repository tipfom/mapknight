using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.Screens {
    public class GameplayScreen : Screen {
        Map map;
        Entity testEntity;

        public GameplayScreen ( ) {
            map = Assets.Load<Map>("testMap");
            testEntity = Assets.Load<EntityConfig>("potatoe_patrick").Create(new Core.Vector2(0, 0), map);
            testEntity.Transform.TranslateY(map.Height - map.DrawSize.Height  - 3);
            map.Focus(testEntity.ID);
        }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
        }

        public override void Update (TimeSpan time) {
            map.Update(time.Milliseconds, 1);
            testEntity.Transform.Translate(testEntity.Transform.Center + new Core.Vector2(0, 0.01f));
            base.Update(time);
        }
    }
}
