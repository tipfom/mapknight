using System;
using System.Collections.Generic;
using mapKnight.Android.CGL.Buffer;
using mapKnight.Basic;

namespace mapKnight.Android.CGL.Entity {
    public class CGLEntity : Physics.Entity {
        const int MAX_QUADS = 50;

        private static List<CGLEntity> activeEntitys = new List<CGLEntity> ( );

        protected List<CGLBoundedPoint> boundedPoints;

        protected CGLSet set;
        protected List<CGLAnimation> animations;
        private int currentAnimation;

        public CGLEntity (int health, fPoint position, string name, int weight, fSize bounds, List<CGLBoundedPoint> boundedpoints, List<CGLAnimation> animations, CGLSet set)
            : base (weight, bounds, health, position, name) {
            this.set = set;
            this.boundedPoints = boundedpoints;
            this.animations = animations;
            this.currentAnimation = animations.FindIndex (((CGLAnimation obj) => obj.Action == "default"));
            this.animations[currentAnimation].Start ( );

            activeEntitys.Add (this); // register entity
        }

        protected virtual fVector2D GetCentreOnScreen (CGLCamera camera) {
            return this.Position - camera.ScreenCentre;
        }

        public List<CGLVertexData> GetVertexData (CGLCamera camera) {
            List<CGLVertexData> vertexData = new List<CGLVertexData> ( );
            fVector2D entityOffset = GetCentreOnScreen (camera);
            // begin translating the current animationdata
            for (int i = 0; i < boundedPoints.Count; i++) {
                float[ ] data = animations[currentAnimation].Current[boundedPoints[i].Name];
                vertexData.Add (new CGLVertexData (MathHelper.TranslateRotateMirror (MathHelper.GetVerticies (boundedPoints[i].Size), 0, 0, data[0] + entityOffset.X, data[1] + entityOffset.Y, data[2], (data[3] == 1f) ? true : false),
                    this.Name + "_" + boundedPoints[i].Name, Color.White));
            }

            return vertexData;
        }

        public bool Animate (string animation) {
            if (animations[currentAnimation].Abortable == true || animations[currentAnimation].Finished == true) {
                currentAnimation = animations.IndexOf (animations.Find (((CGLAnimation obj) => obj.Action == animation)));
                animations[currentAnimation].Start ( );
                return true;
            } else
                return false;
        }

        [Obsolete ("this is only for debug puroses! remove it in the final builds.")]
        public void AddAnimation (CGLAnimation animation) {
            animations.RemoveAll ((CGLAnimation obj) => obj.Action == animation.Action);

            animations.Add (animation);
            Animate (animation.Action);
        }

        private void UpdateEntity (float deltatime) {
            if (animations[currentAnimation].Finished) {
                if (animations[currentAnimation].Loopable)
                    animations[currentAnimation].Start ( );
            } else {
                animations[currentAnimation].Step (deltatime);
            }
        }

        public void Die () {
            // destruktor
            // wenns hier fehler, auf erik zukommen (keine destruktor)
            activeEntitys.Remove (this); // unregister entity
        }

        #region static drawing and updating

        private static CGLColorBufferBatch buffer = new CGLColorBufferBatch (MAX_QUADS * 4);
        private static CGLSprite2D entitySprite;

        public static void Draw (CGLCamera camera, CGLMap map) {
            Array.Clear (buffer.VertexData, 0, buffer.VertexData.Length);
            // updating buffer
            int currentIndex = 0;
            foreach (CGLEntity entityToDraw in activeEntitys.FindAll ((CGLEntity entity) => (entity.Position - camera.ScreenCentre).Abs ( ) < (fVector2D)map.DrawSize / 2)) {
                // all entitys, that can be seen on the map
                foreach (CGLVertexData vertexData in entityToDraw.GetVertexData (camera)) {
                    Array.Copy (vertexData.Verticies, 0, buffer.VertexData, currentIndex * 8, 8);
                    Array.Copy (entitySprite.Get (vertexData.Texture), 0, buffer.TextureData, currentIndex * 8, 8);
                    Array.Copy (vertexData.Color.ToOpenGL ( ), 0, buffer.ColorData, currentIndex * 16, 16);
                    currentIndex++;
                }
            }
            buffer.UpdateBuffer ( );

            Content.ColorProgram.Begin ( );
            Content.ColorProgram.Draw (buffer.VertexBuffer, buffer.TextureBuffer, buffer.ColorBuffer, buffer.IndexBuffer, entitySprite.Texture, Screen.DefaultMatrix.MVP, true);
            Content.ColorProgram.End ( );
        }

        public static void Update (float dt) {
            foreach (CGLEntity entity in activeEntitys) {
                entity.UpdateEntity (dt);
            }
        }

        #endregion
    }
}