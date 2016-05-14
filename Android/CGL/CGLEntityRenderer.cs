using Java.Nio;
using mapKnight.Android.ECS;
using System;
using System.Collections.Generic;

namespace mapKnight.Android.CGL {
    public class CGLEntityRenderer : IEntityRenderer {
        const int TEXTURE_UNIT_COUNT = 12;
        const int MAX_QUAD_COUNT = 300;

        public float VertexSize { get; set; }
        private FloatBuffer vertexBuffer;
        private float[] vertexBufferData;
        private FloatBuffer textureBuffer;
        private float[] textureBufferData;
        private ShortBuffer indexBuffer;


        private Queue<EntityVertexData> frameVertexData = new Queue<EntityVertexData> ();
        private List<Tuple<int, int>> frameData = new List<Tuple<int, int>> ();
        private Dictionary<int, CGLSprite2D> entityTextures = new Dictionary<int, CGLSprite2D> ();

        public CGLEntityRenderer () {
            // create buffer
            vertexBufferData = new float[MAX_QUAD_COUNT * 8];
            vertexBuffer = CGLTools.CreateBuffer<FloatBuffer> (MAX_QUAD_COUNT * 8);
            textureBufferData = new float[MAX_QUAD_COUNT * 8];
            textureBuffer = CGLTools.CreateBuffer<FloatBuffer> (MAX_QUAD_COUNT * 8);
            indexBuffer = CGLTools.CreateIndexBuffer (MAX_QUAD_COUNT);
        }

        public void QueueVertexData (EntityVertexData vertexData) {
            frameVertexData.Enqueue (vertexData);
        }

        public void AddTexture (int entityID, CGLSprite2D entityTexture) {
            if (!entityTextures.ContainsKey (entityID)) {
                entityTextures.Add (entityID, entityTexture);
            }
        }

        public void Update () {
            Dictionary<int, Tuple<List<float>, List<float>>> currentFrameData = new Dictionary<int, Tuple<List<float>, List<float>>> ();
            frameData.Clear ();
            while (frameVertexData.Count > 0) {
                EntityVertexData vertexData = frameVertexData.Dequeue ();
                if (!currentFrameData.ContainsKey (vertexData.Entity))
                    currentFrameData.Add (vertexData.Entity, new Tuple<List<float>, List<float>> (new List<float> (), new List<float> ()));
                currentFrameData[vertexData.Entity].Item1.AddRange (vertexData.VertexCoords);
                vertexData.SpriteNames.ForEach (delegate (string entry) { currentFrameData[vertexData.Entity].Item2.AddRange (entityTextures[vertexData.Entity].Get (entry)); });
            }

            // parse
            int currentIndex = 0;
            foreach (int entity in currentFrameData.Keys) {
                frameData.Add (new Tuple<int, int> (entityTextures[entity].Texture, currentFrameData[entity].Item2.Count / 8));
                Array.Copy (currentFrameData[entity].Item1.ToArray (), 0, vertexBufferData, currentIndex, currentFrameData[entity].Item1.Count);
                Array.Copy (currentFrameData[entity].Item2.ToArray (), 0, textureBufferData, currentIndex, currentFrameData[entity].Item2.Count);
                currentIndex += currentFrameData[entity].Item1.Count;
            }

            textureBuffer.Put (textureBufferData);
            vertexBuffer.Put (vertexBufferData);
            textureBuffer.Position (0);
            vertexBuffer.Position (0);
        }

        public void Draw () {
            Content.ProgramCollection.Entity.Begin ();
            int overallOffset = 0;
            for (int i = 0; i < frameData.Count; i++) {
                Content.ProgramCollection.Entity.Draw (vertexBuffer, textureBuffer, indexBuffer, frameData[i].Item1, overallOffset, frameData[i].Item2, true);
                overallOffset += frameData[i].Item2;
            }
            Content.ProgramCollection.Entity.End ();
            textureBuffer.Position (0);
            vertexBuffer.Position (0);
        }
    }
}