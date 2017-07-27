using System;
using System.Collections.Generic;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Extended.Combat;
using mapKnight.Extended.Graphics.Buffer;
using OpenTK.Graphics.ES20;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Programs;

namespace mapKnight.Extended.Graphics.UI {
    public class UIAbilityPanel : UIItem {
        private const int MAX_ABILITY_COUNT = 3;
        private const int RENDER_COUNT = MAX_ABILITY_COUNT * 6;
        private const int BORDER_COUNT = 17;
        private const int BORDER_LOOP_INTERVAL = 600 / BORDER_COUNT;

        private static readonly float[ ] COLOR_ACTIVE = new Color("40b7ee").ToArray4( );
        private static readonly float[ ] COLOR_BOOSTING = new Color("de2853").ToArray4( );
        private static readonly float[ ] COLOR_DEFAULT = Color.White.ToArray4( );

        private int lastIndex = -1;
        private float totalIconHeight;
        private CachedGPUBuffer borderVertexBuffer;
        private CachedGPUBuffer borderTextureBuffer;
        private CachedGPUBuffer borderColorBuffer;
        private CachedGPUBuffer baseVertexBuffer;
        private CachedGPUBuffer baseTextureBuffer;
        private CachedGPUBuffer ampTextureBuffer;
        private IndexBuffer indexBuffer;
        private Texture2D ampTexture;
        private List<Ability> abilities = new List<Ability>( );
        private int currentBorderIndex = 0;
        private int nextBorderIndexIncrease = Environment.TickCount;

        public UIAbilityPanel (Screen owner, UILayout layout) : base(owner, layout, UIDepths.FOREGROUND, false) {
            ampTexture = Assets.GetTexture(InterpolationMode.Linear, "textures/interface_ability_amp.png");
            borderVertexBuffer = new CachedGPUBuffer(2, MAX_ABILITY_COUNT * 2, PrimitiveType.Quad, BufferUsage.StaticDraw);
            borderTextureBuffer = new CachedGPUBuffer(2, MAX_ABILITY_COUNT * 2, PrimitiveType.Quad, BufferUsage.DynamicDraw);
            borderColorBuffer = new CachedGPUBuffer(4, MAX_ABILITY_COUNT * 2, PrimitiveType.Quad, BufferUsage.DynamicDraw);
            baseVertexBuffer = new CachedGPUBuffer(2, MAX_ABILITY_COUNT, PrimitiveType.Quad, BufferUsage.StaticDraw);
            baseTextureBuffer = new CachedGPUBuffer(2, MAX_ABILITY_COUNT, PrimitiveType.Quad, BufferUsage.StaticDraw);
            ampTextureBuffer = new CachedGPUBuffer(2, MAX_ABILITY_COUNT, PrimitiveType.Quad, BufferUsage.DynamicDraw);
            indexBuffer = new IndexBuffer(MAX_ABILITY_COUNT * 2);

            ResetVertexBuffer( );
            layout.Changed += ( ) => ResetVertexBuffer( );
        }

        private void ResetVertexBuffer ( ) {
            totalIconHeight = 1.1f * Layout.Width;
            for (int i = 0; i < MAX_ABILITY_COUNT * 2; i++) {
                int bPosition = i * 8;
                float top = Layout.Y - (i % MAX_ABILITY_COUNT) * totalIconHeight;

                borderVertexBuffer[bPosition + 00] = Layout.X;
                borderVertexBuffer[bPosition + 01] = top;
                borderVertexBuffer[bPosition + 02] = Layout.X;
                borderVertexBuffer[bPosition + 03] = top - Layout.Width;
                borderVertexBuffer[bPosition + 04] = Layout.X + Layout.Width;
                borderVertexBuffer[bPosition + 05] = top - Layout.Width;
                borderVertexBuffer[bPosition + 06] = Layout.X + Layout.Width;
                borderVertexBuffer[bPosition + 07] = top;
            }
            for (int i = 0; i < MAX_ABILITY_COUNT; i++) {
                int bPosition = i * 8;
                float top = Layout.Y - i * totalIconHeight;
                float offset = Layout.Width / 16f;

                baseVertexBuffer[bPosition + 00] = Layout.X + offset;
                baseVertexBuffer[bPosition + 01] = top - offset;
                baseVertexBuffer[bPosition + 02] = Layout.X + offset;
                baseVertexBuffer[bPosition + 03] = top - Layout.Width + offset;
                baseVertexBuffer[bPosition + 04] = Layout.X + Layout.Width - offset;
                baseVertexBuffer[bPosition + 05] = top - Layout.Width + offset;
                baseVertexBuffer[bPosition + 06] = Layout.X + Layout.Width - offset;
                baseVertexBuffer[bPosition + 07] = top - offset;
            }
            borderVertexBuffer.Apply( );
            borderColorBuffer.Apply( );
            baseVertexBuffer.Apply( );
        }

