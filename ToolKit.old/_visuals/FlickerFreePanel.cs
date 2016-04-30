using System;
using System.Drawing;
using System.Windows.Forms;

namespace mapKnight.ToolKit {
    class FlickerFreePanel : Panel {
        bool clearedOnce = false;

        public FlickerFreePanel() {
            SizeChanged += (object sender, EventArgs e) => { clearedOnce = false; };
        }

        protected override void OnPaintBackground (PaintEventArgs e) {
            if(!clearedOnce) {
                e.Graphics.Clear (Color.White);
                clearedOnce = true;
            }
        }
    }
}
