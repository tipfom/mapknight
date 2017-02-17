using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.UI.Layout;

namespace mapKnight.Extended.Graphics.UI {
    public class UIBar : UIItem {
        const float BORDER_BOUNDS_RATIO = 10f / 10f; // width / height of image

        private Color foregroundColor;
        private Color backgroundColor;
        private IValueBinder valueBinder;
        private float currentPercent;

        public UIBar (Screen owner, Color foregroundColor, Color backgroundColor, IValueBinder valueBinder, UIMargin hmargin, UIMargin vmargin, Vector2 size, int depth) : base(owner, hmargin, vmargin, size, depth, false) {
            this.foregroundColor = foregroundColor;
            this.backgroundColor = backgroundColor;
            this.valueBinder = valueBinder;
            this.currentPercent = Mathf.Clamp01(this.valueBinder.Value / this.valueBinder.Maximum);

            this.valueBinder.ValueChanged += ValueBinder_ValueChanged;
            IsDirty = true;
        }

        private void ValueBinder_ValueChanged (float value) {
            currentPercent = Mathf.Clamp01(value / valueBinder.Maximum);
            IsDirty = true;
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            float barwidth = Size.X * currentPercent;
            yield return new DepthVertexData(Bounds.Verticies, "blank", Depth, backgroundColor);
            yield return new DepthVertexData(UIRectangle.GetVerticies(Position.X, Position.Y, barwidth, Size.Y), "blank", Depth, foregroundColor);
        }

        public interface IValueBinder {
            event Action<float> ValueChanged;
            float Value { get; }
            float Maximum { get; }
        }
    }
}