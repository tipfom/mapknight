using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace mapKnight.ToolKit.Controls.Animation {
    public class MLGCanvas : Canvas {
        private enum ClickedMouseButton {
            None,
            Left,
            Right,
            Middle,
        }

        public static event Action<BoneImage> SelectedBoneImageChanged;
        // to remember the clicked image
        private BoneImage clickedBoneImage;
        private RotateTransform clickedTransform;
        private ClickedMouseButton clickState;
        // to remember the delta
        private Point lastPosition;
        // rotation stuff
        private Point centerPoint;
        private Vector startVector;
        private double initialAngle;

        public MLGCanvas ( ) : base( ) {
            PreviewMouseDown += MLGCanvas_PreviewMouseDown;
            PreviewMouseMove += MLGCanvas_PreviewMouseMove;
            PreviewMouseUp += MLGCanvas_PreviewMouseUp;
            MouseLeave += MLGCanvas_MouseLeave;
        }

        private void MLGCanvas_MouseLeave (object sender, MouseEventArgs e) {
            ChangesCompleted( );
        }

        private void MLGCanvas_PreviewMouseUp (object sender, MouseButtonEventArgs e) {
            ChangesCompleted( );
        }

        private void ChangesCompleted ( ) {
            clickedBoneImage?.SetNoEffect( );
            switch (clickState) {
                case ClickedMouseButton.Left:
                    clickedBoneImage?.PositionChangeCompleted( );
                    break;

                case ClickedMouseButton.Right:
                    clickedBoneImage?.RotationChangeCompleted(clickedTransform.Angle);
                    break;
            }

            clickedBoneImage = null;
            clickState = ClickedMouseButton.None;
        }

        private void MLGCanvas_PreviewMouseMove (object sender, MouseEventArgs e) {
            if (clickedBoneImage != null) {
                Point newPosition = e.GetPosition(this);
                Point delta = new Point(newPosition.X - lastPosition.X, newPosition.Y - lastPosition.Y);
                switch (clickState) {
                    case ClickedMouseButton.Left:
                        // move
                        Canvas.SetLeft(clickedBoneImage, Canvas.GetLeft(clickedBoneImage) + delta.X);
                        Canvas.SetTop(clickedBoneImage, Canvas.GetTop(clickedBoneImage) + delta.Y);
                        lastPosition = newPosition;
                        break;

                    case ClickedMouseButton.Right:
                        // rotate
                        Vector deltaVector = Point.Subtract(newPosition, centerPoint);
                        double angle = Vector.AngleBetween(startVector, deltaVector);
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                            double step = Math.Round((initialAngle + angle) / 20d, 0);
                            clickedTransform.Angle = step * 18d;
                        } else {
                            clickedTransform.Angle = initialAngle + Math.Round(angle, 0);
                        }
                        clickedBoneImage.InvalidateMeasure( );
                        break;
                }
            }
        }

        private void MLGCanvas_PreviewMouseDown (object sender, MouseButtonEventArgs e) {
            foreach (UIElement element in Children) {
                BoneImage image = element as BoneImage;
                if (image != null) {
                    BoneImage.HitResult result = image.EvaluateHit(e.GetPosition(image));
                    if (result == BoneImage.HitResult.VisualChildHit) return;
                    if (result == BoneImage.HitResult.NoHit) continue;

                    clickedBoneImage = image;
                    clickedTransform = (RotateTransform)clickedBoneImage.RenderTransform;
                    e.Handled = true;
                    lastPosition = e.GetPosition(this);
                    SelectedBoneImageChanged?.Invoke(image);
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        // move
                        clickState = ClickedMouseButton.Left;
                        clickedBoneImage.SetMoveEffect( );
                        clickedBoneImage.PositionOrRotationChangeBegan( );
                    } else if (e.RightButton == MouseButtonState.Pressed) {
                        // rotate
                        clickState = ClickedMouseButton.Right;
                        centerPoint = clickedBoneImage.TranslatePoint(
                            new Point(clickedBoneImage.Width * clickedBoneImage.RenderTransformOrigin.X,
                                      clickedBoneImage.Height * clickedBoneImage.RenderTransformOrigin.Y),
                            this);
                        startVector = Point.Subtract(lastPosition, centerPoint);
                        initialAngle = clickedTransform.Angle;
                        clickedBoneImage.SetRotateEffect( );
                        clickedBoneImage.PositionOrRotationChangeBegan( );
                    } else if(e.MiddleButton == MouseButtonState.Pressed) {
                        // FLIPP
                    }
                    else {
                        clickState = ClickedMouseButton.None;
                    }
                    return;
                }
            }
        }

    }
}
