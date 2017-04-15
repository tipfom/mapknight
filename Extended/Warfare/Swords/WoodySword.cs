using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics.UI;

namespace mapKnight.Extended.Warfare.Swords {
    public class WoodySword : Sword {
        public override string Name { get { return "Woody Sword"; } }
        public override string[ ] SpecialGestures { get { return new[ ] { UIGesturePanel.SWIPE_LEFT, UIGesturePanel.SWIPE_RIGHT, UIGesturePanel.SWIPE_DOWN }; } }

        public WoodySword (Entity holder) : base(holder, new Vector2(1f * holder.Transform.Width, 0f), new Vector2(holder.Transform.Width, holder.Transform.Height), 2f) {
        }

        public override void Special (string move) {
            switch (move) {
                case UIGesturePanel.SWIPE_LEFT:
                    break;
                case UIGesturePanel.SWIPE_RIGHT:
                    break;
                case UIGesturePanel.SWIPE_DOWN:
                    break;
            }
        }
    }
}
