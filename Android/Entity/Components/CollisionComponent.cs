using System;
using mapKnight.Basic;

namespace mapKnight.Android.Entity.Components {
    public class CollisionComponent : Component {
        public CollisionComponent (Entity owner) : base (owner) {

        }
        public override void Update (float dt) {
            // handles movement to a certain point, if one is given
            Transform lastTransform = Owner.Transform;
            while (Owner.HasComponentInfo (Type.Collision)) {
                Info pendingComponentInfo = Owner.GetComponentInfo (Type.Collision);

                Transform targetTransform = (Transform)pendingComponentInfo.Data;
                bool[ ] moveResult = { moveHorizontally (lastTransform, targetTransform), moveVertically (lastTransform, targetTransform) };
                Owner.SetComponentInfo (pendingComponentInfo.Sender, Type.Collision, Action.Result, moveResult);
                lastTransform = targetTransform;
            }
            Owner.Transform = lastTransform;
        }


        private bool moveHorizontally (Transform oldTransform, Transform targetTransform) {
            // returns true if any collision happened and modifies the transform
            if (targetTransform.Center.X > oldTransform.Center.X) {
                if (targetTransform.TR.X >= Owner.Owner.Bounds.X) {
                    targetTransform.TranslateHorizontally (Owner.Owner.Bounds.X - targetTransform.Bounds.X / 2);
                    return true;
                }

                // moves to the right
                for (int x = (int)oldTransform.TR.X; x <= (int)(targetTransform.TR.X); x++) {
                    for (int y = (int)oldTransform.BL.Y; y <= ((oldTransform.TR.Y == Math.Floor (oldTransform.TR.Y)) ? (int)(oldTransform.TR.Y - 1) : (int)oldTransform.TR.Y); y++) {
                        if (x > Owner.Owner.Bounds.X || Owner.Owner.HasCollider (x, y)) {
                            targetTransform.TranslateHorizontally (x - targetTransform.Bounds.X / 2);
                            return true;
                        }
                    }
                }
            } else if (targetTransform.Center.X < oldTransform.Center.X) {
                // moves to the left
                if (targetTransform.BL.X <= 0) {
                    targetTransform.TranslateHorizontally (targetTransform.Bounds.X / 2);
                    return true;
                }

                for (int x = (int)oldTransform.BL.X; x >= (int)(targetTransform.BL.X); x--) {
                    for (int y = (int)oldTransform.BL.Y; y <= ((oldTransform.TR.Y == Math.Floor (oldTransform.TR.Y)) ? (int)(oldTransform.TR.Y - 1) : (int)oldTransform.TR.Y); y++) {
                        if (x < 0 || Owner.Owner.HasCollider (x, y)) {
                            targetTransform.TranslateHorizontally (x + 1 + targetTransform.Bounds.X / 2);
                            return true;
                        }
                    }
                }
            } else {
                // no movement happens
                return false;
            }

            // no collision happened
            return false;
        }

        private bool moveVertically (Transform oldTransform, Transform targetTransform) {
            if (oldTransform.Center.Y < targetTransform.Center.Y) {
                if (targetTransform.TR.Y >= Owner.Owner.Bounds.Y - 1) {
                    targetTransform.TranslateVertically (Owner.Owner.Bounds.Y - 1 - targetTransform.Bounds.Y / 2);
                    return true;
                }

                // goes up
                for (int y = (int)oldTransform.TR.Y; y <= (int)targetTransform.TR.Y; y++) {
                    for (int x = (int)targetTransform.BL.X; x <= ((targetTransform.TR.X == Math.Floor (targetTransform.TR.X)) ? (int)targetTransform.TR.X - 1 : (int)targetTransform.TR.X); x++) {
                        if (Owner.Owner.HasCollider (x, y)) {
                            targetTransform.TranslateVertically (y - targetTransform.Bounds.Y / 2f);
                            return true;
                        }
                    }
                }
            } else if (oldTransform.Center.Y > targetTransform.Center.Y) {
                if (targetTransform.BL.Y <= 0) {
                    targetTransform.TranslateVertically (targetTransform.Bounds.Y / 2);
                    return true;
                }

                // goes down
                for (int y = (int)oldTransform.BL.Y; y >= (int)targetTransform.BL.Y; y--) {
                    for (int x = (int)targetTransform.BL.X; x <= ((targetTransform.TR.X == Math.Floor (targetTransform.TR.X)) ? (int)targetTransform.TR.X - 1 : (int)targetTransform.TR.X); x++) {
                        if (Owner.Owner.HasCollider (x, y)) {
                            targetTransform.TranslateVertically (y + 1 + targetTransform.Bounds.Y / 2f);
                            return true;
                        }
                    }
                }
            } else {
                // no movement on y
                return false;
            }

            // no collision
            return false;
        }
    }
}