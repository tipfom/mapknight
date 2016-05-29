using System;

namespace mapKnight.Core.Exceptions {
    public class ChangedHashException : Exception {
        public ChangedHashException (Exception innerException) : base ("while loading data, an invalid hash value was detected", innerException) {

        }
    }
}
