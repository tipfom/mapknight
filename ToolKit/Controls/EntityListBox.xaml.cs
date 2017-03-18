using mapKnight.ToolKit.Data;
using System.Windows.Controls;

namespace mapKnight.ToolKit.Controls {
    /// <summary>
    /// Interaktionslogik für EntityListBox.xaml
    /// </summary>
    public partial class EntityListBox : UserControl {
        public delegate void OnSelectionChanged(EntityData oldSelection, EntityData newSelection);
        public event OnSelectionChanged SelectionChanged;

        public EntityListBox( ) {
            InitializeComponent( );
            Add(new EntityData("Canone", "canone"));
            Add(new EntityData("Guardian", "guardian_tent"));
            Add(new EntityData("Landmine", "landmine"));
            Add(new EntityData("Moonball", "moonball"));
            Add(new EntityData("Plugger", "plugger"));
            Add(new EntityData("Sepler", "sepler"));
            Add(new EntityData("Shark", "shark"));
            Add(new EntityData("Shell", "shell"));
            Add(new EntityData("Slime", "slime"));
        }

        public void Add(EntityData data) {
            listbox_entities.Items.Add(data);
        }

        private void listbox_entities_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectionChanged?.Invoke((EntityData)e.RemovedItems[0], (EntityData)e.AddedItems[0]);
            if(e.AddedItems.Count > 0) {
                textblock_name.Text = ((EntityData)e.AddedItems[0]).Name;
            } else {
                textblock_name.Text = "";
            }
        }
    }
}
