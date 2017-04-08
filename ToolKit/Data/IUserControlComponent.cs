using System;
using System.Collections.Generic;
using System.Windows.Controls;
using mapKnight.Core;

namespace mapKnight.ToolKit.Data {
    interface IUserControlComponent {
        UserControl Control { get; }
        Action<Action<List<Vector2>>> RequestMapVectorList { get; set; }
    }
}
