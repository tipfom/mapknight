using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace mapKnight.ToolKit.Controls {
    public class ClosableTabItem : TabItem {
        public event Action<ClosableTabItem> CloseRequested;

        public ClosableTabItem ( ) : base( ) {
            Loaded += ClosableTabItem_Loaded;
        }

        private void Grid_Close_MouseDown (object sender, MouseButtonEventArgs e) {
            CloseRequested?.Invoke(this);
        }

        private void ClosableTabItem_Loaded (object sender, System.Windows.RoutedEventArgs e) {
            FrameworkElementFactory stackpanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackpanelFactory.Name = "stackpanel_container";
            stackpanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            stackpanelFactory.SetValue(StackPanel.VerticalAlignmentProperty, VerticalAlignment.Center);

            FrameworkElementFactory labelFactory = new FrameworkElementFactory(typeof(Label));
            labelFactory.Name = "label_header";
            labelFactory.SetValue(Label.ContentProperty, new Binding( ));
            labelFactory.SetValue(Label.PaddingProperty, new Thickness(0));
            labelFactory.SetValue(Label.MarginProperty, new Thickness(0));
            labelFactory.SetValue(Label.VerticalAlignmentProperty, VerticalAlignment.Center);
            stackpanelFactory.AppendChild(labelFactory);

            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.Name = "button_close";
            buttonFactory.SetValue(Button.PaddingProperty, new Thickness(0));
            buttonFactory.SetValue(Button.MarginProperty, new Thickness(5, 1, 0, 0));
            buttonFactory.SetValue(Button.WidthProperty, 15d);
            buttonFactory.SetValue(Button.FocusableProperty, false);
            buttonFactory.SetValue(Button.BackgroundProperty, Brushes.Transparent);
            buttonFactory.SetValue(Button.BorderBrushProperty, Brushes.Transparent);
            buttonFactory.SetValue(Button.VerticalAlignmentProperty, VerticalAlignment.Center);
            buttonFactory.SetValue(Button.StyleProperty, GetButtonStyle( ));
            stackpanelFactory.AppendChild(buttonFactory);

            HeaderTemplate = new DataTemplate( );
            HeaderTemplate.VisualTree = stackpanelFactory;
        }

        private Style GetButtonStyle ( ) {
            Style buttonStyle = new Style(typeof(Button)) { };
            buttonStyle.Setters.Add(new Setter(Button.FocusVisualStyleProperty, null));
            buttonStyle.Setters.Add(new Setter(Button.OverridesDefaultStyleProperty, true));
            buttonStyle.Setters.Add(new Setter(Button.TemplateProperty, GetButtonTemplate( )));
            return buttonStyle;
        }

        private ControlTemplate GetButtonTemplate ( ) {
            FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.AddHandler(Grid.MouseDownEvent, new MouseButtonEventHandler(Grid_Close_MouseDown));

            FrameworkElementFactory ellipseFactory = new FrameworkElementFactory(typeof(Ellipse));
            ellipseFactory.Name = "ellipse_background";
            ellipseFactory.SetValue(Ellipse.MarginProperty, new Thickness(0));
            gridFactory.AppendChild(ellipseFactory);

            FrameworkElementFactory pathFactory = new FrameworkElementFactory(typeof(Path));
            pathFactory.Name = "path_x";
            pathFactory.SetValue(Path.MarginProperty, new Thickness(4));
            pathFactory.SetValue(Path.StrokeProperty, Brushes.Black);
            pathFactory.SetValue(Path.StrokeThicknessProperty, 1.5d);
            pathFactory.SetValue(Path.StretchProperty, Stretch.Uniform);
            pathFactory.SetValue(Path.VerticalAlignmentProperty, VerticalAlignment.Center);
            pathFactory.SetValue(Path.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            pathFactory.SetValue(Path.DataProperty, new PathGeometry(new[ ] { new PathFigure(new Point(0, 0), new[ ] { new LineSegment(new Point(25, 25), false) }, true), new PathFigure(new Point(0, 25), new[ ] { new LineSegment(new Point(25, 0), false) }, true) }));
            gridFactory.AppendChild(pathFactory);

            ControlTemplate buttonTemplate = new ControlTemplate(typeof(Button));
            buttonTemplate.VisualTree = gridFactory;
            buttonTemplate.Triggers.Add(new Trigger( ) {
                Property = Grid.IsMouseOverProperty, Value = true, Setters = {
                    new Setter(Ellipse.FillProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC13535")), "ellipse_background"),
                    new Setter(Path.StrokeProperty, Brushes.White, "path_x")
                }
            });

            return buttonTemplate;
        }
    }
}
