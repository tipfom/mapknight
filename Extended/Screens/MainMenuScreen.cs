using System.Reflection;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Extended.Screens.Windows;
using mapKnight.Extended.Warfare;

namespace mapKnight.Extended.Screens {
    public class MainMenuScreen : Screen {
        public BaseWeapon SelectedWeapon (Entity owner) { 
            switch (weaponSelectWindow.SelectedWeapon) {
                case 0:
                    return BaseWeaponCollection.Broadswords.Diamond(owner);
                case 1:
                    return BaseWeaponCollection.Broadswords.Copper(owner);
                case 2:
                    return BaseWeaponCollection.Rapier.Jade(owner);
                case 3:
                    return BaseWeaponCollection.Dagger.Rubidium(owner);
            }
            return null;
        }

        WeaponSelectWindow weaponSelectWindow;

        public override void Load ( ) {
            new UILabel(this, new UILayout(new UIMargin(0.05f, 0.05f), UIMarginType.Absolute, UIPosition.Bottom | UIPosition.Right, UIPosition.Bottom | UIPosition.Right), UIDepths.FOREGROUND, 0.07f, "VERSION: " + Assembly.GetExecutingAssembly( ).GetName( ).Version.ToString(3)
#if DEBUG
                + " - DEBUG"
#endif                
                );
            UIMap map = new UIMap(this);

            UIButton playButton = new UIButton(this, new UILayout(new UIMargin(293f / 450f, 102f / 450f, 53f / 450f, 176f / 450f), UIMarginType.Pixel, UIPosition.Bottom | UIPosition.Left, UIPosition.Bottom | UIPosition.Left), "PLAY");
            playButton.Release += ( ) => {
                if (map.CurrentSelection != null) {
                    Screen.Gameplay = new GameplayScreen(map.CurrentSelection);
                    UIRenderer.Prepare(Screen.Gameplay);
                    Screen.Gameplay.Load( );
                    Screen.Active = Screen.Gameplay;
                }
            };

            weaponSelectWindow = new WeaponSelectWindow(this);
            weaponSelectWindow.Load( );
            UIButton weaponButton = new UIButton(this, new UILayout(new UIMargin(293f / 450f, 102f / 450f, 53f / 450f, 115f / 450f), UIMarginType.Pixel, UIPosition.Bottom | UIPosition.Left, UIPosition.Bottom | UIPosition.Left), "CUSTOMIZE");
            weaponButton.Release += ( ) => {
                weaponSelectWindow.FillUIBuffer(true);
                Screen.Active = weaponSelectWindow;
            };

            base.Load( );
        }

        public override void Update (DeltaTime dt) {
            base.Update(dt);
        }
    }
}
