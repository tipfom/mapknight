using System.Collections.Generic;

using mapKnight.Android.CGL.Entity;
using mapKnight.Basic;

namespace mapKnight.Android.CGL {
    public class Character : CGLEntity {
        public ChangingProperty Energy;

        private float moveSpeed;
        private float jumpSpeed;


        public Character (int health, int energy, string name, int weight, fSize bounds, List<CGLBoundedPoint> boundedpoints, List<CGLAnimation> animations, CGLSet set, float movespeed, float jumpspeed)
            : base (health, new fPoint (0f, 0f), name, weight, bounds, boundedpoints, animations, set) {
            Energy = new ChangingProperty (energy);

            moveSpeed = movespeed;
            jumpSpeed = jumpspeed;
        }

        public void Move (Direction dir) {
            switch (dir) {
            case Direction.Left:
                this.Velocity.X = -moveSpeed;
                break;
            case Direction.Right:
                this.Velocity.X = moveSpeed;
                break;
            }
        }

        protected override fVector2D GetCentreOnScreen (CGLCamera camera) {
            return camera.CharacterCentreOnScreen;
        }

        public void ResetMovement () {
            this.Velocity.X = 0;
        }

        public void Jump () {
            this.Velocity.Y = jumpSpeed;
        }
    }
}

