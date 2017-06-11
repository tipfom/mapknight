using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System;
using mapKnight.ToolKit.Serializer;
using mapKnight.ToolKit.Windows.Dialogs;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework;
using Point = System.Windows.Point;

namespace mapKnight.ToolKit.Data {
    public class VertexAnimationData {
        public string Description { get { return Meta.Entity; } }

        public ObservableCollection<VertexAnimation> Animations = new ObservableCollection<VertexAnimation>( );
        public ObservableCollection<VertexBone> Bones = new ObservableCollection<VertexBone>( );
        public AnimationMetaData Meta;
        public Dictionary<string, ImageData> Images = new Dictionary<string, ImageData>( );

        public SelectBonesDialog SelectBonesDialog;
        public EditBonesDialog EditBonesDialog;

        public event Action BoneCountChanged;
        public event Action<int, int> BoneZIndexChanged;
        public event Action<int> BoneScaleChanged;
        public event Action BoneImageChanged;

        private Dictionary<VertexAnimation, Dictionary<VertexAnimationFrame, Stack<ObservableCollection<VertexBone>>>> undoStack = new Dictionary<VertexAnimation, Dictionary<VertexAnimationFrame, Stack<ObservableCollection<VertexBone>>>>( );

        public VertexAnimationData ( ) {
            SelectBonesDialog = new SelectBonesDialog(Bones);

            EditBonesDialog = new EditBonesDialog(Bones);
            EditBonesDialog.BoneAdded += EditBonesDialog_BoneAdded;
            EditBonesDialog.BoneDeleted += EditBonesDialog_BoneDeleted; ;
            EditBonesDialog.BonePositionChanged += EditBonesDialog_BonePositionChanged; ;
            EditBonesDialog.ScaleChanged += EditBonesDialog_ScaleChanged; ;
            EditBonesDialog.BoneTextureChanged += EditBonesDialog_BoneTextureChanged; ;
        }

        private void EditBonesDialog_BoneAdded (VertexBone addedBone) {
            LoadImage(addedBone.Image);
            Bones.Add(addedBone);
            addedBone.SetBitmapImage(this);

            string name = Path.GetFileNameWithoutExtension(addedBone.Image);
            for (int i = 0; i < Animations.Count; i++) {
                for (int j = 0; j < Animations[i].Frames.Count; j++) {
                    VertexBone bone = addedBone.Clone( );
                    bone.Image = name;
                    Animations[i].Frames[j].Bones.Add(bone);
                    if (undoStack.ContainsKey(Animations[i])) {
                        if (undoStack[Animations[i]].ContainsKey(Animations[i].Frames[j])) {
                            foreach (ObservableCollection<VertexBone> collection in undoStack[Animations[i]][Animations[i].Frames[j]]) {
                                collection.Add(bone);
                            }
                        }
                    }
                }
            }

            BoneCountChanged?.Invoke( );
        }

        private void EditBonesDialog_BoneDeleted (int deletedBoneIndex) {
            Bones.RemoveAt(deletedBoneIndex);
            foreach (VertexAnimation animation in Animations) {
                foreach (VertexAnimationFrame frame in animation.Frames) {
                    frame.Bones.RemoveAt(deletedBoneIndex);
                }
            }

            BoneCountChanged?.Invoke( );
        }

        private void EditBonesDialog_BonePositionChanged (int newz, int oldz) {
            BoneZIndexChanged?.Invoke(newz, oldz);

            Bones.Move(oldz, newz);
            foreach (VertexAnimation animation in Animations) {
                foreach (VertexAnimationFrame frame in animation.Frames) {
                    frame.Bones.Move(oldz, newz);
                    if (undoStack.ContainsKey(animation) && undoStack[animation].ContainsKey(frame)) {
                        foreach (ObservableCollection<VertexBone> collection in undoStack[animation][frame]) {
                            collection.Move(oldz, newz);
                        }
                    }
                }
            }
        }

        private void EditBonesDialog_ScaleChanged (VertexBone bone, double scale) {
            int index = Bones.IndexOf(bone);
            foreach (VertexAnimation animation in Animations) {
                foreach (VertexAnimationFrame frame in animation.Frames) {
                    frame.Bones[index].Scale = (float)scale;
                }
            }

            BoneScaleChanged?.Invoke(index);
        }

        private void EditBonesDialog_BoneTextureChanged (VertexBone bone, string newPath) {
            ImageData initial = Images[Path.GetFileNameWithoutExtension(bone.Image)];
            BitmapImage image = new BitmapImage(new Uri(newPath));
            Images[Path.GetFileNameWithoutExtension(bone.Image)] = new ImageData( ) { Image = image, TransformOrigin = new Point(Math.Min(initial.TransformOrigin.X, image.PixelWidth), Math.Min(initial.TransformOrigin.Y, image.PixelHeight)) };
            bone.SetBitmapImage(this);
            bone.OnPropertyChanged("BitmapImage");
            BoneImageChanged?.Invoke( );
        }

        public void ShowEditBonesDialog ( ) {
            foreach (VertexBone bone in Bones) {
                bone.SetBitmapImage(this);
            }
            EditBonesDialog.Show( );
        }

        public void ShowSelectBonesDialog ( ) {
            SelectBonesDialog.Show( );
        }

