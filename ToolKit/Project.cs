using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using mapKnight.ToolKit.Controls.Xna;
using mapKnight.ToolKit.Data;
using mapKnight.ToolKit.Serializer;
using mapKnight.ToolKit.Windows;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace mapKnight.ToolKit {
    public class Project {
        public event Action LocationChanged;

        public bool HasLocation { get { return Location != null; } }

        private string _Location;
        public string Location {
            get { return _Location; }
            set { _Location = value; LocationChanged?.Invoke( ); }
        }

        public GraphicsDevice GraphicsDevice;
        public ObservableCollection<EditorMap> Maps = new ObservableCollection<EditorMap>( );
        public ObservableCollection<VertexAnimationData> Animations = new ObservableCollection<VertexAnimationData>( );

        public override string ToString ( ) {
            return "PAROMA";
        }

        public Project ( ) {
            if (!CreateGraphicsDevice( )) {
                App.Current.MainWindow.IsVisibleChanged += (sender, e) => CreateGraphicsDevice( );
            }
        }

        public Project (string location, EditorWindow window) : this( ) {
            Location = location;
            if (!HasLocation) return;

            window.tabcontrol_editor.Items.Clear( );
            ZipArchive archive = new ZipArchive(File.Open(location, FileMode.Open), ZipArchiveMode.Update, false);

            // Load Maps
            foreach (string file in archive.GetAllEntries("maps")) {
                if (Path.GetExtension(file) == ".map") {
                    string dir = Path.GetDirectoryName(file);
                    string name = Path.GetFileNameWithoutExtension(file);

                    using (Stream mapStream = archive.GetOrCreateStream(false, file)) {
                        EditorMap loadedMap = new EditorMap(mapStream);
                        using (Stream imageStream = archive.GetOrCreateStream(false, dir, name + ".png")) {
                            imageStream.Read(loadedMap.ImageData = new byte[(int)imageStream.Length], 0, (int)imageStream.Length);
                        }

                        if (archive.Contains(dir, name + ".brushes")) {
                            using (Stream brushStream = archive.GetOrCreateStream(false, dir, name + ".brushes"))
                            using (StreamReader reader = new StreamReader(brushStream))
                                loadedMap.Brushes.AddRange(JsonConvert.DeserializeObject<List<TileBrush>>(reader.ReadToEnd( )));
                        }

                        Maps.Add(loadedMap);
                    }
                }
            }

            // Load Animations
            foreach (string file in archive.GetAllEntries("animations")) {
                if (Path.GetFileName(file) == ".meta") {
                    string dir = Path.GetDirectoryName(file);
                    VertexAnimationData data = new VertexAnimationData( );

                    foreach (string texturedir in archive.GetAllEntries(dir, "textures")) {
                        if (Path.GetFileName(texturedir) == ".png") {
                            string name = new DirectoryInfo(System.IO.Path.GetDirectoryName(texturedir)).Name;
                            data.LoadImage(name, archive.GetOrCreateStream(false, texturedir), archive.GetOrCreateStream(false, System.IO.Path.ChangeExtension(texturedir, ".data")), false);
                        }
                    }

                    using (Stream stream = archive.GetOrCreateStream(false, dir, ".meta"))
                        data.Meta = JsonConvert.DeserializeObject<AnimationMetaData>(new StreamReader(stream).ReadToEnd( ));
                    using (Stream stream = archive.GetOrCreateStream(false, dir, "bones.json"))
                        data.Bones.AddRange(JsonConvert.DeserializeObject<VertexBone[ ]>(new StreamReader(stream).ReadToEnd( )));
                    foreach (VertexBone bone in data.Bones)
                        bone.SetBitmapImage(data);
                    using (Stream stream = archive.GetOrCreateStream(false, dir, "animations.json"))
                        data.Animations.AddRange(JsonConvert.DeserializeObject<VertexAnimation[ ]>(new StreamReader(stream).ReadToEnd( )));

                    Animations.Add(data);
                }
            }

            archive.Dispose( );
        }

        public void Save ( ) {
            if (File.Exists(Location)) {
                File.Delete(Location);
            }
            ZipArchive archive = new ZipArchive(File.Create(Location), ZipArchiveMode.Update, false);

            foreach (EditorMap map in Maps) {
                map.SaveTo(this, archive);
            }

            foreach (VertexAnimationData animation in Animations) {
                animation.SaveTo(this, archive);
            }

            archive.Dispose( );
        }

        public void Compile (string path) {
            string mappath = Path.Combine(path, "maps");
            foreach (EditorMap map in Maps) {
                string basedirectory = Path.Combine(mappath, map.Name);
                if (!Directory.Exists(basedirectory))
                    Directory.CreateDirectory(basedirectory);
                // build texture
                Texture2D packedTexture = TileSerializer.BuildTexture(map.Tiles, map.XnaTextures, GraphicsDevice);
                using (Stream stream = File.Open(Path.Combine(basedirectory, map.Name + ".png"), FileMode.Create))
                    packedTexture.SaveAsPng(stream, packedTexture.Width, packedTexture.Height);

                map.Texture = Path.GetFileNameWithoutExtension(map.Name + ".png");
                using (Stream stream = File.Open(Path.Combine(basedirectory, map.Name + ".map"), FileMode.Create))
                    map.CreateCompileVersion( ).Serialize(stream, new WindowsEntitySerializer( ));

                using (Stream stream = File.Open(Path.Combine(basedirectory, map.Name + "_shadow.png"), FileMode.Create))
                    map.PrerenderShadowMap(stream);
            }

            string animationpath = Path.Combine(path, "animations");
            foreach (VertexAnimationData animation in Animations) {
                string basedirectory = Path.Combine(animationpath, animation.Meta.Entity);
                if (!Directory.Exists(basedirectory)) Directory.CreateDirectory(basedirectory);

                using (Stream stream = File.Open(Path.Combine(basedirectory, "animation.json"), FileMode.Create))
                    AnimationSerizalizer.Compile(animation.Animations.ToArray( ), stream, animation.SelectBonesDialog.GetSelectedBones( ));
            }
        }

        private bool CreateGraphicsDevice ( ) {
            if (GraphicsDevice == null && App.Current.MainWindow.IsVisible) {
                HwndSource source = PresentationSource.FromVisual(App.Current.MainWindow) as HwndSource;
                if (source == null)
                    return false;
                GraphicsDevice = GraphicsDeviceService.AddRef(source.Handle).GraphicsDevice;
                return true;
            }
            return false;
        }
    }
}
