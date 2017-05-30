using mapKnight.ToolKit.Controls.Xna;
using mapKnight.ToolKit.Data;
using mapKnight.ToolKit.Serializer;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

namespace mapKnight.ToolKit {
    public class Project {
        public const string EXTENSION = ".mkproj";
        public const CompressionLevel COMPRESSION = CompressionLevel.Fastest;

        public bool HasPath { get; set; }
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
                HasPath = true;
            }
        }
        public bool Updated { get; private set; }

        public GraphicsDevice GraphicsDevice;
        public ObservableCollection<EditorMap> Maps = new ObservableCollection<EditorMap>( );
        public ObservableCollection<VertexAnimationData> Animations = new ObservableCollection<VertexAnimationData>( );

        private ZipArchive archive;
        private string _Path;

        public Project (string path) {
            Path = path;
            archive = new ZipArchive(File.Open(path, FileMode.Open), ZipArchiveMode.Update, false);

            App.Current.MainWindow.IsVisibleChanged += MainWindow_IsVisibleChanged;
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

        public Project ( ) : this(System.IO.Path.GetTempFileName( )) {
            HasPath = false;
        }

        public void Save ( ) {
            archive.Dispose( );

            foreach (EditorMap map in Maps) {
                map.SaveTo(this);
            }

            foreach (VertexAnimationData animation in Animations) {
                animation.SaveTo(this);
            }

            archive = new ZipArchive(File.Open(Path, FileMode.Open), ZipArchiveMode.Update, false);
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
