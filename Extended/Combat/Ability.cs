using System;
using mapKnight.Core;
using Android.Gestures;
using System.Collections.Generic;

namespace mapKnight.Extended.Combat {
    public abstract class Ability {
        private const float LONG_PRESS_TIME = 300f;

        public delegate void GestureCompletedDelegate (bool success, float precision);
        public delegate void GestureInputRequestDelegate (Ability ability, GestureCompletedDelegate callback);
        public delegate void UpdateRequiredDelegate (Ability ability);

        public readonly string Name;
        public readonly int Cooldown;
        public readonly string Texture;
        public readonly SecondaryWeapon Weapon;
        public readonly Vector2[ ] Preview;
        public readonly Gesture Gesture;

        public float Stride = 1f;
        public event GestureInputRequestDelegate GestureInputRequested;
        public event UpdateRequiredDelegate UpdateRequired;
        public AbilityMode Mode = AbilityMode.Ready;

        public Ability (SecondaryWeapon Weapon, string Name, int Cooldown, string Texture, Vector2[ ] Preview) {
            this.Name = Name;
            this.Cooldown = Cooldown;
            this.Texture = Texture;
            this.Weapon = Weapon;
            this.Preview = Preview;

            Gesture = new Gesture( );
            for (int i = 0; i < Preview.Length; i++)
                Gesture.AddStroke(new GestureStroke(GetGesturePoints(i)));
        }

        private IList<GesturePoint> GetGesturePoints (int offset) {
            List<GesturePoint> result = new List<GesturePoint>( );

            int i = offset;
            do {
                foreach (Vector2 vector in BuildDirectLine(Preview[i % Preview.Length], Preview[(i + 1) % Preview.Length], .05f))
                    result.Add(new GesturePoint(vector.X, vector.Y, 0));
            } while ((i++) % Preview.Length != (offset - 1 + Preview.Length) % Preview.Length);

            return result;
        }

        private IEnumerable<Vector2> BuildDirectLine (Vector2 start, Vector2 finish, float maxIntervall) {
            float distance = start.Distance(finish);
            int count = Mathi.Ceil(distance / maxIntervall);
            float intervall = distance / count;
            Vector2 normal = (start - finish) / distance;

            for (int i = 0; i < count; i++) {
                yield return start + normal * i * distance;
            }
        }

        public virtual void Prepare ( ) {
        }

        public void Cast (float gestureSuccess) {
            Mode = AbilityMode.Active;
            Stride = 1f;
            OnCast(gestureSuccess);
            UpdateRequired?.Invoke(this);
        }

        protected abstract void OnCast (float gestureSuccess);

        protected void EndCast ( ) {
            Mode = AbilityMode.Recharging;
            Stride = 0f;
            UpdateRequired?.Invoke(this);
        }

        public virtual void Update (DeltaTime dt) {
            if (Mode == AbilityMode.Recharging) {
                Stride = Mathf.Clamp01(Stride + dt.TotalMilliseconds / Cooldown);
                if (Stride == 1f) Mode = AbilityMode.Ready;
                UpdateRequired?.Invoke(this);
            } else if (Mode == AbilityMode.Casting) {
                Stride = Mathf.Clamp01(Stride - dt.TotalMilliseconds / LONG_PRESS_TIME);
                if (Stride == 0f) {
                    Mode = AbilityMode.Boosting;
                    GestureInputRequested?.Invoke(this, null);
                }
                UpdateRequired?.Invoke(this);
            } else if (Mode == AbilityMode.Boosting) {
                UpdateRequired?.Invoke(this);
            }
        }

        public void AbortCasting ( ) {
            Mode = AbilityMode.Active;
            Stride = 1f;
            UpdateRequired?.Invoke(this);
        }
    }
}
