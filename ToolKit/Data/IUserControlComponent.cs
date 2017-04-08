using System;
using System.Windows.Controls;
using mapKnight.Core;

namespace mapKnight.ToolKit.Data {
    interface IUserControlComponent {
        UserControl Control { get; }
        Action<Func<Vector2, bool>> RequestMapVectorList { get; set; }
    }
}
