using mapKnight.Core.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mapKnight.Core;
using mapKnight.Core.World.Serialization;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Controls;
using mapKnight.ToolKit.Controls.Components;

namespace mapKnight.ToolKit.Data.Components {
    public class NPCDataComponent : Component, IUserControlComponent {
        public string[ ] Dialog = new string[0];

        public UserControl Control { get; set; }
        public Action<Func<Vector2, bool>> RequestMapVectorList { get; set; }

        public event Action RequestRender;

        public NPCDataComponent(Entity owner) : base(owner) {
            Control = new NPCDataControl(this);
        }

        public override void Load(Dictionary<DataID, object> data) {
            Dialog = (string[ ])data[DataID.NPC_Messages];
            ((NPCDataControl)Control).UpdateTextBox( );
        }

        public IEnumerable<Tuple<DataID, DataType, object>> CollectData( ) {
            for(int i =0;i < Dialog.Length;i++) {
                Dialog[i] = Dialog[i].TrimStart('\n', '\r').TrimEnd('\n', '\r');
            }
            yield return Tuple.Create(DataID.NPC_Messages, DataType.StringArray, (object)Dialog);
        }
        
        public void Render(SpriteBatch spriteBatch, float offsetx, float offsety, int tilesize) {
        }

        public new class Configuration : Component.Configuration {
            public override Component Create(Entity owner) {
                return new NPCDataComponent(owner);
            }
        }
    }
}
