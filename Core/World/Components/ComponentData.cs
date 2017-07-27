namespace mapKnight.Core.World.Components {
    public enum ComponentData : int{
        None = -1,

        // player
        InputInclude = 0,
        InputExclude,
        InputGesture,

        // graphics
        Verticies,
        Texture,
        Color,
        VertexAnimation,
        SpriteAnimation,

        // stats
        Damage,
        Heal,

        // physics
        Velocity,
        Acceleration,
        SlowDown,
        ScaleX
    }
}
