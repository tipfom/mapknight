using System;
using System.Windows.Controls;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.ToolKit.Controls.Components;
using Microsoft.Xna.Framework.Graphics;
using mapKnight.Core.World.Components;
using mapKnight.Core.World.Serialization;
using System.Collections.Generic;

namespace mapKnight.ToolKit.Data.Components {
    public class SlimeDataComponent : Component, IUserControlComponent {
        public enum Direction : sbyte {
            Top = 0,
            Down = 1,
            Left = 2,
            Right = -1
        }
        private Direction _InitialWallDirection = Direction.Down;
        public Direction InitialWallDirection {
            get { return _InitialWallDirection; }
            set { _InitialWallDirection = value; RequestRender?.Invoke( ); }
        }
        private Direction _InitialMoveDirection = Direction.Left;
        public Direction InitialMoveDirection {
            get { return _InitialMoveDirection; }
            set { _InitialMoveDirection = value; RequestRender?.Invoke( ); }
        }

        public event Action RequestRender;

        public UserControl Control { get; }
        public Action<Func<Vector2, bool>> RequestMapVectorList { get; set; }

        public SlimeDataComponent(Entity owner) : base(owner) {
            Control = new SlimeDataControl(this);
        }

        public void Render(SpriteBatch spriteBatch, float offsetx, float offsety, int tilesize) {
        }

        public override void Draw( ) {
            SpriteEffects effect;
            float rotation;
            SetGraphicalData(out effect, out rotation);
            Owner.SetComponentInfo(ComponentData.Texture, effect, rotation);
        }

        public override void Load(Dictionary<DataID, object> data) {
            _InitialMoveDirection = (Direction)data[DataID.SLIME_InitialMoveDirection];
            _InitialWallDirection = (Direction)data[DataID.SLIME_InitialWallDirection];
        }

        private void SetGraphicalData(out SpriteEffects effect, out float rotation) {
            switch (InitialMoveDirection) {
                case Direction.Top:
                    rotation = Mathf.PI / 2f;
                    effect = InitialWallDirection == Direction.Right ? SpriteEffects.FlipVertically : SpriteEffects.None;
                    break;

                case Direction.Down:
                    rotation = Mathf.PI * 3f / 2f;
                    effect = InitialWallDirection == Direction.Left ? SpriteEffects.FlipVertically : SpriteEffects.None;
                    break;

                case Direction.Left:
                    if (InitialWallDirection == Direction.Down) {
                        rotation = 0f;
                        effect = SpriteEffects.None;
                    } else {
                        rotation = Mathf.PI;
                        effect = SpriteEffects.FlipHorizontally;
                    }
                    break;

                case Direction.Right:
                    if (InitialWallDirection == Direction.Down) {
                        rotation = 0f;
                        effect = SpriteEffects.FlipHorizontally;
                    } else {
                        rotation = Mathf.PI;
                        effect = SpriteEffects.None;
                    }
                    break;
                default:
                    rotation = 0f;
                    effect = SpriteEffects.None;
                    break;
            }
        }

        public IEnumerable<Tuple<DataID, DataType, object>> CollectData( ) {
            yield return Tuple.Create(DataID.SLIME_InitialMoveDirection, DataType.SByte, (object)((sbyte)_InitialMoveDirection));
            yield return Tuple.Create(DataID.SLIME_InitialWallDirection, DataType.SByte, (object)((sbyte)_InitialWallDirection));
        }

        public new class Configuration : Component.Configuration {
            public override Component Create(Entity owner) {
                return new SlimeDataComponent(owner);
            }
        }
    }
}