        private void PrepareAbility (int index) {
            int bPosition = 8 * index;

            float[ ] textureData = UIRenderer.Texture[abilities[index].Texture];
            for (int i = 0; i < 8; i++)
                baseTextureBuffer[bPosition + i] = textureData[i];
            baseTextureBuffer.Apply( );

            float[ ] data = UIRenderer.Texture["bdloop"];
            for (int i = 0; i < 8; i++)
                borderTextureBuffer[bPosition + i] = data[i];
            borderTextureBuffer.Apply( );

            bPosition *= 2;
            for (int i = 0; i < 16; i++)
                borderColorBuffer[bPosition + i] = 1f;

            UpdateAbility(index);
        }

        private void UpdateAbility (int index) {
            Ability ability = abilities[index];

            float offset = 0f;
            float[ ] data;
            float[ ] color;
            switch (ability.Mode) {
                case AbilityMode.Active:
                    data = UIRenderer.Texture["bdloop_" + currentBorderIndex];
                    color = COLOR_ACTIVE;
                    break;
                case AbilityMode.Boosting:
                    data = UIRenderer.Texture["bdloop_" + currentBorderIndex];
                    color = COLOR_BOOSTING;
                    break;
                case AbilityMode.Ready:
                case AbilityMode.Recharging:
                case AbilityMode.Casting:
                default:
                    offset = .5f - .5f * abilities[index].Stride;
                    data = new float[ ] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
                    color = COLOR_DEFAULT;
                    break;
            }

            int bPosition = index * 8;
            ampTextureBuffer[bPosition] = 0f;
            ampTextureBuffer[bPosition + 1] = offset;
            ampTextureBuffer[bPosition + 2] = 0f;
            ampTextureBuffer[bPosition + 3] = .5f + offset;
            ampTextureBuffer[bPosition + 4] = 1f;
            ampTextureBuffer[bPosition + 5] = .5f + offset;
            ampTextureBuffer[bPosition + 6] = 1f;
            ampTextureBuffer[bPosition + 7] = offset;
            ampTextureBuffer.Apply( );

            bPosition += MAX_ABILITY_COUNT * 8;
            for (int i = 0; i < 8; i++)
                borderTextureBuffer[bPosition + i] = data[i];

            bPosition *= 2;
            for (int i = 0; i < color.Length; i++)
                borderColorBuffer[bPosition + i] = color[i];
            
            borderTextureBuffer.Apply( );
            borderColorBuffer.Apply( );
        }

        public bool Add (Ability ability) {
            if (abilities.Count < MAX_ABILITY_COUNT) {
                abilities.Add(ability);
                PrepareAbility(abilities.Count - 1);
                ability.UpdateRequired += Ability_UpdateRequired;
                return true;
            }
            return false;
        }

        private void Ability_UpdateRequired (Ability ability) {
            for (int i = 0; i < abilities.Count; i++) {
                if (abilities[i] == ability) {
                    UpdateAbility(i);
                    return;
                }
            }
        }

        public void Draw ( ) {
            UIAbilityIconProgram.Program.Begin( );
            UIAbilityIconProgram.Program.Draw(indexBuffer, baseVertexBuffer, baseTextureBuffer, ampTextureBuffer, UIRenderer.Texture, ampTexture, Matrix.Default, RENDER_COUNT, 0, true);
            UIAbilityIconProgram.Program.End( );
            ColorProgram.Program.Begin( );
            ColorProgram.Program.Draw(indexBuffer, borderVertexBuffer, borderTextureBuffer, borderColorBuffer, UIRenderer.Texture, Matrix.Default, indexBuffer.Length, 0, true);
            ColorProgram.Program.End( );
        }

        public override void Update (DeltaTime dt) {
            if (Environment.TickCount > nextBorderIndexIncrease) {
                currentBorderIndex++;
                currentBorderIndex %= BORDER_COUNT;
                nextBorderIndexIncrease = Environment.TickCount + BORDER_LOOP_INTERVAL;
            }
            base.Update(dt);
        }

        public override bool HandleTouch (UITouchAction action, UITouch touch) {
            int index = GetClickedAbilityIndex(touch);
            if (index < 0) return false;

            if (abilities[index].Mode != AbilityMode.Ready && abilities[index].Mode != AbilityMode.Casting) return false;

            switch (action) {
                case UITouchAction.Begin:
                    lastIndex = index;
                    abilities[index].Mode = AbilityMode.Casting;
                    return true;
                case UITouchAction.Move:
                    if (lastIndex != index && lastIndex > -1 && abilities[lastIndex].Mode == AbilityMode.Casting) {
                        abilities[lastIndex].AbortCasting( );
                        abilities[index].Mode = AbilityMode.Casting;
                        lastIndex = index;
                    }
                    return true;
                case UITouchAction.End:
                    if (lastIndex > -1 && abilities[lastIndex].Mode == AbilityMode.Casting) {
                        abilities[lastIndex].Cast(0f);
                    }
                    lastIndex = -1;
                    return true;
            }

            return false;
        }

        private int GetClickedAbilityIndex (UITouch touch) {
            float relativeY = 2 * (Layout.Y - touch.RelativePosition.Y) / Layout.Height;

            int index = (int)(relativeY / totalIconHeight);
            if (index < abilities.Count) {
                return index;
            }

            return -1;
        }

        public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
            yield break;
        }
    }
}
