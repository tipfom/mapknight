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

        public UIDialog (Screen owner, NPCComponent npc) : base(owner, new UILayout(new UIMargin(0.375f, 0.375f, 0.35f, .1f), UIMarginType.Relative, UIPosition.Center | UIPosition.Bottom, UIPosition.Center | UIPosition.Bottom), UIDepths.MIDDLE) {
            this.npc = npc;
            currentPopupLabel = new UILabel(owner, new UILayout(new UIMargin(.1f, .1f), UIMarginType.Relative, UIPosition.Top | UIPosition.Left, UIPosition.Top | UIPosition.Left, relative: this), UIDepths.FOREGROUND, 0.08f, npc.NextMessage( ), UITextAlignment.Left);
            dotsLabel = new UILabel(owner, new UILayout(new UIMargin(.05f, .05f), UIMarginType.Relative, UIPosition.Bottom | UIPosition.Right, UIPosition.Bottom | UIPosition.Right, relative: this), UIDepths.FOREGROUND, 0.1f, "...", UITextAlignment.Center);

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
            IsDirty = true;
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            yield return new DepthVertexData(Layout, "blank", UIDepths.BACKGROUND, Color.Black);
            yield return new DepthVertexData(UIRectangle.GetVerticies(Layout.Position + new Vector2(0.0125f, -0.0125f), Layout.Size - new Vector2(0.025f, 0.025f)), "blank", UIDepths.MIDDLE, Color.White);
            yield return new DepthVertexData(UIRectangle.GetVerticies(Layout.Position + new Vector2(0.025f, -0.025f), Layout.Size - new Vector2(0.05f, 0.05f)), "blank", UIDepths.MIDDLE, Color.Black);
        }
    }
}
