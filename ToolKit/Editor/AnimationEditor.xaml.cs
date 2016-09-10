using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using mapKnight.ToolKit.Controls.Components.Graphics;
using mapKnight.ToolKit.Windows;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using Path = System.IO.Path;

namespace mapKnight.ToolKit.Editor {
    /// <summary>
    /// Interaktionslogik für AnimationEditor.xaml
    /// </summary>
    public partial class AnimationEditor : UserControl {
        private List<FrameworkElement> _Menu = new List<FrameworkElement>( ) {
            new MenuItem() { Header = "ANIMATION", Items = {
                    new MenuItem() {Header="NEW", Icon = App.Current.FindResource("image_animation_new") },
                    new MenuItem() {Header="LOAD" }
            } },
            new ComboBox() { VerticalAlignment = VerticalAlignment.Center, Width = 200 }
        };

        public List<FrameworkElement> Menu { get { return _Menu; } }

        private ObservableCollection<AnimationControl> animationControls { get; set; } = new ObservableCollection<AnimationControl>( );
        private ObservableCollection<string> animationControlStrings { get; set; } = new ObservableCollection<string>( );

        public AnimationEditor ( ) {
            InitializeComponent( );

            ((MenuItem)((MenuItem)_Menu[0]).Items[0]).Click += AnimationAdd_Click; ;
            ((MenuItem)((MenuItem)_Menu[0]).Items[1]).Click += AnimationLoad_Click; ;
            ((ComboBox)_Menu[1]).SelectionChanged += ComboBoxAnimation_SelectionChanged;
            ((ComboBox)_Menu[1]).ItemsSource = animationControlStrings;

            App.ProjectChanged += ( ) => {
                App.Project.Saved += (path) => {
                    string animpath = Path.Combine(path, "animations");
                    foreach (AnimationControl animationControl in animationControls)
                        animationControl.Save(animpath);
                };
            };
        }

        private void AnimationLoad_Click (object sender, RoutedEventArgs e) {
            FolderBrowserDialog dialog = new FolderBrowserDialog( );
            if (dialog.ShowDialog( ) == System.Windows.Forms.DialogResult.OK) {
                animationControls.Add(new AnimationControl(dialog.SelectedPath));
                animationControlStrings.Add(animationControls[animationControls.Count - 1].ToString( ));
                ((ComboBox)_Menu[1]).SelectedIndex = ((ComboBox)_Menu[1]).Items.Count - 1;
            }
        }

        private void AnimationAdd_Click (object sender, RoutedEventArgs e) {
            AddAnimationWindow dialog = new AddAnimationWindow( );
            if (dialog.ShowDialog( ) ?? false) {
                animationControls.Add(new AnimationControl(dialog.Ratio, dialog.textbox_name.Text));
                animationControlStrings.Add(animationControls[animationControls.Count - 1].ToString( ));
                ((ComboBox)_Menu[1]).SelectedIndex = ((ComboBox)_Menu[1]).Items.Count - 1;
            }
        }

        private void ComboBoxAnimation_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            contentpresenter_animation.Content = animationControls[((ComboBox)_Menu[1]).SelectedIndex];
        }
    }
}
