using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.ToolKit.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System;
using Microsoft.Xna.Framework.Graphics;
using mapKnight.ToolKit.Data.Components;
using mapKnight.Core;
using System.Collections.Generic;
using mapKnight.ToolKit.Serializer;

namespace mapKnight.ToolKit.Controls {
    /// <summary>
    /// Interaktionslogik für EntityListBox.xaml
    /// </summary>
    public partial class EntityListBox : UserControl {
        public delegate void OnSelectionChanged (EntityData oldSelection, EntityData newSelection);
        public event OnSelectionChanged SelectionChanged;

        private ObservableCollection<EntityData> entityCollection = new ObservableCollection<EntityData>( );

        public EntityListBox ( ) {
            InitializeComponent( );
            listbox_entities.ItemsSource = entityCollection;
        }

        public void Init (GraphicsDevice g) {
            entityCollection.Add(new EntityData("Canone", "canone", g));
            entityCollection.Add(new EntityData("Guardian", "guardian_tent", g));
            entityCollection.Add(new EntityData("Landmine", "landmine", g));
            entityCollection.Add(new EntityData("Moonball", "button", g));
            entityCollection.Add(new EntityData("Plugger", "plugger", g));
            entityCollection.Add(new EntityData("Sepler", "sepler", g));
            entityCollection.Add(new EntityData("Shark", "shark", g));
            entityCollection.Add(new EntityData("Shell", "shell", g));
            entityCollection.Add(new EntityData("Slime", "slime", g));
            entityCollection.Add(new EntityData("Copper Platform", "platform_copper", g));
            entityCollection.Add(new EntityData("Lenny", "npc_lenny", g));
            entityCollection.Add(new EntityData("Drillbomb", "drillbomb", g));
            entityCollection.Add(new EntityData("Maple Tree", "tree_maple", g));
            entityCollection.Add(new EntityData("Cherry Tree", "tree_cherry", g));

            listbox_entities.SelectedIndex = 0;
        }

        private void listbox_entities_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            SelectionChanged?.Invoke((EntityData)e.RemovedItems[0], (EntityData)e.AddedItems[0]);
            if (e.AddedItems.Count > 0) {
                textblock_name.Text = ((EntityData)e.AddedItems[0]).Name;
            } else {
                textblock_name.Text = "";
            }
        }

        public EntityData Find (string name) {
            return entityCollection.FirstOrDefault(entityData => entityData.Name == name);
        }

        public Entity.Configuration GetCurrentShadowConfiguration ( ) {
            return WindowsEntitySerializer.SHADOW_CONFIGURATIONS[listbox_entities.SelectedIndex];
        }

        public Entity.Configuration GetCurrentFinalConfiguration ( ) {
            return WindowsEntitySerializer.FINAL_CONFIGURATIONS[listbox_entities.SelectedIndex];
        }

        public Dictionary<string, Texture2D> GetEntityData ( ) {
            return entityCollection.ToDictionary(v => v.Name, v => v.Texture);
        }
    }
}
