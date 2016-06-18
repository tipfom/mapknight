using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.Screens
{
    public class GameplayScreen : Screen
    {
        Map map;

        public GameplayScreen ( ) {
            map = Assets.Load<Map>("test");
            map.UpdateTextureBuffer(0, map.Height - map.DrawSize.Height);
        }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
        }
    }
}
