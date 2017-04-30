using mapKnight.Core.World.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using mapKnight.Core;
using mapKnight.Core.World;

namespace mapKnight.Extended {
    public class MobileAndroidSerializer : IEntitySerializer {
        public IEnumerable<Tuple<DataID, DataType, object>> GetData(Entity entity) {
            yield break; // only deserialization
        }

        public EntityID GetID(Entity entity) {
            return EntityID.Error; // only deserialization
        }

        public void Instantiate(EntityID id, Dictionary<DataID, object> data, Vector2 position, IEntityWorld world) {
            switch (id) {
                case EntityID.Canone:
                    EntityCollection.Enemys.Turret.Create(position, world, false).Load(data);
                    break;
                case EntityID.Drillbomb:
                    EntityCollection.Obstacles.Drillbomb.Create(position, world, false).Load(data);
                    break;
                case EntityID.Fir:
                    // not ingame yet
                    break;
                case EntityID.Guardian:
                    EntityCollection.Enemys.Guardians.Tent.Create(position, world, false).Load(data);
                    break;
                case EntityID.Landmine:
                    EntityCollection.Obstacles.Landmine.Create(position, world, false).Load(data);
                    break;
                case EntityID.Moonball:
                    EntityCollection.Obstacles.Moonball.Create(position, world, false).Load(data);
                    break;
                case EntityID.Npc_Lenny:
                    EntityCollection.NPCs.Lenny.Create(position, world, false).Load(data);
                    break;
                case EntityID.Platform_Copper:
                    EntityCollection.Platforms.Copper.Create(position, world, false).Load(data);
                    break;
                case EntityID.Plugger:
                    EntityCollection.Enemys.Plugger.Create(position, world, false).Load(data);
                    break;
                case EntityID.Sepler:
                    EntityCollection.Enemys.Sepling.Create(position, world, false).Load(data);
                    break;
                case EntityID.Shark:
                    EntityCollection.Enemys.Shark.Create(position, world, false).Load(data);
                    break;
                case EntityID.Shell:
                    EntityCollection.Enemys.Shell.Create(position, world, false).Load(data);
                    break;
                case EntityID.Slime:
                    EntityCollection.Enemys.Slime.Create(position, world, false).Load(data);
                    break;
            }
        }
    }
}
