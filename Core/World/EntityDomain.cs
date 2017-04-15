namespace mapKnight.Core.World {
    // (╯°□°）╯︵ ┻━┻
    public enum EntityDomain {
        Temporary = 0,  // default, will be assigned to everything not conceirning the player directly 
                        // editorusage : about to be placed in the world
        Enemy,          // everything that attacks the player and can be attack 
                        // editorusage : placed in world
        NPC,            // everything to interact with 
                        // editorusage : about to be removed
        Obstacle,       // everything dealing damage to the player that cant be attacked directly 
                        // editorusage : selected
        Platform,       // platforms
        Player,         // the player itself 
    }
}