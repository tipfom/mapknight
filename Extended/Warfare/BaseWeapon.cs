using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Core.World.Components;

namespace mapKnight.Extended.Warfare {
    public class BaseWeapon {
#if DEBUG
        private const int BASE_WEAPON_HITBOX_SPECIES = -3;
        private static Spritebatch2D BASE_WEAPON_TEXTURE;

        private float[ ] sizeVertices;
        private float[ ] transformedVertices;
#endif

        public readonly string Name;
        public readonly int ID;
        public readonly float Damage;

        private MotionComponent motionComponent;
        private Vector2 hitboxOffset;
        private Transform hitbox;
        private Entity owner;

        public BaseWeapon (string Name, int ID, float Damage, Transform hitbox, Entity owner) {
            this.Name = Name;
            this.ID = ID;
            this.Damage = Damage;
            this.hitbox = hitbox;
            this.hitboxOffset = hitbox.Center;
            this.owner = owner;
        }

        public void Prepare ( ) {
            motionComponent = owner.GetComponent<MotionComponent>( );
#if DEBUG
            if (BASE_WEAPON_TEXTURE == null) {
                BASE_WEAPON_TEXTURE = new Spritebatch2D(new Dictionary<string, int[ ]>( ) { ["0"] = new int[ ] { 0, 0, 1, 1 } }, Texture2D.CreateEmpty( ));
                owner.World.Renderer.AddTexture(BASE_WEAPON_HITBOX_SPECIES, BASE_WEAPON_TEXTURE);            
            }

            UpdateSizeVertices( );
            Window.Changed += UpdateSizeVertices;
            transformedVertices = new float[8];
#endif
        }

#if DEBUG
        private void UpdateSizeVertices ( ) {
            sizeVertices = (owner.Transform.Size * owner.World.VertexSize).ToQuad( );
        }

        public void Destroy ( ) {
            Window.Changed -= UpdateSizeVertices;
        }
#endif

        public bool Update () {
            hitbox.Center = owner.Transform.Center + new Vector2(motionComponent.ScaleX * hitboxOffset.X, hitboxOffset.Y);
            for(int i = 0; i< owner.World.Entities.Count; i++) {
                Entity entity = owner.World.Entities[i];
                if (entity.Domain == EntityDomain.Enemy && entity.Transform.Touches(hitbox)) {
                    return true;
                }
            }
            return false;
        }

        public int Attack ( ) {
            int hitCount = 0;
            hitbox.Center = owner.Transform.Center + new Vector2(motionComponent.ScaleX * hitboxOffset.X, hitboxOffset.Y);
            for (int i = 0; i < owner.World.Entities.Count; i++) {
                Entity entity = owner.World.Entities[i];
                if (entity.Domain == EntityDomain.Enemy && entity.Transform.Touches(hitbox)) {
                    hitCount++;
                    entity.SetComponentInfo(ComponentData.Damage, owner, Damage);
                }
            }
            return hitCount;
        }

#if DEBUG
        public void Draw ( ) {
            owner.World.Renderer.QueueVertexData(BASE_WEAPON_HITBOX_SPECIES, ConstructVertexData( ));
        }

        private IEnumerable<VertexData> ConstructVertexData ( ) {
            Vector2 position = owner.PositionOnScreen + new Vector2(motionComponent.ScaleX * hitboxOffset.X, hitboxOffset.Y) * owner.World.VertexSize;
            Mathf.TranslateAndScale(sizeVertices, ref transformedVertices, position.X, position.Y, 1f, 1f);
            yield return new VertexData(transformedVertices, "0", Color.Yellow);
        }
#endif
    }
}
