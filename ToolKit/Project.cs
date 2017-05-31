using mapKnight.ToolKit.Controls;
using mapKnight.ToolKit.Controls.Xna;
using mapKnight.ToolKit.Data;
using mapKnight.ToolKit.Editor;
using mapKnight.ToolKit.Serializer;
using mapKnight.ToolKit.Windows;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;

namespace mapKnight.ToolKit {
    public class Project {
        public const string EXTENSION = ".mkproj";
        public const CompressionLevel COMPRESSION = CompressionLevel.Fastest;

        public bool HasPath { get { return _Path != null; } }

        private string _Path;
        public string Path {
            get {
                return _Path;
            }
            set {
                if (_Path != null) {
                    archive.Dispose( );
                    if (File.Exists(value)) File.Delete(value);
                    File.Copy(_Path, value);
                    archive = new ZipArchive(File.Open(value, FileMode.Open), ZipArchiveMode.Update, false);
                }
                _Path = value;
            }
        }
        public bool Updated { get; private set; }

        public GraphicsDevice GraphicsDevice;
        public ObservableCollection<EditorMap> Maps = new ObservableCollection<EditorMap>( );
        public ObservableCollection<VertexAnimationData> Animations = new ObservableCollection<VertexAnimationData>( );

        private ZipArchive archive;

        public Project ( ) {
        }

        public Project (string path, EditorWindow window) {
            Path = path;
            window.IsVisibleChanged += MainWindow_IsVisibleChanged;
            if (!HasPath) return;

            window.tabcontrol_editor.Items.Clear( );
            archive = new ZipArchive(File.Open(path, FileMode.Open), ZipArchiveMode.Update, false);
    
            // Load Maps
            foreach (string file in GetAllEntries("maps")) {
                if (System.IO.Path.GetExtension(file) == ".map") {
                    string dir = System.IO.Path.GetDirectoryName(file);
                    string name = System.IO.Path.GetFileNameWithoutExtension(file);

                    using (Stream mapStream = GetOrCreateStream(false, file)) {
                        EditorMap loadedMap = new EditorMap(mapStream);
                        using (Stream imageStream = GetOrCreateStream(false, dir, name + ".png")) {
                            imageStream.Read(loadedMap.ImageData = new byte[(int)imageStream.Length], 0, (int)imageStream.Length); 
                        }
                        Maps.Add(loadedMap);
                    }
                }
            }

            // Load Animations
            foreach (string file in GetAllEntries("animations")) {
                if (System.IO.Path.GetFileName(file) == ".meta") {
                    string dir = System.IO.Path.GetDirectoryName(file);
                    VertexAnimationData data = new VertexAnimationData( );

                    foreach (string texturedir in GetAllEntries(dir, "textures")) {
                        if (System.IO.Path.GetFileName(texturedir) == ".png") {
                            string name = new DirectoryInfo(System.IO.Path.GetDirectoryName(texturedir)).Name;
                            data.LoadImage(name, GetOrCreateStream(false, texturedir), GetOrCreateStream(false, System.IO.Path.ChangeExtension(texturedir, ".data")), false);
                        }
                    }
                     
                    using (Stream stream = GetOrCreateStream(false, dir, ".meta"))
                        data.Meta = JsonConvert.DeserializeObject<AnimationMetaData>(new StreamReader(stream).ReadToEnd( ));
                    using (Stream stream = GetOrCreateStream(false, dir, "bones.json"))
                        data.Bones = new ObservableCollection<VertexBone>(JsonConvert.DeserializeObject<VertexBone[ ]>(new StreamReader(stream).ReadToEnd( )));
                    using (Stream stream = GetOrCreateStream(false, dir, "animations.json"))
                        data.Animations = new ObservableCollection<VertexAnimation>(JsonConvert.DeserializeObject<VertexAnimation[ ]>(new StreamReader(stream).ReadToEnd( )));

                    Animations.Add(data);
                }
            }

            archive.Dispose( );
        }

        private void MainWindow_IsVisibleChanged (object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
            if (GraphicsDevice != null)
                return;
            if (App.Current.MainWindow.IsVisible) {
                HwndSource source = PresentationSource.FromVisual(App.Current.MainWindow) as HwndSource;
                if (source == null)
                    return;
                GraphicsDevice = GraphicsDeviceService.AddRef(source.Handle).GraphicsDevice;
            }
        }

        public void Save ( ) {
            archive = new ZipArchive(File.Open(Path, FileMode.Open), ZipArchiveMode.Update, false);

            foreach (EditorMap map in Maps) {
                map.SaveTo(this);
            }

            foreach (VertexAnimationData animation in Animations) {
                animation.SaveTo(this);
            }

            archive.Dispose( );
            Updated = false;
        }

        public void Compile (string path) {

        }

        public bool Contains (params string[ ] path) {
            return Contains(System.IO.Path.Combine(path));
        }

        public bool Contains (string path) {
            return archive.Entries.Any(entry => entry.FullName == path);
        }

        public IEnumerable<string> GetAllEntries (params string[ ] path) {
            string internalpath = System.IO.Path.Combine(path);
            foreach (ZipArchiveEntry entry in archive.Entries) {
                if (entry.FullName.StartsWith(internalpath)) yield return entry.FullName;
            }
        }

        public Stream GetOrCreateStream (bool forcenew, params string[ ] path) {
            Updated = true;
            string internalpath = System.IO.Path.Combine(path);
            if (Contains(internalpath)) {
                if (forcenew) {
                    archive.GetEntry(internalpath)?.Delete( );
                    return archive.CreateEntry(internalpath, COMPRESSION).Open( );
                } else {
                    return archive.GetEntry(internalpath).Open( );
                }
            } else {
                return archive.CreateEntry(internalpath, COMPRESSION).Open( );
            }
        }
    }
}
