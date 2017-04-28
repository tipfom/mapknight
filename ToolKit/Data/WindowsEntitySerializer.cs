using mapKnight.Core.World.Serialization;
using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.World;
using mapKnight.ToolKit.Controls;

namespace mapKnight.ToolKit.Data {
    public class WindowsEntitySerializer : IEntitySerializer {
        public IEnumerable<Tuple<DataID, DataType, object>> GetData(Entity entity) {
            foreach (Component c in entity.GetComponents( )) {
                if(c is IUserControlComponent) {
                    foreach(Tuple<DataID, DataType, object> data in (c as IUserControlComponent).CollectData( )) {
                        yield return data;
                    }
                }
            }
        }

        public EntityID GetID(Entity entity) {
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
                case "Fir":
                    return EntityID.Fir;
                case "Lenny":
                    return EntityID.Npc_Lenny;
                case "Drillbomb":
                    return EntityID.Drillbomb;
                case "Oak":
                    return EntityID.Oak;
                default:
                    return EntityID.Error;
            }
        }

        public void Instantiate(EntityID id, Dictionary<DataID, object> data, Vector2 position, IEntityWorld world) {
            Entity entity = null;

            switch (id) {
                case EntityID.Canone:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[0].Create(position, world, false);
                    break;
                case EntityID.Guardian:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[1].Create(position, world, false);
                    break;
                case EntityID.Landmine:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[2].Create(position, world, false);
                    break;
                case EntityID.Moonball:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[3].Create(position, world, false);
                    break;
                case EntityID.Plugger:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[4].Create(position, world, false);
                    break;
                case EntityID.Sepler:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[5].Create(position, world, false);
                    break;
                case EntityID.Shark:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[6].Create(position, world, false);
                    break;
                case EntityID.Shell:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[7].Create(position, world, false);
                    break;
                case EntityID.Slime:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[8].Create(position, world, false);
                    break;
                case EntityID.Platform_Copper:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[9].Create(position, world, false);
                    break;
                case EntityID.Fir:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[10].Create(position, world, false);
                    break;
                case EntityID.Npc_Lenny:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[11].Create(position, world, false);
                    break;
                case EntityID.Drillbomb:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[12].Create(position, world, false);
                    break;
                case EntityID.Oak:
                    entity = EntityListBox.FINAL_CONFIGURATIONS[13].Create(position, world, false);
                    break;
            }

            entity?.Load(data);
        }
    }
}
