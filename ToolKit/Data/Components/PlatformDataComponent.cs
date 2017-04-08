using System.Windows.Controls;
using mapKnight.Core.World;
using mapKnight.ToolKit.Controls.Components;
using System.Collections.Generic;
using mapKnight.Core;
using System;
using System.Collections.ObjectModel;

namespace mapKnight.ToolKit.Data.Components {
    public class PlatformDataComponent : Component, IUserControlComponent {
        public UserControl Control { get; }

        public ObservableCollection<Vector2> Waypoints { get; set; }
        public Action<Action<List<Vector2>>> RequestMapVectorList { get; set; }

        public PlatformDataComponent(Entity owner) : base(owner) {
            Waypoints = new ObservableCollection<Vector2>( );
            Control = new PlatformDataControl(this);
        }

        public new class Configuration : Component.Configuration {
            public override Component Create(Entity owner) {
                return new PlatformDataComponent(owner);
            }
        }
    }
}
