using mapKnight.Core.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mapKnight.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Controls;
using mapKnight.ToolKit.Controls.Components;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace mapKnight.ToolKit.Data.Components {
    public class MoonballDataComponent : Component, IUserControlComponent {
        private static Texture2D moonballTexture;

        public UserControl Control { get; }
        public Action<Func<Vector2, bool>> RequestMapVectorList { get; set; }

        public event Action RequestRender;

        private Vector2 _MoonballSpawnOffset = new Vector2(0, 0);
        public Vector2 MoonballSpawnOffset {
            get { return _MoonballSpawnOffset; }
            set { _MoonballSpawnOffset = value; RequestRender?.Invoke( ); }
        }

        public MoonballDataComponent(Entity owner) : base(owner) {
            Control = new MoonballDataControl(this);
        }

        public void Render(SpriteBatch spriteBatch, int offsetx, int offsety, int tilesize) {
            if (moonballTexture == null) {
                moonballTexture = new BitmapImage(new Uri(@"pack://application:,,,/" + Assembly.GetExecutingAssembly( ).GetName( ).Name + ";component/Resources/Images/Entities/moonball.png", UriKind.Absolute)).ToTexture2D(spriteBatch.GraphicsDevice);
            }
            spriteBatch.Draw(moonballTexture,
                new Microsoft.Xna.Framework.Rectangle(
                    (int)((Owner.Transform.Center.X + MoonballSpawnOffset.X - offsetx) * tilesize),
                    (int)((Owner.World.Size.Height - Owner.Transform.Center.Y - MoonballSpawnOffset.Y - offsety) * tilesize),
                    2 * tilesize,
                    2 * tilesize)
                    , null, Microsoft.Xna.Framework.Color.White, 0f, new Microsoft.Xna.Framework.Vector2(moonballTexture.Width / 2f, moonballTexture.Height / 2f), SpriteEffects.None, 0);
        }

        public new class Configuration : Component.Configuration {
            public override Component Create(Entity owner) {
                return new MoonballDataComponent(owner);
            }
        }
    }
}
