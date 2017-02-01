using System;
using mapKnight.Extended.Components.AI;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;
using System.Collections.Generic;

namespace mapKnight.Extended.Graphics.UI {
    public class UIDialog : UIItem {
        public event Action DialogFinished;
        UILabel currentPopup;
        NPCComponent npc;

        public UIDialog (Screen owner, NPCComponent npc) : base(owner, new UIHorizontalCenterMargin(0f), new UIBottomMargin(0.1f), new Vector2(Window.Ratio * 1.5f, 0.7f), UIDepths.MIDDLE){
            this.npc = npc;
            currentPopup = new UILabel(owner, new UIHorizontalCenterMargin(0), new UITopMargin(1.35f), UIDepths.FOREGROUND, 0.1f, npc.NextMessage( ), UITextAlignment.Center);

            void HandleClick ( )
            {
                string nextMessage = npc.NextMessage( );
                if (nextMessage != null) {
                    currentPopup.Text = nextMessage;
                } else {
                    currentPopup.Dispose( );
                    Dispose( );
                    DialogFinished?.Invoke( );
                }
            }
            Click += HandleClick;

            Window.Changed += ( ) => { Size =new Vector2(1.5f * Window.Ratio, 0.7f); };
            IsDirty = true;
        }

        public override List<DepthVertexData> ConstructVertexData ( ) {
            List<DepthVertexData> vertexData = new List<DepthVertexData>( );
            vertexData.Add(new DepthVertexData(Bounds.Verticies, "blank", UIDepths.BACKGROUND, Color.Black));
            vertexData.Add(new DepthVertexData(new UIRectangle(Bounds.Position + new Vector2(0.05f, -0.05f), Bounds.Size - new Vector2(0.1f, 0.1f)).Verticies,"blank", UIDepths.MIDDLE, Color.Blue));
            return vertexData;
        }
    }
}
