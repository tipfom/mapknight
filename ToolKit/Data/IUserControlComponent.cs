using System;
using System.Collections.Generic;
using System.Windows.Controls;
using mapKnight.Core;
using mapKnight.Core.World.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace mapKnight.ToolKit.Data {
    interface IUserControlComponent {
        UserControl Control { get; }
        Action<Func<Vector2, bool>> RequestMapVectorList { get; set; }
        event Action RequestRender;

        void Render(SpriteBatch spriteBatch,int offsetx, int offsety, int tilesize);
        IEnumerable<Tuple<DataID, DataType, object>> CollectData( );
    }
}
