namespace mapKnight.Core.World.Serialization {
    public enum DataType : byte {
        Default = 0,

        Boolean = 1,
        BooleanArray = 2,

        Byte = 3,
        ByteArray = 4,
        SByte = 5,
        SByteArray = 6,

        Short = 7,
        ShortArray = 8,
        UShort = 9,
        UShortArray = 10,

        Int = 11,
        IntArray = 12,
        UInt = 13,
        UIntArray = 14,

        Long = 15,
        LongArray = 16,
        ULong = 17,
        ULongArray = 18,

        Float = 19,
        FloatArray = 20,
        Double = 21,
        DoubleArray = 22,
        Decimal = 23,
        DecimalArray = 24,

        Vector2 = 25,
        Vector2Array = 26,

        String = 27,
        StringArray = 28,
    }
}
