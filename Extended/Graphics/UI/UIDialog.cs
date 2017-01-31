using System;
using mapKnight.Extended.Components.AI;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UIDialog {
        public event Action DialogFinished;
        UILabel currentPopup;
        NPCComponent npc;

        public UIDialog (Screen owner, NPCComponent npc) {
            this.npc = npc;
            currentPopup = new UILabel(owner, new UIHorizontalCenterMargin(0), new UIBottomMargin(0.2f), 0.1f, npc.NextMessage( ), UITextAlignment.Center);

            void HandleClick ( )
            {
                string nextMessage = npc.NextMessage( );
                if (nextMessage != null) {
                    currentPopup.Text = nextMessage;
                } else {
                    currentPopup.Dispose( );
                    DialogFinished?.Invoke( );
                }
            }
            currentPopup.Click += HandleClick;
        }
    }
}
