using System;
using System.Collections.Generic;

namespace mapKnight.Core.World.Serialization {
    public interface IEntitySerializer {
        // deserialization
        void Instantiate(EntityID id, Dictionary<DataID, object> data, Vector2 position, IEntityWorld world);

        // serialization
        IEnumerable<Tuple<DataID, DataType, object>> GetData(Entity entity);
        EntityID GetID(Entity entity);
    }
}
