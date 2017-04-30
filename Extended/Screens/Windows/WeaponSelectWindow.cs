using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Screens.Windows {
    public class WeaponSelectWindow : WindowScreen {
        private UIItemFrame[ ] weaponFrames = new UIItemFrame[4];

        public int SelectedWeapon;

        public WeaponSelectWindow (Screen parent) : base(parent, new Vector2(2.1f, .6f)) {
        }

        public override void Load ( ) {
            weaponFrames[0] = new UIItemFrame(this, new UILayout(new UIMargin(.4f, .55f, .8f, .4f), UIMarginType.Absolute, dock: UIPosition.Center | UIPosition.Top, anchor: UIPosition.Right | UIPosition.Top), "wp_bs1", UIDepths.MIDDLE);
            weaponFrames[0].Selected = true;
            weaponFrames[0].Release += ( ) => SelectWeapon(0);

            weaponFrames[1] = new UIItemFrame(this, new UILayout(new UIMargin(.4f, .05f, .8f, .4f), UIMarginType.Absolute, dock: UIPosition.Center | UIPosition.Top, anchor: UIPosition.Right | UIPosition.Top), "wp_bs2", UIDepths.MIDDLE);
            weaponFrames[1].Release += ( ) => SelectWeapon(1);

            weaponFrames[2] = new UIItemFrame(this, new UILayout(new UIMargin(.05f, .4f, .8f, .4f), UIMarginType.Absolute, dock: UIPosition.Center | UIPosition.Top, anchor: UIPosition.Left | UIPosition.Top), "wp_rp1", UIDepths.MIDDLE);
            weaponFrames[2].Release += ( ) => SelectWeapon(2);

            weaponFrames[3] = new UIItemFrame(this, new UILayout(new UIMargin(.55f, .4f, .8f, .4f), UIMarginType.Absolute, dock: UIPosition.Center | UIPosition.Top, anchor: UIPosition.Left | UIPosition.Top), "wp_dg1", UIDepths.MIDDLE);
            weaponFrames[3].Release += ( ) => SelectWeapon(3);

            base.Load( );
        }

        private void SelectWeapon (int index) {
            if (weaponFrames[index].Selected) {
                Screen.Active = Parent;
            }
            for (int i = 0; i < weaponFrames.Length; i++) {
                weaponFrames[i].Selected = false;
            }
            weaponFrames[index].Selected = true;
            SelectedWeapon = index;
        }
    }
}
