using System;
using mapKnight.Extended.Components.AI;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;
using System.Collections.Generic;

namespace mapKnight.Extended.Graphics.UI {
    public class UIDialog : UIItem {
        public event Action DialogFinished;
        UILabel currentPopupLabel;
        UILabel dotsLabel;
        NPCComponent npc;

        public UIDialog (Screen owner, NPCComponent npc) : base(owner, new UIHorizontalCenterMargin(0f), new UIBottomMargin(0.1f), new Vector2(Window.Ratio * 1.5f, 0.7f), UIDepths.MIDDLE){
            this.npc = npc;
            currentPopupLabel = new UILabel(owner, new UILeftMargin(0.3f * Window.Ratio), new UITopMargin(1.3f), UIDepths.FOREGROUND, 0.08f, npc.NextMessage( ), UITextAlignment.Left);
            dotsLabel = new UILabel(owner, new UIHorizontalCenterMargin(0.68f * Window.Ratio), new UIBottomMargin(0.15f),UIDepths.FOREGROUND, 0.1f, "...", UITextAlignment.Center);

            void HandleClick ( )
            {
                string nextMessage = npc.NextMessage( );
                if (nextMessage != null) {
                    currentPopupLabel.Text = nextMessage;
                } else {
                    currentPopupLabel.Dispose( );
                    dotsLabel.Dispose( );
                    Dispose( );
                    DialogFinished?.Invoke( );
                }
            }
            Click += HandleClick;

            Window.Changed += ( ) => {
                Size =new Vector2(1.5f * Window.Ratio, 0.7f);
            };
            IsDirty = true;
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            yield return new DepthVertexData(Bounds.Verticies, "blank", UIDepths.BACKGROUND, Color.Black);
            yield return new DepthVertexData(new UIRectangle(Bounds.Position + new Vector2(0.0125f, -0.0125f), Bounds.Size - new Vector2(0.025f, 0.025f)).Verticies, "blank", UIDepths.MIDDLE, Color.White);
            yield return new DepthVertexData(new UIRectangle(Bounds.Position + new Vector2(0.025f, -0.025f), Bounds.Size - new Vector2(0.05f, 0.05f)).Verticies, "blank", UIDepths.MIDDLE, Color.Black);
        }
    }
}
