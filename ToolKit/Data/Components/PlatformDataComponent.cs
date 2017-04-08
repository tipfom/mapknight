using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.ToolKit.Controls.Components;

namespace mapKnight.ToolKit.Data.Components {
    public class PlatformDataComponent : Component, IUserControlComponent {
        public UserControl Control { get; }

        public ObservableCollection<Vector2> Waypoints { get; set; }
        public Action<Func<Vector2, bool>> RequestMapVectorList { get; set; }

        public PlatformDataComponent(Entity owner) : base(owner) {
            Waypoints = new ObservableCollection<Vector2>( ) { new Vector2(0, 0) };
            Control = new PlatformDataControl(this);
        }

        public new class Configuration : Component.Configuration {
            public override Component Create(Entity owner) {
                return new PlatformDataComponent(owner);
            }
        }
    }
}
