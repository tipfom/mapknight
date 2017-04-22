using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using mapKnight.Core;
using mapKnight.ToolKit.Data;
using Newtonsoft.Json;
using System.Globalization;

namespace mapKnight.ToolKit.Serializer {
    public static class AnimationSerizalizer {
        private static JsonSerializerSettings settings = new JsonSerializerSettings( ) { NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.None, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate };

        public static void Compile (VertexAnimation[ ] animations, Stream stream, List<int> indices) {
            Array.Sort(animations, Comparer<VertexAnimation>.Create((a, b) => b.IsDefault.CompareTo(a.IsDefault)));
            using (StreamWriter writer = new StreamWriter(stream)) {
                writer.WriteLine("Scales = new float[ ] {");
                foreach (VertexBone bone in SelectBones(animations[0].Frames[0], indices).Reverse( )) {
                    writer.WriteLine("\t" + bone.Scale.ToString(CultureInfo.InvariantCulture) + "f,");
                }
                writer.WriteLine("},");

                writer.WriteLine("Animations = new VertexAnimation[ ] {");

                foreach (VertexAnimation animation in animations) {
                    writer.WriteLine("\tnew VertexAnimation( ) {");

                    writer.WriteLine("\t\tName = \"" + animation.Name + "\",");
                    if (animation.CanRepeat)
                        writer.WriteLine("\t\tCanRepeat = true,");

                    writer.WriteLine("\t\tFrames = new VertexAnimationFrame[ ] {");
                    foreach (VertexAnimationFrame frame in animation.Frames) {
                        writer.WriteLine("\t\t\tnew VertexAnimationFrame( ) {");
                        writer.WriteLine("\t\t\t\tTime = " + frame.Time + ",");

                        writer.WriteLine("\t\t\t\tState = new VertexBone[ ] {");
                        foreach (VertexBone bone in SelectBones(frame, indices).Reverse( )) {
                            writer.WriteLine("\t\t\t\t\tnew VertexBone( ) {");
                            if (bone.Mirrored)
                                writer.WriteLine("\t\t\t\t\t\tMirrored = true,");
                            if (bone.Rotation != 0f)
                                writer.WriteLine("\t\t\t\t\t\tRotation = " + (-bone.Rotation).ToString(CultureInfo.InvariantCulture) + "f,");
                            if (bone.Position != default(Vector2))
                                writer.WriteLine("\t\t\t\t\t\tPosition = new Vector2(" + bone.Position.X.ToString(CultureInfo.InvariantCulture) + "f, " + bone.Position.Y.ToString(CultureInfo.InvariantCulture) + "f),");
                            writer.WriteLine("\t\t\t\t\t\tTexture = \"" + bone.Image + "\"");
                            writer.WriteLine("\t\t\t\t\t},");
                        }
                        writer.WriteLine("\t\t\t\t}");

                        writer.WriteLine("\t\t\t},");
                    }
                    writer.WriteLine("\t\t},");

                    writer.WriteLine("\t},");
                }

                writer.WriteLine("}");
            }
        }

        private static IEnumerable<VertexBone> SelectBones (VertexAnimationFrame frame, List<int> indices) {
            for (int i = 0; i < frame.Bones.Count; i++) {
                if (indices.Contains(i)) {
                    yield return frame.Bones[i];
                }
            }
        }
    }
}