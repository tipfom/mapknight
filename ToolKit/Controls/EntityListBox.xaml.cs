using mapKnight.Core.World;
using mapKnight.Core.World.Components;
using mapKnight.ToolKit.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System;
using Microsoft.Xna.Framework.Graphics;
using mapKnight.ToolKit.Data.Components;

namespace mapKnight.ToolKit.Controls {
    /// <summary>
    /// Interaktionslogik für EntityListBox.xaml
    /// </summary>
    public partial class EntityListBox : UserControl {
        public delegate void OnSelectionChanged(EntityData oldSelection, EntityData newSelection);
        public event OnSelectionChanged SelectionChanged;

        private ObservableCollection<EntityData> entityCollection = new ObservableCollection<EntityData>( );
        private Entity.Configuration[ ] shadowConfigurations = new Entity.Configuration[ ] {
            new Entity.Configuration("Canone", new Core.Vector2( 1.2f, 0.85161f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Guardian", new Core.Vector2(3.405405405f, 2f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Landmine", new Core.Vector2(0.7f, 0.21875f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Moonball", new Core.Vector2(2f, 2f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Plugger", new Core.Vector2(1.35f, 1f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Sepler", new Core.Vector2(1f * 23f / 17f, 15f / 17f * 23f / 15f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Shark", new Core.Vector2(0.540540f * 29f/15f, 0.540540f*31f/15f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Shell", new Core.Vector2(0.75f, 0.875f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Slime", new Core.Vector2(1f, 0.95238095f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Copper Platform", new Core.Vector2(1.5f, 1f)) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            }
        };
        private Entity.Configuration[ ] finalConfigurations = new Entity.Configuration[ ] {
            new Entity.Configuration("Canone", new Core.Vector2( 1.2f, 0.85161f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Guardian", new Core.Vector2(3.405405405f, 2f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Landmine", new Core.Vector2(0.7f, 0.21875f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Moonball", new Core.Vector2(2f, 2f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Plugger", new Core.Vector2(1.35f, 1f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Sepler", new Core.Vector2(1f * 23f / 17f, 15f / 17f * 23f / 15f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Shark", new Core.Vector2(0.540540f * 29f/15f, 0.540540f*31f/15f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Shell", new Core.Vector2(0.75f, 0.875f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Slime", new Core.Vector2(1f, 0.95238095f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( ),
                    new SlimeDataComponent.Configuration( )
                }
            },
            new Entity.Configuration("Copper Platform", new Core.Vector2(1.5f, 1f)) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( ),
                    new PlatformDataComponent.Configuration( )
                }
            }
        };

        public EntityListBox( ) {
            InitializeComponent( );
            listbox_entities.ItemsSource = entityCollection;
        }

        public void Init(GraphicsDevice g) {
            entityCollection.Add(new EntityData("Canone", "canone", g));
            entityCollection.Add(new EntityData("Guardian", "guardian_tent", g));
            entityCollection.Add(new EntityData("Landmine", "landmine", g));
            entityCollection.Add(new EntityData("Moonball", "moonball", g));
            entityCollection.Add(new EntityData("Plugger", "plugger", g));
            entityCollection.Add(new EntityData("Sepler", "sepler", g));
            entityCollection.Add(new EntityData("Shark", "shark", g));
            entityCollection.Add(new EntityData("Shell", "shell", g));
            entityCollection.Add(new EntityData("Slime", "slime", g));
            entityCollection.Add(new EntityData("Copper Platform", "platform_copper", g));

            listbox_entities.SelectedIndex = 0;
        }

        private void listbox_entities_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectionChanged?.Invoke((EntityData)e.RemovedItems[0], (EntityData)e.AddedItems[0]);
            if (e.AddedItems.Count > 0) {
                textblock_name.Text = ((EntityData)e.AddedItems[0]).Name;
            } else {
                textblock_name.Text = "";
            }
        }

        public EntityData Find(string name) {
            return entityCollection.FirstOrDefault(entityData => entityData.Name == name);
        }

        public Entity.Configuration GetCurrentShadowConfiguration( ) {
            return shadowConfigurations[listbox_entities.SelectedIndex];
        }

        public Entity.Configuration GetCurrentFinalConfiguration( ) {
            return finalConfigurations[listbox_entities.SelectedIndex];
        }

        public class ShadowComponent : Component {
            public ShadowComponent(Entity owner) : base(owner) {
                owner.Domain = EntityDomain.Temporary;
            }

            public new class Configuration : Component.Configuration {
                public override Component Create(Entity owner) {
                    return new ShadowComponent(owner);
                }
            }
        }

        public class ActiveComponent : Component {
            public ActiveComponent(Entity owner) : base(owner) {
                owner.Domain = EntityDomain.Enemy;
            }

            public new class Configuration : Component.Configuration {
                public override Component Create(Entity owner) {
                    return new ActiveComponent(owner);
                }
            }
        }
    }
}
