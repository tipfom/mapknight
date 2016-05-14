using System.Collections.Generic;
using mapKnight.Android.CGL;
using mapKnight.Android.Map;
using mapKnight.Basic;

namespace mapKnight.Android.Entity {
    public class Map : Android.Map.Map, Entity.IContainer {
        private int currentID = 0;
        private CGLEntityRenderer renderer;
        private HashSet<Entity> entities;

        public Map (string mapname, CGLCamera camera) : base (mapname) {
            Gravity = new Vector2 (0, -10);
            Camera = camera;
            renderer = new CGLEntityRenderer ( );
            entities = new HashSet<Entity> ( );
        }

        public CGLCamera Camera { get; private set; }

        public Vector2 Gravity { get; private set; }

        public Entity.IRenderer Renderer { get { return renderer; } }

        public int CreateID () { return ++currentID; }

        public bool HasCollider (int x, int y) {
            return base.GetTileL2 (x, y).Mask.HasFlag (Tile.TileMask.COLLISION);
        }

        public void Update (float dt) {
            foreach (Entity entity in entities) {
                entity.Update (dt);
                Log.Print (this, entity.Transform.Center.ToString ( ));
            }
            this.renderer.Update ( );
        }

        public void Draw () {
            this.renderer.Draw ( );
        }

        public void Add (Entity entity) {
            entities.Add (entity);
        }

        public bool IsOnScreen (Entity entity) {
            return Camera.ScreenCentre - entity.Transform.Center < Camera.DrawRange;
        }
    }
}