using System;
using System.Collections.Generic;

namespace mapKnight.Graphics {
    public class SpriteAnimation {
        public int Delay;
        public List<string> Sprites;

        private int currentSprite;
        public string Current { get { return Sprites[currentSprite]; } }
        private int lastSwap = Environment.TickCount;

        public void Update (float dt) {
            if (lastSwap + Delay < Environment.TickCount) {
                lastSwap = Environment.TickCount;
                currentSprite++;
                if (currentSprite == Sprites.Count)
                    currentSprite = 0;
            }
        }
    }
}