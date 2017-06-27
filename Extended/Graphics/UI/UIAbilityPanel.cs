using System;
using System.Collections.Generic;
using System.Timers;
using mapKnight.Core.Graphics;
using mapKnight.Extended.Graphics.UI.Layout;
using mapKnight.Extended.Combat;
using mapKnight.Extended.Graphics.Buffer;
using OpenTK.Graphics.ES20;
using static mapKnight.Extended.Graphics.Programs.UIAbilityIconProgram;

namespace mapKnight.Extended.Graphics.UI {
    public class UIAbilityPanel : UIItem {
        private const int MAX_ABILITY_COUNT = 3;
        private const double LONG_PRESS_TIME = 200d;

        public event Action<Ability> OnLongAbilityPress;

        private int lastIndex = -1;
        private float totalIconHeight;
        private CachedGPUBuffer vertexBuffer;
        private CachedGPUBuffer baseTextureBuffer;
        private CachedGPUBuffer ampTextureBuffer;
        private IndexBuffer indexBuffer;
        private Texture2D ampTexture;
        private List<Ability> abilities = new List<Ability>( );
        private Timer longPressTimer = new Timer(LONG_PRESS_TIME) { AutoReset = false };

        public UIAbilityPanel (Screen owner, UILayout layout) : base(owner, layout, UIDepths.FOREGROUND, false) {
            ampTexture = Assets.Load<Texture2D>("interface_ability_amp");
            vertexBuffer = new CachedGPUBuffer(2, MAX_ABILITY_COUNT, PrimitiveType.Quad, BufferUsage.StaticDraw);
            baseTextureBuffer = new CachedGPUBuffer(2, MAX_ABILITY_COUNT, PrimitiveType.Quad, BufferUsage.StaticDraw);
            ampTextureBuffer = new CachedGPUBuffer(2, MAX_ABILITY_COUNT, PrimitiveType.Quad, BufferUsage.StaticDraw);
            indexBuffer = new IndexBuffer(MAX_ABILITY_COUNT);

            ResetVertexBuffer( );
            layout.Changed += ( ) => ResetVertexBuffer( );

            longPressTimer.Elapsed += LongPressTimer_Elapsed;
        }

        private void LongPressTimer_Elapsed (object sender, ElapsedEventArgs e) {
            if (lastIndex < 0) return;

            OnLongAbilityPress?.Invoke(abilities[lastIndex]);
        }

        private void ResetVertexBuffer ( ) {
            totalIconHeight = 1.1f * Layout.Width;
            for (int i = 0; i < MAX_ABILITY_COUNT; i++) {
                int bPosition = i * 8;
                float top = Layout.Y - i * totalIconHeight;

                vertexBuffer[bPosition + 00] = Layout.X;
                vertexBuffer[bPosition + 01] = top;
                vertexBuffer[bPosition + 02] = Layout.X;
                vertexBuffer[bPosition + 03] = top - Layout.Width;
                vertexBuffer[bPosition + 04] = Layout.X + Layout.Width;
                vertexBuffer[bPosition + 05] = top - Layout.Width;
                vertexBuffer[bPosition + 06] = Layout.X + Layout.Width;
                vertexBuffer[bPosition + 07] = top;
            }
            vertexBuffer.Apply( );
        }

        private void PrepareAbility (int index) {
            int bPosition = 8 * index;

            float[ ] textureData = UIRenderer.Texture[abilities[index].Texture];
            baseTextureBuffer[bPosition + 0] = textureData[0];
            baseTextureBuffer[bPosition + 1] = textureData[1];
            baseTextureBuffer[bPosition + 2] = textureData[2];
            baseTextureBuffer[bPosition + 3] = textureData[3];
            baseTextureBuffer[bPosition + 4] = textureData[4];
            baseTextureBuffer[bPosition + 5] = textureData[5];
            baseTextureBuffer[bPosition + 6] = textureData[6];
            baseTextureBuffer[bPosition + 7] = textureData[7];
            baseTextureBuffer.Apply( );

            UpdateAbility(index);
        }

        private void UpdateAbility (int index) {
            int bPosition = index * 8;
            float offset = .5f - .5f * abilities[index].Availability;

            ampTextureBuffer[bPosition + 0] = 0f;
            ampTextureBuffer[bPosition + 1] = offset;
            ampTextureBuffer[bPosition + 2] = 0f;
            ampTextureBuffer[bPosition + 3] = .5f + offset;
            ampTextureBuffer[bPosition + 4] = 1f;
            ampTextureBuffer[bPosition + 5] = .5f + offset;
            ampTextureBuffer[bPosition + 6] = 1f;
            ampTextureBuffer[bPosition + 7] = offset;
            ampTextureBuffer.Apply( );
        }

        public bool Add (Ability ability) {
            if (abilities.Count < MAX_ABILITY_COUNT) {
                abilities.Add(ability);
                PrepareAbility(abilities.Count - 1);
                ability.AvailabilityChanged += Ability_CooldownChanged;
                return true;
            }
            return false;
        }

        private void Ability_CooldownChanged (Ability ability) {
            for (int i = 0; i < abilities.Count; i++) {
                if (abilities[i] == ability) {
                    UpdateAbility(i);
                    return;
                }
            }
        }

        public void Draw ( ) {
            Program.Begin( );
            Program.Draw(indexBuffer, vertexBuffer, baseTextureBuffer, ampTextureBuffer, UIRenderer.Texture, ampTexture, Matrix.Default, indexBuffer.Length, 0, true);
            Program.End( );
        }

        public override bool HandleTouch (UITouchAction action, UITouch touch) {
            int index = GetClickedAbilityIndex(touch);
            if (index < 0) return false;

            switch (action) {
                case UITouchAction.Begin:
                    lastIndex = index;
                    longPressTimer.Start( );
                    return true;
                case UITouchAction.Move:
                    if (lastIndex != index && lastIndex > -1 && longPressTimer.Enabled) {
                        longPressTimer.Stop( );
                        longPressTimer.Start( );
                    }
                    return true;
                case UITouchAction.End:
                case UITouchAction.Leave:
                    if (longPressTimer.Enabled) {
                        longPressTimer.Stop( );
                        if (abilities[index].Available)
                            abilities[index].OnCast( );
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
