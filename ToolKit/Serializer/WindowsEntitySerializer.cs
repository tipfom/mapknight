using mapKnight.Core.World.Serialization;
using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.ToolKit.Controls;
using mapKnight.ToolKit.Data;
using mapKnight.ToolKit.Data.Components;
using mapKnight.Core.World.Components;

namespace mapKnight.ToolKit.Serializer {
    public class WindowsEntitySerializer : IEntitySerializer {
        private static readonly Vector2 CANONE_SIZE = new Vector2(1.2f, 0.85161f);
        private static readonly Vector2 GUARDIAN_SIZE = new Vector2(3.405405405f, 2f);
        private static readonly Vector2 LANDMINE_SIZE = new Vector2(0.7f, 0.21875f);
        private static readonly Vector2 MOONBALL_SIZE = new Vector2(1, 0.538461538f);
        private static readonly Vector2 PLUGGER_SIZE = new Vector2(1.35f, 1f);
        private static readonly Vector2 SEPLER_SIZE = new Vector2(1f * 23f / 17f, 15f / 17f * 23f / 15f);
        private static readonly Vector2 SHARK_SIZE = new Vector2(0.540540f * 29f / 15f, 0.540540f * 31f / 15f);
        private static readonly Vector2 SHELL_SIZE = new Vector2(0.75f, 0.875f);
        private static readonly Vector2 SLIME_SIZE = new Vector2(1f, 0.95238095f);
        private static readonly Vector2 PLATFORM_COPPER_SIZE = new Vector2(1.5f, 1f);
        private static readonly Vector2 NPC_LENNY_SIZE = new Vector2(1.1f, 1.1f);
        private static readonly Vector2 DRILLBOMB_SIZE = new Vector2(19f / 29f, 1f);
        private static readonly Vector2 MAPLE_TREE_SIZE = new Vector2(3.5f * 46f / 71f, 3.5f * 59f / 71f);
        private static readonly Vector2 CHERRY_TREE_SIZE = new Vector2(3.5f * 66f / 71f, 3.5f);

