using System;
using System.Collections.Generic;
using System.Drawing;

namespace mapKnight.ToolKit
{
    class SlotSpriteData
    {
        public readonly Size SpriteSize;
        public readonly Dictionary<Slot, Rectangle> SlotPositions;

        public SlotSpriteData(Size spriteSize, Dictionary<Slot,Rectangle> slotPositions)
        {
            SpriteSize = spriteSize;
            SlotPositions = slotPositions;
        }
    }
}
