using System;
using System.Windows.Controls;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.ToolKit.Controls.Components;
using Microsoft.Xna.Framework.Graphics;

namespace mapKnight.ToolKit.Data.Components {
    public class SlimeDataComponent : Component, IUserControlComponent {
        public enum Direction {
            Top,
            Down,
            Left,
            Right
        }

        public Direction InitialWallDirection = Direction.Down;
        public Direction InitialMoveDirection = Direction.Left;

        public event Action RequestRender;

        public UserControl Control { get; }
        public Action<Func<Vector2, bool>> RequestMapVectorList { get; set; }

        public SlimeDataComponent(Entity owner) : base(owner) {
            Control = new SlimeDataControl(this);
        }

        public new class Configuration : Component.Configuration {
            public override Component Create(Entity owner) {
                return new SlimeDataComponent(owner);        
            }
        }

        public void Render(SpriteBatch spriteBatch, int offsetx, int offsety, int tilesize) {
        }
    }
}
