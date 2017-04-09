using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Serialization;
using mapKnight.ToolKit.Controls.Components;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace mapKnight.ToolKit.Data.Components {
    public class PlatformDataComponent : Component, IUserControlComponent {
        private static Texture2D emptyTexture;

        public event Action RequestRender;

        public UserControl Control { get; }

        public ObservableCollection<Vector2> Waypoints { get; set; }
        public Action<Func<Vector2, bool>> RequestMapVectorList { get; set; }

        public PlatformDataComponent(Entity owner) : base(owner) {
            Waypoints = new ObservableCollection<Vector2>( ) { new Vector2(0, 0) };
            Control = new PlatformDataControl(this);
            Waypoints.CollectionChanged += (sender, e) => RequestRender?.Invoke( );
        }

        public override void Load(Dictionary<DataID, object> data) {
            Waypoints.Clear( );
            Waypoints.AddRange((Vector2[ ])data[DataID.PLATFORM_Waypoint]);
        }

        public void Render(SpriteBatch spriteBatch, int ox, int oy, int tilesize) {
            if (emptyTexture == null) {
                emptyTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                emptyTexture.SetData(new Microsoft.Xna.Framework.Color[ ] { new Microsoft.Xna.Framework.Color(Microsoft.Xna.Framework.Color.Lime, 128) });
            }
            for (int i = 0; i < Waypoints.Count; i++)
                spriteBatch.Draw(emptyTexture, new Microsoft.Xna.Framework.Rectangle((int)((Waypoints[i].X - ox + Owner.Transform.Center.X) * tilesize), (int)((Owner.World.Size.Height - Waypoints[i].Y - oy - Owner.Transform.Center.Y) * tilesize), tilesize / 5, tilesize / 5), null, Microsoft.Xna.Framework.Color.White, Mathf.PI / 4f, new Microsoft.Xna.Framework.Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
        }

        public IEnumerable<Tuple<DataID, DataType, object>> CollectData( ) {
            yield return Tuple.Create(DataID.PLATFORM_Waypoint, DataType.Vector2Array, (object)Waypoints.ToArray( ));
        }

        public new class Configuration : Component.Configuration {
            public override Component Create(Entity owner) {
                return new PlatformDataComponent(owner);
            }
        }
    }
}
