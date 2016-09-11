using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;

namespace mapKnight.ToolKit.Controls.Components.Animation {
    public class ResizeThumb : Thumb {
        public bool SnapRatioOnShiftDown { get; set; }
        public ResizeThumb ( ) {
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragDelta (object sender, DragDeltaEventArgs e) {
            ResizableImage item = (this.DataContext as Control).DataContext as ResizableImage;

            if (item != null) {
                double deltaVertical, deltaHorizontal;
                bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                switch (VerticalAlignment) {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, item.ActualHeight - item.MinHeight);
                        if (SnapRatioOnShiftDown && shiftPressed) {
                            double right = Canvas.GetLeft(item) + item.Width, left = Canvas.GetLeft(item), top = Canvas.GetTop(item);
                            item.Height -= deltaVertical;
                            item.Width = item.Height * item.Image.Width / item.Image.Height;
                            Canvas.SetTop(item, top);
                            switch (HorizontalAlignment) {
                                case HorizontalAlignment.Left:
                                    Canvas.SetLeft(item, right - item.Width);
                                    break;
                                default:
                                    Canvas.SetLeft(item, left);
                                    break;
                            }
                            e.Handled = true;
                        } else {
                            item.Height -= deltaVertical;
                        }
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, item.ActualHeight - item.MinHeight);
                        if (SnapRatioOnShiftDown && shiftPressed) {
                            double right = Canvas.GetLeft(item) + item.Width, left = Canvas.GetLeft(item), bottom = Canvas.GetTop(item) + item.Height;
                            item.Height -= deltaVertical;
                            item.Width = item.Height * item.Image.Width / item.Image.Height;
                            Canvas.SetTop(item, bottom - item.Height);
                            switch (HorizontalAlignment) {
                                case HorizontalAlignment.Left:
                                    Canvas.SetLeft(item, right - item.Width);
                                    break;
                                default:
                                    Canvas.SetLeft(item, left);
                                    break;
                            }
                            e.Handled = true;
                        } else {
                            Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
                            item.Height -= deltaVertical;
                        }
                        break;
                    default:
                        break;
                }

                if (e.Handled) return;

                switch (HorizontalAlignment) {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, item.ActualWidth - item.MinWidth);
                        if (SnapRatioOnShiftDown && shiftPressed) {
                            double right = Canvas.GetLeft(item) + item.Width, top = Canvas.GetTop(item);
                            item.Width -= deltaHorizontal;
                            item.Height = item.Width * item.Image.Height / item.Image.Width;
                            Canvas.SetTop(item, top);
                            Canvas.SetLeft(item, right - item.Width);
                        } else {
                            Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                            item.Width -= deltaHorizontal;
                        }
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, item.ActualWidth - item.MinWidth);
                        if (SnapRatioOnShiftDown && shiftPressed) {
                            double left = Canvas.GetLeft(item), top = Canvas.GetTop(item);
                            item.Width -= deltaHorizontal;
                            item.Height = item.Width * item.Image.Height / item.Image.Width;
                            Canvas.SetTop(item, top);
                            Canvas.SetLeft(item, left);
                        } else {
                            Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                            item.Width -= deltaHorizontal;
                        }
                        item.Width -= deltaHorizontal;
                        break;
                    default:
                        break;
                }
            }

            e.Handled = true;
        }
    }
}
