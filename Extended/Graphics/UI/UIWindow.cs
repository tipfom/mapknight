using System;
using System.Collections.Generic;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Programs;
using mapKnight.Extended.Graphics.UI.Layout;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.UI {
    public class UIWindow : Screen {
        private Framebuffer uiBuffer;

        public UIWindow (Vector2 size) {
            uiBuffer = new Framebuffer(Window.Size.Width, Window.Size.Height, true);
            new UIWindowItem(this, size);
        }

        public void FillUIBuffer ( ) {
            Framebuffer cache = new Framebuffer(Window.Size.Width, Window.Size.Height, true);

            uiBuffer.Bind( );
            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            UIRenderer.Update(default(DeltaTime));
            UIRenderer.Draw( );

            GaussianBlurProgram.Program.Begin( );
            GaussianBlurProgram.Program.Draw(uiBuffer, cache, true);
            GaussianBlurProgram.Program.End( );
            uiBuffer.Unbind( );

            cache.Dispose( );
        }

        public override void Draw ( ) {
            FBOProgram.Program.Begin( );
            FBOProgram.Program.Draw(GaussianBlurProgram.INDEX_BUFFER, GaussianBlurProgram.VERTEX_BUFFER, GaussianBlurProgram.TEXTURE_BUFFER, uiBuffer.Texture, 6);
            FBOProgram.Program.End( );
            base.Draw( );
        }

        private class UIWindowItem : UIItem {
            private Vector2 screenSize;

            public UIWindowItem (Screen owner, Vector2 screenSize) : base(owner, new UILayout(new UIMargin(0f, 1f, 0f, 1f), UIMarginType.Relative), UIDepths.BACKGROUND, false) {
                this.screenSize = screenSize;
                IsDirty = true;
            }

            public override bool HandleTouch (UITouchAction action, UITouch touch) {
                if (action == UITouchAction.End) {
                    if (Math.Abs(touch.RelativePosition.X) <= screenSize.X / 2f && Math.Abs(touch.RelativePosition.Y) <= screenSize.Y / 2f && (touch.RelativePosition - screenSize / 2f).MagnitudeSqr( ) > (6f / 150f * Window.Ratio) * (6f / 150f * Window.Ratio)) {
                        global::Android.Widget.Toast.MakeText(Assets.Context, "inside", global::Android.Widget.ToastLength.Short).Show( );
                    } else {
                        global::Android.Widget.Toast.MakeText(Assets.Context, "outside", global::Android.Widget.ToastLength.Short).Show( );
                    }
                }
                return true;
            }

            public override IEnumerable<DepthVertexData> ConstructVertexData ( ) {
                float pixelWidth = 2f / 150f * Window.Ratio;
                float borderSize = 2 * pixelWidth;
                float cornerX = screenSize.X / 2f, cornerY = screenSize.Y / 2f;
                float edgeX = cornerX - borderSize, edgeY = cornerY - borderSize;
                float edgeWidth = edgeX * 2, edgeHeight = edgeY * 2;
                yield return new DepthVertexData(UIRectangle.GetVerticies(-edgeX, edgeY, edgeWidth, edgeHeight), "window_c", Depth);
                yield return new DepthVertexData(UIRectangle.GetVerticies(-edgeX, cornerY, edgeWidth, borderSize), "window_t", Depth);
                yield return new DepthVertexData(UIRectangle.GetVerticies(-edgeX, -cornerY + borderSize, edgeWidth, borderSize), "window_b", Depth);
                yield return new DepthVertexData(UIRectangle.GetVerticies(-cornerX, edgeY, borderSize, edgeHeight), "window_l", Depth);
                yield return new DepthVertexData(UIRectangle.GetVerticies(cornerX - borderSize, edgeY, borderSize, edgeHeight), "window_r", Depth);
                yield return new DepthVertexData(UIRectangle.GetVerticies(-cornerX, cornerY, borderSize, borderSize), "window_tl", Depth);
                yield return new DepthVertexData(UIRectangle.GetVerticies(-cornerX, -cornerY + borderSize, borderSize, borderSize), "window_bl", Depth);
                yield return new DepthVertexData(UIRectangle.GetVerticies(cornerX - 3f * pixelWidth, cornerY + borderSize, 5f * pixelWidth, 5f * pixelWidth), "window_tr", Depth);
                yield return new DepthVertexData(UIRectangle.GetVerticies(cornerX - borderSize, -cornerY + borderSize, borderSize, borderSize), "window_br", Depth);
            }
        }
    }
}
