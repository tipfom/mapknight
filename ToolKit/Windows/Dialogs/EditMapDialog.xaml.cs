using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using mapKnight.Core;

namespace mapKnight.ToolKit.Windows {
    /// <summary>
    /// Interaktionslogik für ModifyMapWindow.xaml
    /// </summary>
    public partial class ModifyMapWindow : Window {
        private Map modifyingMap;

        public ModifyMapWindow ( ) {
            InitializeComponent( );
        }

        public ModifyMapWindow(Map map) {
            InitializeComponent( );
            modifyingMap = map;
            textbox_creator.Text = map.Creator;
            textbox_name.Text = map.Name;
            textbox_gravityy.Text = map.Gravity.Y.ToString( );
            textbox_gravityx.Text = map.Gravity.X.ToString( );
        }

        private void button_create_Click (object sender, RoutedEventArgs e) {
            Vector2 gravity = new Vector2(0, 0);
            if (ValidName( ) && ValidCreator( ) && ValidGravity(ref gravity)) {
                modifyingMap.Name = textbox_name.Text;
                modifyingMap.Creator = textbox_creator.Text;
                modifyingMap.Gravity = gravity;
                DialogResult = true;
                Close( );
            }
        }

        private bool ValidName ( ) {
            if (textbox_name.Text.Replace(" ", "") != "")
                return true;
            else {
                MessageBox.Show("name is not valid", "invalid name", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool ValidCreator ( ) {
            if (textbox_creator.Text.Replace(" ", "") != "") {
                return true;
            } else {
                MessageBox.Show("creator is not valid", "invalid creator", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        
        private bool ValidGravity (ref Vector2 gravity) {
            float x, y = 0;
            if (textbox_gravityx.Text == "" || !float.TryParse(textbox_gravityx.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out x)) {
                MessageBox.Show("x value of gravity is not valied", "invalid gravity", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (textbox_gravityy.Text == "" || !float.TryParse(textbox_gravityy.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out y)) {
                MessageBox.Show("y value of gravity is not valied", "invalid gravity", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            gravity = new Vector2(x, y);
            return true;
        }
    }
}
