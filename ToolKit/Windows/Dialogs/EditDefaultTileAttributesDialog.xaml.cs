﻿using System;
using System.Collections.Generic;
using System.Windows;
using mapKnight.Core;

namespace mapKnight.ToolKit.Windows {
    /// <summary>
    /// Interaktionslogik für EditDefaultTileAttributesWindow.xaml
    /// </summary>
    public partial class EditDefaultTileAttributesWindow : Window {
        public Dictionary<TileAttribute, string> NewDefault = new Dictionary<TileAttribute, string>( );

        public EditDefaultTileAttributesWindow ( ) {
            InitializeComponent( );
        }

        public EditDefaultTileAttributesWindow(Dictionary<TileAttribute,string> oldDefault) {
            InitializeComponent( );
            foreach(TileAttribute attribute in Enum.GetValues(typeof(TileAttribute))) {
                if (oldDefault.ContainsKey(attribute)) {
                    listview_tile_attributes.Items.Add(new AttributeListViewEntry(true, attribute.ToString( ), oldDefault[attribute]));
                } else {
                    listview_tile_attributes.Items.Add(new AttributeListViewEntry(false, attribute.ToString( ), ""));
                }
            }
        }
        
        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            foreach(AttributeListViewEntry entry in listview_tile_attributes.Items) {
                if (entry.Active) {
                    NewDefault.Add((TileAttribute)Enum.Parse(typeof(TileAttribute), entry.Attribute), entry.Value);
                }
            }
            DialogResult = true;
        }

        private void Button_Apply_Click (object sender, RoutedEventArgs e) {
            Close( );
        }

        private class AttributeListViewEntry {

            public AttributeListViewEntry (bool active, string attribute, string value) {
                Active = active;
                Attribute = attribute;
                Value = value;
            }

            public bool Active { get; set; }
            public string Attribute { get; set; }
            public string Value { get; set; }
        }
    }
}
