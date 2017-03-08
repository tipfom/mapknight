using mapKnight.Extended.Components.Stats;
using System;
using System.Collections.Generic;
using mapKnight.Core;

namespace mapKnight.Extended.Graphics.UI.Layout {
    public class UIHealthBar : UIItem {
        const float HEIGHT_HALF = 0.025f;
        const float BAR_HEIGHT_HALF = HEIGHT_HALF * 1f / 2f;
        const float EDGE_WIDTH = 2f / 4f * 2f * HEIGHT_HALF;
        const float EDGE_OFFSET = 1f / 4f * 2f * HEIGHT_HALF;

        private HealthComponent healthComponent;
        private float[ ][ ] baseVerticies;
        private float[ ][] transformedVerticies;

        public UIHealthBar(Screen owner, HealthComponent healthComponent) : base(owner, new UIHorizontalCenterMargin(0), new UIVerticalCenterMargin(0), new AbsoluteSize(healthComponent.Initial * 0.05f, HEIGHT_HALF * 2), UIDepths.BACKGROUND, false) {
            this.healthComponent = healthComponent;
            healthComponent.Changed += UpdateIndicator;

            float halfWidth = Size.X / 2f;
            baseVerticies = new float[4][ ];
            baseVerticies[0] = new float[ ] {
                -halfWidth,               HEIGHT_HALF,
                -halfWidth,              -HEIGHT_HALF,
                -halfWidth + EDGE_WIDTH, -HEIGHT_HALF,
                -halfWidth + EDGE_WIDTH,  HEIGHT_HALF
            };
            baseVerticies[1] = new float[ ] {
                -halfWidth + EDGE_WIDTH,  HEIGHT_HALF,
                -halfWidth + EDGE_WIDTH, -HEIGHT_HALF,
                 halfWidth - EDGE_WIDTH, -HEIGHT_HALF,
                 halfWidth - EDGE_WIDTH,  HEIGHT_HALF
            };
            baseVerticies[2] = new float[ ] {
                 halfWidth - EDGE_WIDTH,  HEIGHT_HALF,
                 halfWidth - EDGE_WIDTH, -HEIGHT_HALF,
                 halfWidth,              -HEIGHT_HALF,
                 halfWidth,               HEIGHT_HALF
            };
            baseVerticies[3] = new float[8];
            UpdateIndicator( );

            transformedVerticies = new float[baseVerticies.Length][ ];
            for (int i = 0; i < transformedVerticies.Length; i++)
                transformedVerticies[i] = new float[8];
        }

        public override void Update(DeltaTime dt) {
            if (healthComponent.Owner.IsOnScreen)
                IsDirty = true;
            base.Update(dt);
        }

        private void UpdateIndicator( ) {
            float indicatorWidth = healthComponent.Current / healthComponent.Initial * (Size.X - 2 * EDGE_OFFSET);
            baseVerticies[3][0] = -Size.X / 2f + EDGE_OFFSET;
            baseVerticies[3][1] = BAR_HEIGHT_HALF;
            baseVerticies[3][2] = baseVerticies[3][0];
            baseVerticies[3][3] = -BAR_HEIGHT_HALF;
            baseVerticies[3][4] = baseVerticies[3][0] + indicatorWidth;
            baseVerticies[3][5] = -BAR_HEIGHT_HALF;
            baseVerticies[3][6] = baseVerticies[3][4];
            baseVerticies[3][7] = BAR_HEIGHT_HALF;
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData( ) {
            Mathf.TransformAtOrigin(baseVerticies[0], ref transformedVerticies[0], healthComponent.Owner.PositionOnScreen.X, healthComponent.Owner.PositionOnScreen.Y, 0f, false);
            yield return new DepthVertexData(transformedVerticies[0], "hbar_l", UIDepths.BACKGROUND);
            Mathf.TransformAtOrigin(baseVerticies[1], ref transformedVerticies[1], healthComponent.Owner.PositionOnScreen.X, healthComponent.Owner.PositionOnScreen.Y, 0f, false);
            yield return new DepthVertexData(transformedVerticies[1], "hbar_m", UIDepths.BACKGROUND);
            Mathf.TransformAtOrigin(baseVerticies[2], ref transformedVerticies[2], healthComponent.Owner.PositionOnScreen.X, healthComponent.Owner.PositionOnScreen.Y, 0f, false);
            yield return new DepthVertexData(transformedVerticies[2], "hbar_r", UIDepths.BACKGROUND);
            Mathf.TransformAtOrigin(baseVerticies[3], ref transformedVerticies[3], healthComponent.Owner.PositionOnScreen.X, healthComponent.Owner.PositionOnScreen.Y, 0f, false);
            yield return new DepthVertexData(transformedVerticies[3], "blank", UIDepths.BACKGROUND, Color.Red);
        }
    }
}
