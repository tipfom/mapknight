using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace mapKnight.ToolKit.Controls.Components.Animation {
   public class MoveThumb : Thumb {
        public MoveThumb ( ) {
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragDelta (object sender, DragDeltaEventArgs e) {
            Control item = (Control)DataContext;
            if(item != null) {
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

                RotateTransform rotateTransform = item.RenderTransform as RotateTransform;
                if (rotateTransform != null) {
                    dragDelta = rotateTransform.Transform(dragDelta);
                }

                Canvas.SetLeft(item, Canvas.GetLeft(item) + dragDelta.X);
                Canvas.SetTop(item, Canvas.GetTop(item) + dragDelta.Y);
            }
        }
    }
}
