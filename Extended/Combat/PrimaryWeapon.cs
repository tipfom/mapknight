using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Core.Graphics;
using mapKnight.Core.World;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Components.Movement;
using mapKnight.Core.World.Components;
using mapKnight.Extended.Graphics.Animation;
using System;
using System.Timers;

namespace mapKnight.Extended.Combat {
    public class PrimaryWeapon {
#if DEBUG
        private const int PRIMARY_WEAPON_HITBOX_SPECIES = -3;
        private static Spritebatch2D PRIMARY_WEAPON_TEXTURE;

        private float[ ] sizeVertices;
        private float[ ] transformedVertices;
#endif

        public readonly string Name;
        public readonly int ID;
        public readonly float Damage;
        public readonly int Cooldown;
        public readonly int AttackTime;
        public readonly string Texture;
        public readonly VertexAnimationData AnimationData;

        private MotionComponent motionComponent;
        private Vector2 hitboxOffset;
        private Transform hitbox;
        private Entity owner;
        private Timer timer;
        private int nextHitTime;

        public PrimaryWeapon (string Name, int ID, float Damage, int Cooldown, string Texture, int AttackTime, VertexAnimationData AnimationData, Transform hitbox, Entity owner) {
            this.Name = Name;
            this.ID = ID;
            this.Damage = Damage;
            this.Cooldown = Cooldown;
            this.AttackTime = AttackTime;
            this.Texture = Texture;
            this.AnimationData = AnimationData;
            this.hitbox = hitbox;
            this.hitboxOffset = hitbox.Center;
            this.owner = owner;
            this.timer = new Timer(AttackTime);
            this.timer.Elapsed += Timer_Elapsed;
        }

        public void Prepare ( ) {
            motionComponent = owner.GetComponent<MotionComponent>( );
#if DEBUG
            if (PRIMARY_WEAPON_TEXTURE == null) {
                PRIMARY_WEAPON_TEXTURE = new Spritebatch2D(new Dictionary<string, int[ ]>( ) { ["0"] = new int[ ] { 0, 0, 1, 1 } }, Texture2D.CreateEmpty( ));
            }
            if (!owner.World.Renderer.HasTexture(PRIMARY_WEAPON_HITBOX_SPECIES)) {
                owner.World.Renderer.AddTexture(PRIMARY_WEAPON_HITBOX_SPECIES, PRIMARY_WEAPON_TEXTURE);
            }

            UpdateSizeVertices( );
            Window.Changed += UpdateSizeVertices;
            transformedVertices = new float[8];
#endif
        }

#if DEBUG
        private void UpdateSizeVertices ( ) {
            sizeVertices = (hitbox.Size * owner.World.VertexSize).ToQuad( );
        }

        public void Destroy ( ) {
            Window.Changed -= UpdateSizeVertices;
        }
#endif

        public bool Update () {
            if (nextHitTime > Environment.TickCount) return false;
            hitbox.Center = owner.Transform.Center + new Vector2(motionComponent.ScaleX * hitboxOffset.X, hitboxOffset.Y);
            for(int i = 0; i< owner.World.Entities.Count; i++) {
                Entity entity = owner.World.Entities[i];
                if (entity.Domain == EntityDomain.Enemy && entity.Transform.Touches(hitbox)) {
                    return true;
                }
            }
            return false;
        }

        public void Attack ( ) {
            nextHitTime = Environment.TickCount + Cooldown + AttackTime;
            timer.Start( );
        }

        public void Abort ( ) {
            timer.Stop( );
        }

        private void Timer_Elapsed (object sender, ElapsedEventArgs e) {
            for (int i = 0; i < owner.World.Entities.Count; i++) {
                Entity entity = owner.World.Entities[i];
                if (entity.Domain == EntityDomain.Enemy && entity.Transform.Touches(hitbox)) {
                    entity.SetComponentInfo(ComponentData.Damage, owner, Damage);
                }
            }
            timer.Stop( );
        }

#if DEBUG
        public void Draw ( ) {
            owner.World.Renderer.QueueVertexData(PRIMARY_WEAPON_HITBOX_SPECIES, ConstructVertexData( ));
        }

        private IEnumerable<VertexData> ConstructVertexData ( ) {
            Vector2 position = owner.PositionOnScreen + new Vector2(motionComponent.ScaleX * hitboxOffset.X, hitboxOffset.Y) * owner.World.VertexSize;
            Mathf.TranslateAndScale(sizeVertices, ref transformedVertices, position.X, position.Y, 1f, 1f);
            yield return new VertexData(transformedVertices, "0", Color.Yellow);
        }
#endif
    }
}
