namespace mapKnight.Core {
    public enum Interpolation {
        Linear,
        Cosine,
        Jump // returns 2nd value, when percent > 50%, else return value nr 1
    }
}