        public bool ShowEntityResizeDialog (VertexAnimationFrame targetFrame) {
            ResizeEntityDialog dialog = new ResizeEntityDialog(Meta.Ratio, targetFrame?.Bones ?? Bones, this);
            if (dialog.ShowDialog( ) ?? false) {
                double centerShiftXReal = 0.5d + dialog.TrimRight - (1 + dialog.TrimLeft + dialog.TrimRight) / 2d;
                double centerShiftYReal = 0.5d + dialog.TrimTop - (1 + dialog.TrimTop + dialog.TrimBottom) / 2d;
                double scaleX = (1 + dialog.TrimLeft + dialog.TrimRight);
                double scaleY = (1 + dialog.TrimTop + dialog.TrimBottom);

                foreach (VertexBone bone in Bones) {
                    bone.Position = new Core.Vector2(
                        (float)((bone.Position.X - centerShiftXReal) / scaleX),
                        (float)((bone.Position.Y - centerShiftYReal) / scaleY));
                    bone.Scale /= (float)(scaleX);
                }
                foreach (VertexAnimation animation in Animations) {
                    foreach (VertexAnimationFrame frame in animation.Frames) {
                        foreach (VertexBone bone in frame.Bones) {
                            bone.Position = new Core.Vector2(
                                (float)((bone.Position.X - centerShiftXReal) / scaleX),
                                (float)((bone.Position.Y - centerShiftYReal) / scaleY));
                            bone.Scale /= (float)(scaleX);
                        }
                    }
                }
                Meta.Ratio *= scaleX / scaleY;
                return true;
            }
            return false;
        }

        public void DumpChanges (VertexAnimation animation, VertexAnimationFrame frame) {
            if (undoStack.ContainsKey(animation) && undoStack[animation].ContainsKey(frame)) {
                undoStack[animation][frame].Pop( );
            }
        }

        public void BackupChanges (VertexAnimation animation, VertexAnimationFrame frame) {
            if (!undoStack.ContainsKey(animation)) {
                undoStack.Add(animation, new Dictionary<VertexAnimationFrame, Stack<ObservableCollection<VertexBone>>>( ));
            }
            if (!undoStack[animation].ContainsKey(frame)) {
                undoStack[animation].Add(frame, new Stack<ObservableCollection<VertexBone>>( ));
            }
            undoStack[animation][frame].Push(new ObservableCollection<VertexBone>(frame.Bones.Select(bone => bone.Clone( ))));
        }

        public bool CanUndo (VertexAnimation animation, VertexAnimationFrame frame) {
            return undoStack.ContainsKey(animation) && undoStack[animation].ContainsKey(frame) && undoStack[animation][frame].Count > 0;
        }

        public ObservableCollection<VertexBone> Undo (VertexAnimation animation, VertexAnimationFrame frame) {
            return undoStack[animation][frame].Pop( );
        }

        public void LoadImage (string path) {
            string name = Path.GetFileNameWithoutExtension(path);
            if (!Images.ContainsKey(name)) {
                BitmapImage loadedImage = new BitmapImage( );
                loadedImage.BeginInit( );
                loadedImage.UriSource = new Uri(path);
                loadedImage.CacheOption = BitmapCacheOption.OnLoad;
                loadedImage.EndInit( );

                Images.Add(name, new ImageData( ) { TransformOrigin = new Point(0.5, 0.5), Image = loadedImage });
            }
        }

        public void LoadImage(string name, Stream imageStream, Stream dataStream, bool leaveOpen) {
            BitmapImage image = new BitmapImage( );
            image.BeginInit( );
            image.StreamSource = imageStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit( );

            Point transformOrigin = JsonConvert.DeserializeObject<Point>(new StreamReader(dataStream).ReadToEnd( ));

            if (Images.ContainsKey(name)) {
                Images[name] = new ImageData( ) { Image = image, TransformOrigin = transformOrigin };
            } else {
                Images.Add(name, new ImageData( ) { Image = image, TransformOrigin = transformOrigin });
            }

            if (!leaveOpen) {
                imageStream.Close( );
                dataStream.Close( );
            }
        }

        public void SaveTo (Project project) {
            using (Stream stream = project.GetOrCreateStream(true, "animations", Meta.Entity, ".meta"))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(Meta));

            using (Stream stream = project.GetOrCreateStream(true, "animations", Meta.Entity, "animations.json"))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(Animations.ToArray( )));

            using (Stream stream = project.GetOrCreateStream(true, "animations", Meta.Entity, "bones.json"))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(JsonConvert.SerializeObject(Bones.ToArray( )));

            foreach (KeyValuePair<string, ImageData> image in Images) {
                using (Stream stream = project.GetOrCreateStream(true, "animations", Meta.Entity, "textures", image.Key, ".png"))
                    image.Value.Image.SaveToStream(stream);
                using (Stream stream = project.GetOrCreateStream(true, "animations", Meta.Entity, "textures", image.Key, ".data"))
                using (StreamWriter writer = new StreamWriter(stream))
                    writer.WriteLine(JsonConvert.SerializeObject(image.Value.TransformOrigin));
            }
        }

        public void Compile (string path, Project project) {
            string basedirectory = Path.Combine(path, "animations", Meta.Entity);
            if (!Directory.Exists(basedirectory)) Directory.CreateDirectory(basedirectory);

            using (Stream stream = File.Open(Path.Combine(basedirectory, "animation.json"), FileMode.Create))
                AnimationSerizalizer.Compile(Animations.ToArray( ), stream, SelectBonesDialog.GetSelectedBones( ));
        }
    }
}
