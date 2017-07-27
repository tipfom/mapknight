using System;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Components.Player;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.UI;
using mapKnight.Extended.Graphics.UI.Layout;
using Map = mapKnight.Extended.Graphics.Map;
using mapKnight.Extended.Combat;
using System.Collections.Generic;

namespace mapKnight.Extended.Screens {
    public class GameplayScreen : Screen {
        private const int MAX_TIME_BETWEEN_UPDATES = 100;

        private UILabel debugLabel;
        private UIControlButton leftButton, rightButton;
        private UIGesturePanel controlPanel;
        private UIBar healthBar;
        private UIAbilityPanel abilityPanel;

        private Map map;
        private Entity playerEntity;
        private PlayerComponent playerComponent;
        private string mapName;
        private Ability currentGestureAbility;

        public GameplayScreen (string mapName) {
            this.mapName = mapName;
        }

        public override void Draw ( ) {
            map.Draw( );
            base.Draw( );
            abilityPanel.Draw( );
            controlPanel.Draw(Color.Black, Color.Red);
        }

        public override void Load ( ) {
            map = Assets.Load<Map>(mapName);
            for (int i = 0; i < map.Entities.Count; i++) {
                map.Entities[i].Prepare( );
            }
            map.EntityAdded += (Entity obj) => { obj.Prepare( ); };

            playerEntity = EntityCollection.Players.Diamond.Create(map.SpawnPoint, map);
            playerComponent = playerEntity.GetComponent<PlayerComponent>( );
            map.Focus(playerEntity.ID);

            debugLabel = new UILabel(this, new UILayout(new UIMargin(0.1f, 0.075f), UIMarginType.Absolute, UIPosition.Right | UIPosition.Top, UIPosition.Right | UIPosition.Top), 0.05f, "", UITextAlignment.Right);
            healthBar = new UIBar(this, new Color(255, 0, 0, 127), new Color(255, 255, 255, 63), playerComponent.Health, new UILayout(new UIMargin(0, 1, 0, 0.025f), UIMarginType.Relative, UIPosition.Left | UIPosition.Top), UIDepths.MIDDLE);
            abilityPanel = new UIAbilityPanel(this, new UILayout(new UIMargin(0.02f, .3f, 0.02f, 1.7f), UIMarginType.Absolute, UIPosition.Left | UIPosition.Top, UIPosition.Left | UIPosition.Bottom, healthBar));
            SetupControls( );
            foreach (Ability ability in playerComponent.SecondaryWeapon.Abilities( )) {
                ability.GestureInputRequested += Ability_GestureInputRequested;
                abilityPanel.Add(ability);
                controlPanel.Add(ability.Name, ability.Gesture);
            }


            base.Load( );
        }

        private void Ability_GestureInputRequested (Ability ability, Ability.GestureCompletedDelegate callback) {
            controlPanel.AcceptingGestures = true;
            controlPanel.Preview = ability.Preview;
            currentGestureAbility = ability;
        }

        private void SetupControls ( ) {
            controlPanel = new UIGesturePanel(this, new UILayout(new UIMargin(abilityPanel.Layout.Width, 6f / 5f * Window.Ratio - 0.15f, healthBar.Layout.Height, 2f), UIMarginType.Absolute)) { AcceptingGestures = false };
            controlPanel.OnGesturePerformed += (IEnumerable<(string name, float accuracy)> result) => {
                controlPanel.Preview = null;
                if (controlPanel.AcceptingGestures) {
                    float gestureAccuracy = 0f;
                    foreach ((string name, float accuracy) pair in result) {
                        if (pair.name == currentGestureAbility.Name) {
                            gestureAccuracy = pair.accuracy;
                        }
                    }
                    currentGestureAbility.Cast(gestureAccuracy);
                } else {
                    playerComponent.ActionRequested( );
                }
                controlPanel.AcceptingGestures = false;
            };

            leftButton = new UIControlButton(this, "l", new UILayout(new UIMargin(Window.Ratio * 2f / 5f, Window.Ratio * 2f / 5f + .1f, Window.Ratio * 2f / 5f, .05f), UIMarginType.Absolute, UIPosition.Right | UIPosition.Bottom, UIPosition.Right | UIPosition.Bottom));
            leftButton.Click += ( ) => playerEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Left);
            leftButton.Release += ( ) => playerEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Left);
            leftButton.Leave += ( ) => playerEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Left);

            rightButton = new UIControlButton(this, "r", new UILayout(new UIMargin(Window.Ratio * 2f / 5f, .05f, Window.Ratio * 2f / 5f, .05f), UIMarginType.Absolute, UIPosition.Right | UIPosition.Bottom, UIPosition.Right | UIPosition.Bottom));
            rightButton.Click += ( ) => playerEntity.SetComponentInfo(ComponentData.InputInclude, ActionMask.Right);
            rightButton.Release += ( ) => playerEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Right);
            rightButton.Leave += ( ) => playerEntity.SetComponentInfo(ComponentData.InputExclude, ActionMask.Right);
        }

        public override void Update (DeltaTime dt) {
            if (Math.Abs(Manager.FrameTime.TotalMilliseconds) < MAX_TIME_BETWEEN_UPDATES) {
                map.Update(dt);
                base.Update(dt);
                debugLabel.Color = Color.White;
            } else {
                debugLabel.Color = Color.Red;
            }
            debugLabel.Text = $"frame: {Manager.FrameTime.TotalMilliseconds:00.0}\n" +
                              $"update: {Manager.UpdateTime.TotalMilliseconds:00.0}\n" +
                            $"draw: {Manager.DrawTime.TotalMilliseconds:00.0}\n";
        }
    }
}