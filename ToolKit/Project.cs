using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

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

        private ZipArchive archive;
        private string _Path;

        public Project (string path) {
            Path = path;
            archive = new ZipArchive(File.Open(path, FileMode.Open), ZipArchiveMode.Update, false);
        }

        public Project ( ) : this(System.IO.Path.GetTempFileName( )) {
            HasPath = false;
        }

        public void Save ( ) {
            archive.Dispose( );
            archive = new ZipArchive(File.Open(Path, FileMode.Open), ZipArchiveMode.Update, false);
            Updated = false;
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
                    archive.GetEntry(internalpath);
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
