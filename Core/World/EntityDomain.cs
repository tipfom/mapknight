namespace mapKnight.Core.World {
    // (╯°□°）╯︵ ┻━┻
    public enum EntityDomain {
        Enemy,          // everything that attacks the player and can be attack 
        NPC,            // everything to interact with 
        Obstacle,       // everything dealing damage to the player that cant be attacked directly 
        Platform,       // platforms
        Player,         // the player itself 
        Temporary = 0,  // default, will be assigned to everything not conceirning the player directly 
    }
}