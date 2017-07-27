namespace mapKnight.Extended.Combat {
    public enum AbilityMode {
        Ready, // Ability is ready to use
        Active, // Ability is currently running
        Recharging, // Ability is on Cooldown
        Casting, // Ability is currently being casted (Icon is being touched)
        Boosting, // Ability is accepting gesture input
    }
}
