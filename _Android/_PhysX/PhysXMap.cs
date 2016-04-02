using System;
using System.Collections.Generic;

using mapKnight.Basic;

namespace mapKnight.Android.PhysX {
    public class PhysXMap : Map {
        public const int COLLISION_LAYER = 1;

        private List<PhysXEntity> addedEntitys = new List<PhysXEntity> ();
        private fVector2D Gravity = new fVector2D (0, -10);

        public PhysXMap (string name) : base (name) {

        }

        public void AddEntity (PhysXEntity entity) {
            addedEntitys.Add (entity);
        }

        public void RemoveEntity (PhysXEntity entity) {
            if (addedEntitys.Contains (entity)) {
                addedEntitys.Remove (entity);
            }
        }

        public void Step (float time) {
            time /= 1000f;

            foreach (PhysXEntity entity in addedEntitys) {

                if (entity.CollisionMask.HasFlag (PhysXFlag.Map)) {
                    // calculate new positions
                    fPoint movement = new fPoint (entity.Velocity.X * time, entity.Velocity.Y * time);

                    // move on x coordinate
                    if (entity.Velocity.X > 0) {
                        // moves to the right
                        for (int x = (int)entity.AABB.B.X; x <= (int)(entity.AABB.B.X + movement.X); x++) {
                            for (int y = (int)entity.AABB.A.Y; y <= (int)entity.AABB.B.Y; y++) {
                                if (GetTile (x, y, COLLISION_LAYER).Mask.HasFlag (Tile.TileMask.COLLISION)) {
                                    entity.AABB.Translate (x - entity.AABB.Bounds.Width / 2, entity.AABB.Centre.Y);
                                    entity.Velocity.X = 0;
                                    goto MOVEDX;
                                }
                            }
                        }
                    } else if (entity.Velocity.X < 0) {
                        // moves to the left
                        for (int x = (int)entity.AABB.A.X; x >= (int)(entity.AABB.A.X + movement.X); x--) {
                            for (int y = (int)entity.AABB.A.Y; y <= (int)entity.AABB.B.Y; y++) {
                                if (GetTile (x, y, COLLISION_LAYER).Mask.HasFlag (Tile.TileMask.COLLISION)) {
                                    entity.AABB.Translate (x + 1 + entity.AABB.Bounds.Width / 2, entity.AABB.Centre.Y);
                                    entity.Velocity.X = 0;
                                    goto MOVEDX;
                                }
                            }
                        }
                    } else {
                        // no movement happens
                        goto MOVEDX;
                    }

                    // only gets called, when no collision happened
                    entity.AABB.Translate (entity.AABB.Centre.X + movement.X, entity.AABB.Centre.Y);

                    MOVEDX:

                    // move on Y
                    if (entity.Velocity.Y > 0) {
                        // goes up
                        for (int y = (int)entity.AABB.B.Y; y <= (int)(entity.AABB.B.Y + movement.Y); y++) {
                            for (int x = (int)entity.AABB.A.X; x <= ((entity.AABB.B.X == Math.Floor (entity.AABB.B.X)) ? (int)entity.AABB.B.X - 1 : (int)entity.AABB.B.X); x++) {
                                if (GetTile (x, y, COLLISION_LAYER).Mask.HasFlag (Tile.TileMask.COLLISION)) {
                                    entity.AABB.Translate (entity.AABB.Centre.X, y - entity.AABB.Bounds.Height / 2);
                                    entity.Velocity.Y = 0;
                                    goto MOVEDY;
                                }
                            }
                        }
                    } else if (entity.Velocity.Y < 0) {
                        // goes down
                        for (int y = (int)entity.AABB.A.Y; y >= (int)(entity.AABB.A.Y + movement.Y); y--) {
                            for (int x = (int)entity.AABB.A.X; x <= ((entity.AABB.B.X == Math.Floor (entity.AABB.B.X)) ? (int)entity.AABB.B.X - 1 : (int)entity.AABB.B.X); x++) {
                                if (GetTile (x, y, COLLISION_LAYER).Mask.HasFlag (Tile.TileMask.COLLISION)) {
                                    entity.AABB.Translate (entity.AABB.Centre.X, y + 1 + entity.AABB.Bounds.Height / 2);
                                    entity.Velocity.Y = 0;
                                    goto MOVEDY;
                                }
                            }
                        }
                    } else {
                        // no movement on y
                        goto MOVEDY;
                    }

                    entity.AABB.Translate (entity.AABB.Centre.X, entity.AABB.Centre.Y + movement.Y);
                }

                MOVEDY:
                entity.Velocity.Y += (this.Gravity.Y + entity.Acceleration.Y) * time;
                entity.Velocity.X += (this.Gravity.X + entity.Acceleration.X) * time;

                Content.Map.hitboxTiles.Clear ();
                Content.Map.hitboxTiles.Add (new Point ((int)entity.AABB.A.X, (int)entity.AABB.A.Y));
                Content.Map.hitboxTiles.Add (new Point ((int)entity.AABB.B.X, (int)entity.AABB.B.Y));
            }
        }
    }
}
