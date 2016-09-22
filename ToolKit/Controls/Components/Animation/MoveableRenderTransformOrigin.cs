﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace mapKnight.ToolKit.Controls.Components.Animation {
    public class MoveableRenderTransformOrigin : Thumb {
        public event Action<Point> RenderTransformOriginChanged;

        public MoveableRenderTransformOrigin ( ) {
            DragDelta += MoveableRenderTransformOrigin_DragDelta;
            DataContextChanged += MoveableRenderTransformOrigin_DataContextChanged;
        }

        private void MoveableRenderTransformOrigin_DataContextChanged (object sender, DependencyPropertyChangedEventArgs e) {
            ResizableImage item = DataContext as ResizableImage;
            Margin = new Thickness(item.RenderTransformOrigin.X * item.RenderSize.Width - Width / 2d, item.RenderTransformOrigin.Y * item.RenderSize.Height - Height / 2d, 0, 0);
            item.SizeChanged += DataContext_SizeChanged;
        }

        private void DataContext_SizeChanged (object sender, SizeChangedEventArgs e) {
            ResizableImage item = DataContext as ResizableImage;
            Margin = new Thickness(item.RenderTransformOrigin.X * item.RenderSize.Width - Width / 2d, item.RenderTransformOrigin.Y * item.RenderSize.Height - Height / 2d, 0, 0);
        }

        private void MoveableRenderTransformOrigin_DragDelta (object sender, DragDeltaEventArgs e) {
            ResizableImage item = DataContext as ResizableImage;
            if (item != null) {
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

                RotateTransform rotateTransform = item.RenderTransform as RotateTransform;
                if (rotateTransform != null) {
                    dragDelta = rotateTransform.Transform(dragDelta);
                }

                double x = Math.Round(Math.Min(1, Math.Max(0, item.RenderTransformOrigin.X + dragDelta.X / item.Width)) * item.Image.PixelWidth) / item.Image.PixelWidth;
                double y = Math.Round(Math.Min(1, Math.Max(0, item.RenderTransformOrigin.Y + dragDelta.Y / item.Height)) * item.Image.PixelHeight) / item.Image.PixelHeight;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                    x = Math.Round(x * 6d) / 6d;
                    y = Math.Round(y * 6d) / 6d;
                }
                item.RenderTransformOrigin = new Point(x, y);
                Margin = new Thickness(x * item.Width - Width / 2d, y * item.Height - Height / 2d, 0, 0);
                RenderTransformOriginChanged?.Invoke(item.RenderTransformOrigin);
            }
        }
    }
}
