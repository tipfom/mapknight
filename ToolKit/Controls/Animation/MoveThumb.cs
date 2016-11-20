using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace mapKnight.ToolKit.Controls.Components.Animation {
   public class MoveThumb : Thumb {
        private enum Mode : byte {
            Rotate = 0,
            Move = 1
        }

        private Point centerPoint;
        private Vector startVector;
        private double initialAngle;
        private Canvas canvas;
        private Control item;
        private RotateTransform rotateTransform;
        private Mode mode;

        public MoveThumb ( ) {
            DragDelta += MoveThumb_DragDelta;
            DragStarted += MoveThumb_DragStarted;
            DataContextChanged += MoveThumb_DataContextChanged;
        }

        private void MoveThumb_DataContextChanged (object sender, DependencyPropertyChangedEventArgs e) {
            item = (Control)DataContext;
        }

        private void MoveThumb_DragStarted (object sender, DragStartedEventArgs e) {
            mode = Keyboard.IsKeyDown(Key.R) ? Mode.Rotate : Mode.Move;
            if(mode == Mode.Rotate && item != null) {
                canvas = VisualTreeHelper.GetParent(item) as Canvas;
                if(canvas != null) {
                    centerPoint = item.TranslatePoint(
                        new Point(item.Width * item.RenderTransformOrigin.X,
                                  item.Height * item.RenderTransformOrigin.Y),
                                  canvas);

                    Point startPoint = Mouse.GetPosition(canvas);
                    startVector = Point.Subtract(startPoint, this.centerPoint);

                    rotateTransform = item.RenderTransform as RotateTransform;
                    if (rotateTransform == null) {
                        item.RenderTransform = new RotateTransform(0);
                        initialAngle = 0;
                    } else {
                        initialAngle = rotateTransform.Angle;
                    }
                }
            }
        }

        private void MoveThumb_DragDelta (object sender, DragDeltaEventArgs e) {
            if (item == null) return;
            RotateTransform rotateTransform = item.RenderTransform as RotateTransform;

            switch (mode) {
                case Mode.Move:
                    Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

                    if (rotateTransform != null) {
                        dragDelta = rotateTransform.Transform(dragDelta);
                    }

                    Canvas.SetLeft(item, Canvas.GetLeft(item) + dragDelta.X);
                    Canvas.SetTop(item, Canvas.GetTop(item) + dragDelta.Y);
                    break;
                case Mode.Rotate:
                    if (canvas == null) break;
                    Point currentPoint = Mouse.GetPosition(canvas);
                    Vector deltaVector = Point.Subtract(currentPoint, centerPoint);

                    double angle = Vector.AngleBetween(startVector, deltaVector);

                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                        double step = Math.Round((initialAngle + angle) / 20d, 0);
                        rotateTransform.Angle = step * 18d;
                    } else {
                        rotateTransform.Angle = initialAngle + Math.Round(angle, 0);
                    }
                    item.InvalidateMeasure( );
                    break;
            }

            if (item != null) {
                
            }
        }
    }
}
