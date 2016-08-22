using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace mapKnight.ToolKit.Controls.Components {

    public interface IComponentControl {
        List<Control> Menu { get; }
    }
}