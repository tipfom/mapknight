using System;
using mapKnight.Core;
using Android.Gestures;
using System.Collections.Generic;

namespace mapKnight.Extended.Combat {
    public abstract class Ability {
        public readonly string Name;
        public readonly int Cooldown;
        public readonly string Texture;
        public readonly SecondaryWeapon Weapon;
        public readonly Vector2[ ] Preview;
        public readonly Gesture Gesture;

        public bool Available { get { return Availability == 1f; } }
        public float Availability = 1f;
        public event Action<Ability> AvailabilityChanged;

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

        public abstract void OnCast ( );

        public virtual void Update (DeltaTime dt) {
            if (Availability < 1f) {
                Availability = Mathf.Clamp01(Availability + dt.TotalMilliseconds / Cooldown);
                AvailabilityChanged?.Invoke(this);
            }
        }
    }
}
