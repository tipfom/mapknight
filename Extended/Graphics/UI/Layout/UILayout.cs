using mapKnight.Core;
using System;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class UILayout {
        public static implicit operator float[ ] (UILayout layout) {
            return layout.Rectangle.Verticies;
        }

        public event Action Changed;
        public event Action UpdateRequired;

        public Vector2 Position = new Vector2( );
        public Vector2 Size = new Vector2( );
        public bool IsDirty = false;

        public float Width { get { return Size.X; } }
        public float Height { get { return Size.Y; } }
        public float X { get { return Position.X; } }
        public float Y { get { return Position.Y; } }

        private UIPosition _Anchor;
        public UIPosition Anchor { get { return _Anchor; } set { _Anchor = value; PropertyChanged( ); } }

        private UIPosition _Dock;
        public UIPosition Dock { get { return _Dock; } set { _Dock = value; PropertyChanged( ); } }

        private UIMargin _Margin;
        public UIMargin Margin { get { return _Margin; } set { _Margin = value; PropertyChanged( ); } }

        private UIItem _Relative;
        public UIItem Relative { get { return _Relative; } set { _Relative = value; PropertyChanged( ); } }

        private UIMarginType _Type;
        public UIMarginType Type { get { return _Type; } set { _Type = value; PropertyChanged( ); } }

        public UIRectangle Rectangle;

        private UIItem item;
        private bool suppressChanges;
        
        public UILayout (UIMargin margin, UIMarginType type, UIPosition anchor = UIPosition.Left | UIPosition.Top, UIPosition dock = UIPosition.Left | UIPosition.Top, UIItem relative = null) {
            _Margin = margin;
            _Type = type;
            _Anchor = anchor;
            _Dock = dock;
            _Relative = relative;

            if (relative != null) {
                relative.Layout.Changed += ( ) => { PropertyChanged( ); };
            } else {
                Window.Changed += ( ) => { PropertyChanged( ); };
            }
            _Margin.Changed += ( ) => { PropertyChanged( ); };
        }

        public void Initialize (UIItem item) {
            this.item = item;
            Refresh( );
        }

        public void Refresh ( ) {
            IsDirty = false;
            Vector2 relativePosition = _Relative?.Layout.Position ?? item.Screen.Position;
            Vector2 relativeSize = _Relative?.Layout.Size ?? item.Screen.Size;
            float marginRight = _Margin.Right, marginLeft = _Margin.Left, marginTop = _Margin.Top, marginBottom = _Margin.Bottom;
            switch (_Type) {
                case UIMarginType.Relative:
                    marginRight *= relativeSize.X;
                    marginLeft *= relativeSize.X;
                    marginTop *= relativeSize.Y;
                    marginBottom *= relativeSize.Y;
                    break;
                case UIMarginType.Pixel:
                    marginRight *= relativeSize.X;
                    marginLeft *= relativeSize.X;
                    marginTop *= relativeSize.X;
                    marginBottom *= relativeSize.X;
                    break;
            }

            if ((_Dock & UIPosition.Left) == UIPosition.Left) {
                if ((_Anchor & UIPosition.Left) == UIPosition.Left) {
                    Size.X = marginRight;
                    Position.X = relativePosition.X + marginLeft;
                } else if ((_Anchor & UIPosition.Right) == UIPosition.Right) {
                    Size.X = marginLeft;
                    Position.X = relativePosition.X - marginRight - Size.X;
                } else {
                    Size.X = marginLeft + marginRight;
                    Position.X = relativePosition.X - marginLeft;
                }
            } else if ((_Dock & UIPosition.Right) == UIPosition.Right) {
                if ((_Anchor & UIPosition.Left) == UIPosition.Left) {
                    Size.X = marginRight;
                    Position.X = relativePosition.X + relativeSize.X + marginLeft;
                } else if ((_Anchor & UIPosition.Right) == UIPosition.Right) {
                    Size.X = marginLeft;
                    Position.X = relativePosition.X + relativeSize.X - marginRight - Size.X;
                } else {
                    Size.X = marginLeft + marginRight;
                    Position.X = relativePosition.X + relativeSize.X - marginLeft;
                }
            } else {
                if ((_Anchor & UIPosition.Left) == UIPosition.Left) {
                    Size.X = marginRight;
                    Position.X = relativePosition.X + relativeSize.X / 2f + marginLeft;
                } else if ((_Anchor & UIPosition.Right) == UIPosition.Right) {
                    Size.X = marginLeft;
                    Position.X = relativePosition.X + relativeSize.X / 2f - marginRight - Size.X;
                } else {
                    Size.X = marginLeft + marginRight;
                    Position.X = relativePosition.X + relativeSize.X / 2f - marginLeft;
                }
            }

            if ((_Dock & UIPosition.Top) == UIPosition.Top) {
                if ((_Anchor & UIPosition.Top) == UIPosition.Top) {
                    Size.Y = marginBottom;
                    Position.Y = relativePosition.Y - marginTop;
                } else if ((_Anchor & UIPosition.Bottom) == UIPosition.Bottom) {
                    Size.Y = marginTop;
                    Position.Y = relativePosition.Y + marginBottom + Size.Y;
                } else {
                    Size.Y = marginTop + marginBottom;
                    Position.Y = relativePosition.Y - marginTop;
                }
            } else if ((_Dock & UIPosition.Bottom) == UIPosition.Bottom) {
                if ((_Anchor & UIPosition.Top) == UIPosition.Top) {
                    Size.Y = marginBottom;
                    Position.Y = relativePosition.Y - relativeSize.Y + marginTop;
                } else if ((_Anchor & UIPosition.Bottom) == UIPosition.Bottom) {
                    Size.Y = marginTop;
                    Position.Y = relativePosition.Y - relativeSize.Y + marginBottom + Size.Y;
                } else {
                    Size.Y = marginTop + marginBottom;
                    Position.Y = relativePosition.Y - relativeSize.Y + marginTop;
                }
            } else {
                if ((_Anchor & UIPosition.Top) == UIPosition.Top) {
                    Size.Y = marginBottom;
                    Position.Y = relativePosition.Y - relativeSize.Y / 2f + marginTop;
                } else if ((_Anchor & UIPosition.Bottom) == UIPosition.Bottom) {
                    Size.Y = marginTop;
                    Position.Y = relativePosition.Y - relativeSize.Y / 2f + marginBottom + Size.Y;
                } else {
                    Size.Y = marginTop + marginBottom;
                    Position.Y = relativePosition.Y - relativeSize.Y / 2f + marginTop;
                }
            }

            Rectangle = new UIRectangle(Position, Size);
            Changed?.Invoke( );
        }

        public void AdjustSize (Vector2 target) {
            Vector2 relativeSize = _Relative?.Layout.Size ?? item.Screen.Size;

            if (_Type == UIMarginType.Relative) target /= relativeSize;

            suppressChanges = true;

            if ((_Anchor & UIPosition.Left) == UIPosition.Left) {
                Margin.Right = target.X;
            } else if ((_Anchor & UIPosition.Right) == UIPosition.Right) {
                Margin.Left = target.X;
            } else {
                Margin.Left = target.X / 2f;
                Margin.Right = target.X / 2f;
            }

            if ((_Anchor & UIPosition.Top) == UIPosition.Top) {
                Margin.Bottom = target.X;
            } else if ((_Anchor & UIPosition.Bottom) == UIPosition.Bottom) {
                Margin.Top = target.Y;
            } else {
                Margin.Top = target.Y / 2f;
                Margin.Bottom = target.Y / 2f;
            }

            suppressChanges = false;

            PropertyChanged( );
        }

        private void PropertyChanged ( ) {
            IsDirty = true;
            if (!suppressChanges)
                UpdateRequired?.Invoke( );
        }
    }
}