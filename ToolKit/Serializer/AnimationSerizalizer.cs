using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using mapKnight.Core;
using mapKnight.ToolKit.Data;
using Newtonsoft.Json;

namespace mapKnight.ToolKit.Serializer {
    public static class AnimationSerizalizer {
        private static JsonSerializerSettings settings = new JsonSerializerSettings( ) { NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.None, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate };

        public static void Serialize (ObservableCollection<VertexAnimation> animations, Stream stream) {
            List<SerializedAnimation> serializedanimations = new List<SerializedAnimation>( );
            float[ ] scales = animations[0].Frames[0].Bones.Select(bone => bone.Scale).ToArray( );
            for (int i = 0; i < animations.Count; i++) {
                serializedanimations.Add(new SerializedAnimation( ) {
                    Name = animations[i].Name,
                    CanRepeat = animations[i].CanRepeat,
                    Frames = animations[i].Frames.Select(fame => new SerializedFrame( ) {
                        Time = fame.Time,
                        State = fame.Bones.Select(bone => new SerializedBone( ) {
                            Mirrored = bone.Mirrored,
                            Rotation = -bone.Rotation,
                            Position = bone.Position,
                            Texture = bone.Image
                        }).ToArray()
                    }).ToArray()
                });
            }

            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(new SerializedContainer( ) { Scales = scales, Animations = serializedanimations.ToArray( ) }, settings));
        }

        private struct SerializedContainer {
            public float[ ] Scales;
            public SerializedAnimation[ ] Animations;
        }

        private struct SerializedAnimation {
            public string Name;
            public bool CanRepeat;
            public SerializedFrame[ ] Frames;
        }

        private struct SerializedFrame {
            public SerializedBone[ ] State;
            public int Time;
        }

        private struct SerializedBone {
            public bool Mirrored;
            public float Rotation;
            public Vector2 Position;
            public string Texture;
        }
    }
}