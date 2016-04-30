using System;
using System.Collections.Generic;
using mapKnight.Android.Map;
using mapKnight.Basic;

namespace mapKnight.Android.Physics {
    public class Map : Android.Map.Map {
        public const int COLLISION_LAYER = 1;

        private List<Entity> addedEntitys = new List<Entity> ( );
        private fVector2D Gravity = new fVector2D (0, -10);

        public Map (string name) : base (name) {

        }

        public void AddEntity (Entity entity) {
            addedEntitys.Add (entity);
        }

        public void RemoveEntity (Entity entity) {
            if (addedEntitys.Contains (entity)) {
                addedEntitys.Remove (entity);
            }
        }

        public void Step (float time) {
            time /= 1000f;

            foreach (Entity entity in addedEntitys) {

                if (entity.CollisionMask.HasFlag (Flag.Map)) {
                    // calculate new positions
                    fPoint movement = new fPoint (entity.Velocity.X * time, entity.Velocity.Y * time);

                    ////secure that the entity does not move out of the map
                    //if (entity.AABB.A.X + movement.X < 0) {
                    //    movement.X = -entity.AABB.A.X;
                    //} else if (entity.AABB.B.X + movement.X > this.Size.Width - 1) {
                    //    movement.X = this.Size.Width - entity.AABB.B.X - 1;
                    //}
                    //if (entity.AABB.A.Y + movement.X < 0) {
                    //    movement.Y = -entity.AABB.A.Y;
                    //} else if (entity.AABB.B.Y + movement.Y > this.Size.Height) {
                    //    movement.Y = this.Size.Height - entity.AABB.B.Y;
                    //}

                    // move on x coordinate
                    if (entity.Velocity.X > 0) {
                        if (entity.AABB.B.X >= this.Size.Width - 1) {
                            entity.AABB.Translate (this.Size.Width - 1 - entity.AABB.Bounds.X / 2, entity.AABB.Centre.Y);
                            entity.Velocity.X = 0;
                            goto MOVEDX;
                        }

                        // moves to the right
                        for (int x = (int)entity.AABB.B.X; x <= (int)(entity.AABB.B.X + movement.X); x++) {
                            for (int y = (int)entity.AABB.A.Y; y <= ((entity.AABB.B.Y == Math.Floor (entity.AABB.B.Y)) ? (int)(entity.AABB.B.Y - 1) : (int)entity.AABB.B.Y); y++) {
                                if (x == this.Size.Width || GetTile (x, y, COLLISION_LAYER).Mask.HasFlag (Tile.TileMask.COLLISION)) {
                                    entity.AABB.Translate (x - entity.AABB.Bounds.X / 2, entity.AABB.Centre.Y);
                                    entity.Velocity.X = 0;
                                    goto MOVEDX;
                                }
                            }
                        }
                    } else if (entity.Velocity.X < 0) {
                        // moves to the left
                        if (entity.AABB.A.X <= 0) {
                            entity.AABB.Translate (entity.AABB.Bounds.X / 2, entity.AABB.Centre.Y);
                            entity.Velocity.X = 0;
                            goto MOVEDX;
                        }

                        for (int x = (int)entity.AABB.A.X; x >= (int)(entity.AABB.A.X + movement.X); x--) {
                            for (int y = (int)entity.AABB.A.Y; y <= ((entity.AABB.B.Y == Math.Floor (entity.AABB.B.Y)) ? (int)(entity.AABB.B.Y - 1) : (int)entity.AABB.B.Y); y++) {
                                if (x < 0 || GetTile (x, y, COLLISION_LAYER).Mask.HasFlag (Tile.TileMask.COLLISION)) {
                                    entity.AABB.Translate (x + 1 + entity.AABB.Bounds.X / 2, entity.AABB.Centre.Y);
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
                        if (entity.AABB.B.Y >= this.Size.Height - 1) {
                            entity.AABB.Translate (entity.AABB.Centre.X, this.Size.Height - 1 - entity.AABB.Bounds.Y / 2);
                            entity.Velocity.Y = 0;
                            goto MOVEDY;
                        }

                        // goes up
                        for (int y = (int)entity.AABB.B.Y; y <= (int)(entity.AABB.B.Y + movement.Y); y++) {
                            for (int x = (int)entity.AABB.A.X; x <= ((entity.AABB.B.X == Math.Floor (entity.AABB.B.X)) ? (int)entity.AABB.B.X - 1 : (int)entity.AABB.B.X); x++) {
                                if (y == this.Size.Height || GetTile (x, y, COLLISION_LAYER).Mask.HasFlag (Tile.TileMask.COLLISION)) {
                                    entity.AABB.Translate (entity.AABB.Centre.X, y - entity.AABB.Bounds.Y / 2f);
                                    entity.Velocity.Y = 0;
                                    goto MOVEDY;
                                }
                            }
                        }
                    } else if (entity.Velocity.Y < 0) {
                        if (entity.AABB.A.Y <= 0) {
                            entity.AABB.Translate (entity.AABB.Centre.X, entity.AABB.Bounds.Y / 2);
                            entity.Velocity.Y = 0;
                            goto MOVEDY;
                        }

                        // goes down
                        for (int y = (int)entity.AABB.A.Y; y >= (int)(entity.AABB.A.Y + movement.Y); y--) {
                            for (int x = (int)entity.AABB.A.X; x <= ((entity.AABB.B.X == Math.Floor (entity.AABB.B.X)) ? (int)entity.AABB.B.X - 1 : (int)entity.AABB.B.X); x++) {
                                if (y == -1 || GetTile (x, y, COLLISION_LAYER).Mask.HasFlag (Tile.TileMask.COLLISION)) {
                                    entity.AABB.Translate (entity.AABB.Centre.X, y + 1 + entity.AABB.Bounds.Y / 2);
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
            }
        }
    }
}
