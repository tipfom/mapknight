using System;

namespace mapKnight.Core {
    public struct DeltaTime {
        public static DeltaTime operator * (DeltaTime dt, float scale) {
            return new DeltaTime(dt.TotalMilliseconds * scale);
        }

        public float TotalSeconds;
        public float TotalMilliseconds;

        public DeltaTime (float seconds, float milliseconds) {
            TotalSeconds = seconds + milliseconds / 1000f;
            TotalMilliseconds = seconds * 1000f + milliseconds;
        }

        public DeltaTime (float milliseconds) {
            TotalMilliseconds = milliseconds;
            TotalSeconds = milliseconds / 1000f;
        }

        public DeltaTime (TimeSpan span) {
            TotalMilliseconds = (float)span.TotalMilliseconds;
            TotalSeconds = (float)span.TotalSeconds;
        }
    }
}