        public static readonly Entity.Configuration[ ] SHADOW_CONFIGURATIONS = new Entity.Configuration[ ] {
            new Entity.Configuration("Canone", CANONE_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Guardian", GUARDIAN_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Landmine", LANDMINE_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Moonball", MOONBALL_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Plugger", PLUGGER_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Sepler", SEPLER_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Shark", SHARK_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Shell", SHELL_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Slime", SLIME_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Copper Platform", PLATFORM_COPPER_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Lenny", NPC_LENNY_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Drillbomb", DRILLBOMB_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Maple Tree", MAPLE_TREE_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
            new Entity.Configuration("Cherry Tree", CHERRY_TREE_SIZE) { Components = new ComponentList( ) {
                    new ShadowComponent.Configuration( )
                }
            },
        };
        public static readonly Entity.Configuration[ ] FINAL_CONFIGURATIONS = new Entity.Configuration[ ] {
            new Entity.Configuration("Canone", CANONE_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Guardian", GUARDIAN_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Landmine", LANDMINE_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Moonball", MOONBALL_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( ),
                    new MoonballDataComponent.Configuration( )
                }
            },
            new Entity.Configuration("Plugger", PLUGGER_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Sepler", SEPLER_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Shark", SHARK_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Shell", SHELL_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Slime", SLIME_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( ),
                    new SlimeDataComponent.Configuration( )
                }
            },
            new Entity.Configuration("Copper Platform", PLATFORM_COPPER_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( ),
                    new PlatformDataComponent.Configuration( )
                }
            },
            new Entity.Configuration("Lenny", NPC_LENNY_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( ),
                    new NPCDataComponent.Configuration( )
                }
            },
            new Entity.Configuration("Drillbomb", DRILLBOMB_SIZE) {Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Maple Tree", MAPLE_TREE_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
            new Entity.Configuration("Cherry Tree", CHERRY_TREE_SIZE) { Components = new ComponentList( ) {
                    new ActiveComponent.Configuration( )
                }
            },
        };

        public IEnumerable<Tuple<DataID, DataType, object>> GetData (Entity entity) {
            foreach (Component c in entity.GetComponents( )) {
                if (c is IUserControlComponent) {
                    foreach (Tuple<DataID, DataType, object> data in (c as IUserControlComponent).CollectData( )) {
                        yield return data;
                    }
                }
            }
        }

        public EntityID GetID (Entity entity) {
            switch (entity.Name) {
                case "Canone":
                    return EntityID.Canone;
                case "Guardian":
                    return EntityID.Guardian;
                case "Landmine":
                    return EntityID.Landmine;
                case "Moonball":
                    return EntityID.Moonball;
                case "Plugger":
                    return EntityID.Plugger;
                case "Sepler":
                    return EntityID.Sepler;
                case "Shark":
                    return EntityID.Shark;
                case "Shell":
                    return EntityID.Shell;
                case "Slime":
                    return EntityID.Slime;
                case "Copper Platform":
                    return EntityID.Platform_Copper;
                case "Lenny":
                    return EntityID.Npc_Lenny;
                case "Drillbomb":
                    return EntityID.Drillbomb;
                case "Maple Tree":
                    return EntityID.Tree_Maple;
                case "Cherry Tree":
                    return EntityID.Tree_Cherry;
                default:
                    return EntityID.Error;
            }
        }

        public void Instantiate (EntityID id, Dictionary<DataID, object> data, Vector2 position, IEntityWorld world) {
            Entity entity = null;

            switch (id) {
                case EntityID.Canone:
                    entity = FINAL_CONFIGURATIONS[0].Create(position, world, false);
                    break;
                case EntityID.Guardian:
                    entity = FINAL_CONFIGURATIONS[1].Create(position, world, false);
                    break;
                case EntityID.Landmine:
                    entity = FINAL_CONFIGURATIONS[2].Create(position, world, false);
                    break;
                case EntityID.Moonball:
                    entity = FINAL_CONFIGURATIONS[3].Create(position, world, false);
                    break;
                case EntityID.Plugger:
                    entity = FINAL_CONFIGURATIONS[4].Create(position, world, false);
                    break;
                case EntityID.Sepler:
                    entity = FINAL_CONFIGURATIONS[5].Create(position, world, false);
                    break;
                case EntityID.Shark:
                    entity = FINAL_CONFIGURATIONS[6].Create(position, world, false);
                    break;
                case EntityID.Shell:
                    entity = FINAL_CONFIGURATIONS[7].Create(position, world, false);
                    break;
                case EntityID.Slime:
                    entity = FINAL_CONFIGURATIONS[8].Create(position, world, false);
                    break;
                case EntityID.Platform_Copper:
                    entity = FINAL_CONFIGURATIONS[9].Create(position, world, false);
                    break;
                case EntityID.Npc_Lenny:
                    entity = FINAL_CONFIGURATIONS[10].Create(position, world, false);
                    break;
                case EntityID.Drillbomb:
                    entity = FINAL_CONFIGURATIONS[11].Create(position, world, false);
                    break;
                case EntityID.Tree_Maple:
                    entity = FINAL_CONFIGURATIONS[12].Create(position, world, false);
                    break;
                case EntityID.Tree_Cherry:
                    entity = FINAL_CONFIGURATIONS[13].Create(position, world, false);
                    break;
            }

            entity?.Load(data);
        }

        public class ShadowComponent : Component {
            public ShadowComponent (Entity owner) : base(owner) {
                owner.Domain = EntityDomain.Temporary;
            }

            public new class Configuration : Component.Configuration {
                public override Component Create (Entity owner) {
                    return new ShadowComponent(owner);
                }
            }
        }

        public class ActiveComponent : Component {
            public ActiveComponent (Entity owner) : base(owner) {
                owner.Domain = EntityDomain.Enemy;
            }

            public new class Configuration : Component.Configuration {
                public override Component Create (Entity owner) {
                    return new ActiveComponent(owner);
                }
            }
        }
    }
}
