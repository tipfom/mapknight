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
                    new MenuItem() {Header="NEW", Icon = App.Current.FindResource("image_animation_new") }
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
            ((ComboBox)_Menu[1]).SelectionChanged += ComboBoxAnimation_SelectionChanged;
            ((ComboBox)_Menu[1]).ItemsSource = animationControlStrings;
        }

        private void AnimationAdd_Click (object sender, RoutedEventArgs e) {
            AddAnimationWindow dialog = new AddAnimationWindow( );
            if (dialog.ShowDialog( ) ?? false ) {
                if(!animationControls.Any(item => item.MetaData.Entity == dialog.textbox_name.Text)) {
                    animationControls.Add(new AnimationControl2(dialog.Ratio, dialog.textbox_name.Text));
                    animationControlStrings.Add(animationControls[animationControls.Count - 1].ToString( ));
                    ((ComboBox)_Menu[1]).SelectedIndex = ((ComboBox)_Menu[1]).Items.Count - 1;
                } else {
                    MessageBox.Show("please dont add entities with the same name");
                }
            }
        }

        private void ComboBoxAnimation_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            if (((ComboBox)_Menu[1]).SelectedIndex == -1) return;
            contentpresenter_animation.Content = animationControls[((ComboBox)_Menu[1]).SelectedIndex];
            MenuChanged?.Invoke( );
        }

        public void Save (Project project) {
            foreach (AnimationControl2 animcontrol in animationControls)
                animcontrol.Save(project);
        }

        public void Load (Project project) {
            Clear( );
            foreach (string animationdirectory in project.GetAllEntries("animations")) {
                if (Path.GetFileName(animationdirectory) == ".meta") {
                    animationControls.Add(new AnimationControl2(project, Path.GetDirectoryName(animationdirectory)));
                    animationControlStrings.Add(animationControls[animationControls.Count - 1].ToString( ));
                }
            }
            if (animationControls.Count > 0) ((ComboBox)_Menu[1]).SelectedIndex = 0;
        }

        public void Clear ( ) {
            animationControls.Clear( );
            animationControlStrings.Clear( );
        }

        public void Compile (string animationpath) {
            foreach (AnimationControl2 animcontrol in animationControls)
                animcontrol.Compile(animationpath);
        }
    }
}
