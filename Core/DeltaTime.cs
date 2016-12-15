using System;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Core {
    public struct DeltaTime {
        public float TotalSeconds;
        public float TotalMilliseconds;

        public float Seconds;
        public float Milliseconds;

        public DeltaTime (float seconds, float milliseconds) {
            Seconds = seconds;
            Milliseconds = milliseconds;
            TotalSeconds = seconds + milliseconds / 1000f;
            TotalMilliseconds = seconds * 1000f + milliseconds;
        }

        public DeltaTime (float milliseconds) {
            Seconds = 0;
            Milliseconds = milliseconds;
            TotalMilliseconds = milliseconds;
            TotalSeconds = milliseconds / 1000f;
        }

        public DeltaTime (TimeSpan span) {
            Seconds = (float)Mathi.Floor((float)span.TotalSeconds);
            Milliseconds = (float)span.Milliseconds;
            TotalMilliseconds = (float)span.TotalMilliseconds;
            TotalSeconds = (float)span.TotalSeconds;
        }
    }
}
