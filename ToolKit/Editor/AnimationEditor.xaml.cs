using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using mapKnight.ToolKit.Controls;
using mapKnight.ToolKit.Windows;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace mapKnight.ToolKit.Editor {
    public partial class AnimationEditor : UserControl {
        private List<FrameworkElement> _Menu = new List<FrameworkElement>( ) {
            new MenuItem() { Header = "ANIMATION", Items = {
                    new MenuItem() {Header="NEW", Icon = App.Current.FindResource("image_animation_new") },
                    new MenuItem() {Header="LOAD" }
            } },
            new ComboBox() { VerticalAlignment = VerticalAlignment.Center, Width = 200 }
        };

        public List<FrameworkElement> Menu {
            get {
                if (animationControls.Count > 0) return new List<FrameworkElement>(_Menu.Concat(animationControls[((ComboBox)_Menu[1]).SelectedIndex].Menu));
                else return _Menu;
            }
        }

        public event Action MenuChanged;

        private ObservableCollection<AnimationControl2> animationControls { get; set; } = new ObservableCollection<AnimationControl2>( );
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
                    foreach (AnimationControl2 animationControl in animationControls)
                        animationControl.Save(animpath);
                };
            };
        }

        private void AnimationLoad_Click (object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog( );
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Animation-Meta File|animation.meta";
            if (openFileDialog.ShowDialog( ) ?? false) {
                animationControls.Add(new AnimationControl2(openFileDialog.FileName));
                animationControlStrings.Add(animationControls[animationControls.Count - 1].ToString( ));
                ((ComboBox)_Menu[1]).SelectedIndex = ((ComboBox)_Menu[1]).Items.Count - 1;
            }
        }

        private void AnimationAdd_Click (object sender, RoutedEventArgs e) {
            AddAnimationWindow dialog = new AddAnimationWindow( );
            if (dialog.ShowDialog( ) ?? false) {
                animationControls.Add(new AnimationControl2(dialog.Ratio, dialog.textbox_name.Text));
                animationControlStrings.Add(animationControls[animationControls.Count - 1].ToString( ));
                ((ComboBox)_Menu[1]).SelectedIndex = ((ComboBox)_Menu[1]).Items.Count - 1;
            }
        }

        private void ComboBoxAnimation_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            contentpresenter_animation.Content = animationControls[((ComboBox)_Menu[1]).SelectedIndex];
            MenuChanged?.Invoke( );
        }
    }
}
