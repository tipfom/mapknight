﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace mapKnight.ToolKit.Controls.Animation {
    public class AnimationCanvas : Canvas {
        private enum ClickedMouseButton {
            None,
            Left,
            Right,
            Middle,
        }

        public static event Action<BoneImage> SelectedBoneImageChanged;

        public bool UnlockRotation = false;

        // to remember the clicked image
        private BoneImage clickedBoneImage;
        private RotateTransform clickedTransform;
        private ClickedMouseButton clickState;
        // to remember the delta
        private Point lastPosition;
        // rotation stuff
        private Point centerPoint;
        private Vector startVector;
        private Vector currentVector;
        private double initialAngle;

        public AnimationCanvas ( ) : base( ) {
            PreviewMouseDown += AnimationCanvas_PreviewMouseDown;
            PreviewMouseMove += AnimationCanvas_PreviewMouseMove;
            PreviewMouseUp += (sender, e) => ChangesCompleted( );
            MouseLeave += (sender, e) => ChangesCompleted( );
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

        private void AnimationCanvas_PreviewMouseMove (object sender, MouseEventArgs e) {
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
                        if (UnlockRotation) {
                            double angle = Vector.AngleBetween(currentVector, deltaVector);
                            clickedTransform.Angle += Math.Round(angle, 0);
                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                                clickedTransform.Angle = initialAngle + Math.Round((clickedTransform.Angle - initialAngle) / 20d, 0) * 20d;
                            }
                            Matrix m = Matrix.Identity;
                            m.Rotate(clickedTransform.Angle - initialAngle);
                            currentVector = m.Transform(startVector);
                        } else {
                            double angle = Vector.AngleBetween(startVector, deltaVector);
                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                                double step = Math.Round((initialAngle + angle) / 20d, 0);
                                clickedTransform.Angle = step * 18d;
                            } else {
                                clickedTransform.Angle = initialAngle + Math.Round(angle, 0);
                            }
                        }
                        clickedBoneImage.InvalidateMeasure( );
                        break;
                }
            }
        }

        private void AnimationCanvas_PreviewMouseDown (object sender, MouseButtonEventArgs e) {
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
                        if (Keyboard.IsKeyDown(Key.I)) {
                            while (clickedTransform.Angle < 0) clickedTransform.Angle += 360;
                            clickedTransform.Angle %= 360;
                        }
                    } else if (e.MiddleButton == MouseButtonState.Pressed) {
                        // FLIPP
                    } else {
                        clickState = ClickedMouseButton.None;
                    }
                    return;
                }
            }
        }

    }
}