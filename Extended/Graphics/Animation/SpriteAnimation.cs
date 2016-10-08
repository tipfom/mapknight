using System;

namespace mapKnight.Core.Graphics {

    public class SpriteAnimation {
        public string Name;
        public bool CanRepeat;
        public bool IsRunning;

        private int currentSprite;
        private int nextSprite;
        private int nextSpriteTime;

        public SpriteAnimationFrame[ ] Frames;

        public SpriteAnimationFrame CurrentFrame { get { return Frames[currentSprite]; } }

        public void Reset ( ) {
            currentSprite = 0;
            nextSprite = Math.Min(Frames.Length - 1, 1);
            nextSpriteTime = Environment.TickCount + Frames[currentSprite].Time;
            IsRunning = true;
        }

        public void Update (float dt) {
            if (Environment.TickCount > nextSpriteTime) {
                currentSprite = nextSprite;
                nextSprite++;
                if (currentSprite == Frames.Length) {
                    IsRunning = false;
                    currentSprite = currentSprite - 1;
                    return;
                }
                nextSpriteTime = Environment.TickCount + Frames[currentSprite].Time;
            }
        }
    }
}