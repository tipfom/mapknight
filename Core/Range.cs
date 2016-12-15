namespace mapKnight.Core {
    public struct Range<T> {
        public T Min;
        public T Max;

        public Range (T min, T max) {
            Min = min;
            Max = max;
        }
    }
}
