using System;
using System.Collections.Generic;

namespace mapKnight.Extended.Graphics {

    public class SpriteAnimation {
        public int Delay;
        public string[ ] Sprites;

        private int currentSprite;
        private int lastSwap = Environment.TickCount;
        public string Current { get { return Sprites[currentSprite]; } }

        public void Update (float dt) {
            if (lastSwap + Delay < Environment.TickCount) {
                lastSwap = Environment.TickCount;
                currentSprite++;
                if (currentSprite == Sprites.Length)
                    currentSprite = 0;
            }
        }
    }
}